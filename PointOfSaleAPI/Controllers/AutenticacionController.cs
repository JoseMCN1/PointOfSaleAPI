using PointOfSaleAPI.Data;
using PointOfSaleAPI.Data.DTOs;
using PointOfSaleAPI.Model;
using PointOfSaleAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FactuProAPI.Controllers
{
    [ApiController]
    [Route("api/autenticacion")]
    public class AutenticacionController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly BDFactuProContext _context;
        private readonly string _pepper = null;
        private readonly int _iteration = 3;

        public AutenticacionController(IConfiguration configuration, BDFactuProContext context)
        {
            _configuration = configuration;
            _context = context;
            _pepper = Environment.GetEnvironmentVariable("PasswordHashPepper");
        }

        [HttpPost]
        [Route("registro")]
        public async Task<ActionResult<RegistroResponse>> SignUp(RegistroRequest registroRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuarioTemporal = await _context.Usuarios.FirstOrDefaultAsync(x => x.Username == registroRequest.Username);

                if (usuarioTemporal != null)
                    return BadRequest(new
                    {
                        Mensaje = "El nombre de usuario ya esta en uso"
                    }) ;

                Usuario modeloUsuario = new Usuario
                {
                    Username = registroRequest.Username,
                    ContrasenaSalt = ContrasenaHasher.GenerateSalt()
                };

                modeloUsuario.ContrasenaHash = ContrasenaHasher.ComputeHash(registroRequest.Contrasena, modeloUsuario.ContrasenaSalt, _pepper, _iteration);
                await _context.AddAsync(modeloUsuario);
                await _context.SaveChangesAsync();


                return Ok(new RegistroResponse
                {
                    Estatus = "Exito",
                    Mensaje = "Usuario Registrado"
                });
            }

            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex.Message}");

                return StatusCode(500, "Internal Server Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var  usuarioTemporal = await _context.Usuarios.FirstOrDefaultAsync(x => x.Username == loginRequest.Username);

                if (usuarioTemporal == null)
                    return BadRequest(new
                    {
                        Mensaje = "Datos Erroneos"
                    });

                var contrasenaHash = ContrasenaHasher.ComputeHash(loginRequest.Contrasena, usuarioTemporal.ContrasenaSalt, _pepper, _iteration);

                if (usuarioTemporal.ContrasenaHash != contrasenaHash)
                    return BadRequest(new
                    {
                        Mensaje = "Datos Erroneos"
                    });

                //token information

                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,jwt.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                    new Claim("UsuarioId",usuarioTemporal.Id.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: signIn
                    );

                return Ok(new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Username = usuarioTemporal.Username
                }); 
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex.Message}");

                return StatusCode(500, "Internal Server Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }

        }

    }
}
