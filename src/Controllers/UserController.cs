using IdentityMicroservice.DbContexts;
using IdentityMicroservice.Dto;
using IdentityMicroservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityMicroservice.Controllers
{
    [Route("api/rest/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(IdentityDbContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ??
                            throw new ArgumentNullException(nameof(configuration));
        }



        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(UserRegisterDto data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool hasConflitname = _context.Users.Where(x => x.Email == data.Email).Any();

            if (hasConflitname)
            {
                return Conflict();
            }


            User new_record = new User()
            {
                CId = Guid.NewGuid(),
                Email = data.Email,
                Password = data.Password,
                Name = data.Name

            };
            await _context.Users.AddAsync(new_record);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult UserLogin(LoginDto data)
        {
            // Step 1: validate the username/password

            var record = _context.Users.FirstOrDefault(x => x.Email == data.Email);

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
            claimsForToken.Add(new Claim("Id", record.CId.ToString()));
            claimsForToken.Add(new Claim("given_name", record.Name!));
            claimsForToken.Add(new Claim("UserType", "Customer"));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            return Ok(new { name = record.Name,token = tokenToReturn });
        }

        [HttpGet("currentuser")]
        [Authorize(Policy = "MustBeCustomer")]
        public ActionResult CurrentUser()
        {

            var id = User.FindFirst("Id")?.Value;

            Console.WriteLine(id);

            var record = _context.Users.Find(new Guid(id!));

            // Step 2: create a token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretKey"]!));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("Id", record!.CId.ToString()));
            claimsForToken.Add(new Claim("given_name", record.Name!));
            claimsForToken.Add(new Claim("UserType", "Customer"));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            Console.WriteLine("in the current user controller");
            return Ok(new { name = record.Name, token = tokenToReturn });


        }
    }
}
