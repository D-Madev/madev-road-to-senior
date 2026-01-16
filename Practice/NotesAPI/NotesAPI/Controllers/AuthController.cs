// namespace NotesAPI.Controllers;

// using Microsoft.Extensions.Configuration;
// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;
// using System.Text;

// [ApiController]
// [Route("auth")]
// public class AuthController : ControllerBase
// {
//     private readonly IConfiguration _configuration;

//     // Inyectar IConfiguration para acceder a leer la clave JWT
//     public AuthController(IConfiguration configuration) => _configuration = configuration;

//     // Modelo dummy para simular la peticion de login 
//     public record LoginRequest(string Username, string Password);

//     [HttpPost("login")]
//     public IActionResult Login([FromBody] LoginRequest request)
//     {
//         // Simulacion de validacion de credenciales
//         if (request.Username != "testuser" || request.Password != "password")
//         {
//             return Unauthorized("Credenciales invalidas.");
//         }

//         // Definir los claims
//         var claims = new[]
//         {
//             new Claim(JwtRegisteredClaimNames.Sub, request.Username),
//             new Claim(ClaimTypes.Role, "ADMIN")
//         };

//         // Leer clave secreta y crear credenciales
//         var jwtKey = _configuration["Jwt:Key"] ?? "Esta_Es_Una_Clave_Muy_Larga_De_Prueba_32_Chars";
//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
//         key.KeyId = "NotesApiKeyId";
//         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//         // Definir el token
//         var token = new JwtSecurityToken(
//             issuer: "NotesAPI",
//             audience: "NotesClient",
//             claims: claims,
//             expires: DateTime.UtcNow.AddHours(1),
//             signingCredentials: creds
//         );

//         // Devolver el token
//         return Ok(new
//         {
//             token = new JwtSecurityTokenHandler().WriteToken(token)
//         });
//     }
// }
