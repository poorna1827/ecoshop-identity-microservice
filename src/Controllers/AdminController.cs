using IdentityMicroservice.DbContexts;
using IdentityMicroservice.Dto;
using IdentityMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityMicroservice.Controllers
{
    [Route("api/rest/v1/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(IdentityDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }



        [HttpPost("register")]
        public async Task<IActionResult> AdminRegister(AdminRegisterDto data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool hasConflitname = _context.Admins.Where(x => x.Email == data.Email).Any();

            if (hasConflitname)
            {
                return Conflict();
            }


            Admin new_record = new Admin()
            {
                AId = Guid.NewGuid(),
                Email = data.Email,
                Password = data.Password,
                Name = data.Name,
            };
            await _context.Admins.AddAsync(new_record);
            await _context.SaveChangesAsync();

            return Ok();
        }



        [HttpPost("login")]
        public IActionResult AdminLogin(LoginDto data)
        {
            // Step 1: validate the username/password

            var record = _context.Admins.FirstOrDefault(x => x.Email == data.Email);

            if (record == null)
            {
                return NotFound();
            }

            if (record.Password != data.Password)
            {
                return Unauthorized();
            }


            // Step 2: create a token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretKey"]!));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("Id", record.AId.ToString()));
            claimsForToken.Add(new Claim("given_name", record.Name!));
            claimsForToken.Add(new Claim("UserType", "Admin"));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            return Ok(new { token = tokenToReturn });
        }



    }
}
