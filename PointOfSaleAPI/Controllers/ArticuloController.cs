using PointOfSaleAPI.Data.DTOs;
using PointOfSaleAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleAPI.Controllers
{
    [Authorize]
    [Route("api/articulos")]
    [ApiController]
    public class ArticuloController : ControllerBase
    {

        private readonly BDFactuProContext _context;

        public ArticuloController(BDFactuProContext context)
        {
            _context = context;
        }

        // GET: api/articulos
        [HttpGet]
        public async Task<ActionResult<ArticulosResponse>> GetArticulos(
            [FromQuery] QueryParametros parametros)
        {
            try
            {
                // Query para filtrar por codigo si es enviado en el request
                var query = _context.Articulos
                    .Where(a => a.Eliminado == 0);

                if (!string.IsNullOrEmpty(parametros.Code))
                {
                    query = query.Where(a => a.Codigo == parametros.Code.ToUpper());
                }

                // Continuar con la logica normal
                var articulos = await query
                    .Skip((parametros.Page - 1) * parametros.PageSize)
                    .Take(parametros.PageSize)
                    .Select(a => new ArticuloResponse
                    {
                        Id = a.Id,
                        Codigo = a.Codigo,
                        Nombre = a.Nombre,
                        Precio = a.Precio,
                        AplicaIva = a.AplicaIva
                    })
                    .ToListAsync();

                return Ok(new ArticulosResponse
                {
                    Articulos = articulos,
                    CurrentPage = parametros.Page,
                    ItemsPerPage = parametros.PageSize,
                    Total = _context.Articulos.Count()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: api/articulos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloResponse>> GetArticulo(int id)
        {
            try
            {
                var articulo = await _context.Articulos.FindAsync(id);

                if (articulo == null || articulo.Eliminado != 0)
                {
                    return NotFound();
                }

                return Ok(new ArticuloResponse
                {
                    Id=articulo.Id,
                    Codigo = articulo.Codigo,
                    Precio = articulo.Precio,
                    Nombre = articulo.Nombre,
                    AplicaIva = articulo.AplicaIva
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // POST: api/articulos
        [HttpPost]
        public async Task<ActionResult<ArticuloResponse>> PostArticulo(ArticuloRequest articulo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var articuloTemporal = await _context.Articulos.FirstOrDefaultAsync(x => x.Codigo == articulo.Codigo.ToUpper());

                if (articuloTemporal != null)
                    return BadRequest(new
                    {
                        Mensaje = "El codigo ya esta en uso"
                    });

                var nuevoArticulo = new Articulo
                {
                    Nombre = articulo.Nombre,
                    Codigo = articulo.Codigo.ToUpper(),
                    Precio = articulo.Precio,
                    AplicaIva = articulo.AplicaIva,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Eliminado = 0
                }; 
                
                _context.Articulos.Add(nuevoArticulo);


                await _context.SaveChangesAsync();

                return Created("GetArticulo",nuevoArticulo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // PUT: api/articulos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo(int id, ArticuloRequest articulo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var articuloTemporalPorId = await _context.Articulos.FirstOrDefaultAsync(x => x.Id == id);

                if (articuloTemporalPorId == null)
                {
                    return NotFound();
                }

                var articuloTemporalPorCodigo = await _context.Articulos.FirstOrDefaultAsync(x => x.Codigo == articulo.Codigo.ToUpper());

                if (articuloTemporalPorCodigo != null && articuloTemporalPorId.Codigo.ToUpper() != articuloTemporalPorCodigo.Codigo.ToUpper())
                    return BadRequest(new
                    {
                        Mensaje = "El codigo ya esta en uso"
                    });

                articuloTemporalPorId.Nombre = articulo.Nombre;
                articuloTemporalPorId.Codigo = articulo.Codigo.ToUpper();
                articuloTemporalPorId.Precio = articulo.Precio;
                articuloTemporalPorId.AplicaIva = articulo.AplicaIva;
                articuloTemporalPorId.FechaActualizacion = DateTime.Now;

                _context.Update(articuloTemporalPorId);

                await _context.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // DELETE: api/articulos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticulo(int id)
        {
            try
            {
                var articulo = await _context.Articulos.FindAsync(id);

                if (articulo == null || articulo.Eliminado != 0)
                {
                    return BadRequest(new
                    {
                        Mensaje = "Articulo no encontrado"
                    });
                }

                articulo.Eliminado = 1; // Soft delete

                _context.Update(articulo);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Estatus = "Exito",
                    Mensaje = "Articulo Eliminado!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
