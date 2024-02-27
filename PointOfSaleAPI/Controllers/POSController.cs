using PointOfSaleAPI.Data.DTOs;
using PointOfSaleAPI.Model;
using PointOfSaleAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FacturacionAPI.Controllers
{
    [Authorize]
    [Route("api/pos")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly BDFactuProContext _context;
        private readonly IConfiguration _configuration;

        public POSController(BDFactuProContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/pos/{facturaId}
        [HttpGet("{facturaId}")]
        public async Task<ActionResult<FacturaResponse>> GetFactura(int facturaId)
        {
            try
            {
                Factura factura = await _context.Facturas
                .Include(f => f.ArticuloFacturas)
                    .ThenInclude(af => af.IdArticuloNavigation) // Include related Articulo entity
                .FirstOrDefaultAsync(f => f.Id == facturaId);

                if (factura == null )
                {
                    return NotFound();
                }

                FacturaResponse facturaResponse = new FacturaResponse
                {
                    Id = factura.Id,
                    Consecutivo = factura.Consecutivo,
                    NumeroFactura = factura.NumeroFactura,
                    VendedorId = factura.VendedorId,
                    FechaCreacion = factura.FechaCreacion,
                    Subtotal = factura.Subtotal,
                    MontoTotal = factura.MontoTotal,
                    ArticuloFacturas = factura.ArticuloFacturas
                        .Select(af => new ArticuloFacturaResponse
                        {
                            IdArticulo = af.IdArticulo,
                            IdFactura = af.IdFactura,
                            MontoTotal = af.MontoTotal,
                            Cantidad = af.Cantidad,
                            PrecioUnitario = af.PrecioUnitario,
                            AplicaIva = af.AplicaIva,
                            FechaAgregado = af.FechaAgregado,
                            Codigo = af.IdArticuloNavigation.Codigo
                        })
                        .ToList()
                };

                return Ok(facturaResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // POST: api/pos
        [HttpPost]
        public async Task<ActionResult<POSResponse>> ProcessPOSRequest(POSRequest posRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Obtener el id del usuario que esta almacenado en el token
                General funcionesGenerales = new General();
                string tokenHeader = HttpContext.Request.Headers["Authorization"];
                string[] token = tokenHeader.Split(" ");

                ClaimsPrincipal claimsPrincipal = funcionesGenerales.GetPrincipalFromToken(token[1], _configuration);

                Claim usuarioIdClaim = claimsPrincipal.FindFirst("UsuarioId");

                int usuarioId = int.Parse(usuarioIdClaim.Value);

                //simular llamo a hacienda
                var haciendaResponse = HaciendaSimulatorAPI.SimularLlamadoApiHacienda();

                
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //insertar factura

                        Factura factura = new Factura
                        {
                            Consecutivo = haciendaResponse.consecutivoElectronico,
                            NumeroFactura = haciendaResponse.claveElectronica,
                            VendedorId = usuarioId,
                            FechaCreacion = DateTime.Now,
                            Subtotal = posRequest.SubTotal,
                            MontoTotal = posRequest.MontoTotal
                        };

                        _context.Add(factura);

                        await _context.SaveChangesAsync();

                        List<ArticuloFacturaRequest> listaArticulos = posRequest.Articulos;

                        foreach (ArticuloFacturaRequest articuloFacturaRequest in listaArticulos)
                        {
                            ArticuloFactura articuloFactura = new ArticuloFactura
                            {
                                IdArticulo = articuloFacturaRequest.IdArticulo,
                                IdFactura = factura.Id,
                                MontoTotal = articuloFacturaRequest.MontoTotal,
                                Cantidad = articuloFacturaRequest.Cantidad,
                                PrecioUnitario = articuloFacturaRequest.PrecioUnitario,
                                AplicaIva = articuloFacturaRequest.AplicaIva,
                                FechaAgregado = DateTime.Now
                            };

                            _context.Add(articuloFactura);
                        }

                        // Si todo sale bien

                        await transaction.CommitAsync();

                        await _context.SaveChangesAsync();

                        return Ok(new POSResponse { Id = factura.Id });

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");

                        // Si algo falla
                        await transaction.RollbackAsync();

                        return StatusCode(500, "Internal Server Error");
                    }
                
                }
                       
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        

    }
}
