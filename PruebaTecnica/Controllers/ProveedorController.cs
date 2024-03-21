using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using PruebaTecnica.Models;
using PruebaTecnica.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace PruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : Controller
    {
        private static readonly string secretKey = "PruebaTecnicaSecretKey1234567890";

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Aquí simulas la autenticación de usuarios
            if (model.Username == "admin" && model.Password == "admin")
            {
                // Si las credenciales son válidas, generas un token JWT
                var tokenString = GenerateJwtToken(model.Username);
                return Ok(new { Token = tokenString });
            }

            // Si las credenciales son inválidas, devuelves un Unauthorized
            return Unauthorized();
        }

        private ProveedorCollection db = new ProveedorCollection();
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProveedor()
        {
            return Ok(await db.GetAllProveedor());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProveedorDetailis(string id)
        {
            return Ok(await db.GetProveedorById(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProveedor([FromBody] Proveedor proveedor)
        {
            if (proveedor == null)
                return BadRequest();
            if (proveedor.NombreContacto == string.Empty)
            {
                ModelState.AddModelError("Nombre Contacto", "The proveedor shouldn't be empty");
            }

            await db.InserProveedor(proveedor);

            return Created("Created", true);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProveedor(string id, [FromBody] Dictionary<string, object> fieldsToUpdate)
        {
            if (fieldsToUpdate == null || fieldsToUpdate.Count == 0)
                return BadRequest("No fields provided for update.");

            var proveedor = await db.GetProveedorById(id);
            if (proveedor == null)
                return NotFound();

            foreach (var field in fieldsToUpdate)
            {
                if (field.Key == "Id") // Evitar que se actualice el Id
                    continue;

                var property = proveedor.GetType().GetProperty(field.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    // Convertir el valor del campo al tipo esperado por la propiedad
                    var value = Convert.ChangeType(field.Value.ToString(), property.PropertyType);
                    property.SetValue(proveedor, value);
                }
                else
                {
                    ModelState.AddModelError(field.Key, $"The field {field.Key} cannot be updated or does not exist.");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await db.UpdateProveedor(proveedor);

            return Ok(proveedor);
        }


        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteProveedor(string id)
        {
            await db.DeleteProveedor(id);

            return NoContent();
        }


        private string GenerateJwtToken(string username)
        {
            // Generar una clave secreta aleatoria
            var hmac = new System.Security.Cryptography.HMACSHA256();
            var keyBytes = hmac.Key;

            // Utilizar la clave generada para firmar el token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Configuración de la firma del token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims del token (pueden ser personalizados según tus necesidades)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Configuración del token JWT
            var token = new JwtSecurityToken(
                issuer: "tu_issuer",
                audience: "tu_audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Tiempo de expiración del token
                signingCredentials: creds
            );

            // Generar el token como una cadena JWT
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
