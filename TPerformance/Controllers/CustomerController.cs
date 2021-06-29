using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TPerformance.ModelMapping;
using TPerformance.Models;

namespace TPerformance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TPContext _dbcontext;

        public CustomerController(IConfiguration configuration,TPContext dbcontext)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody]CustomerLogin model)
        {
            if (string.IsNullOrEmpty(model.Mail) || (string.IsNullOrEmpty(model.Password)))
                return StatusCode(400, "email or password could nor be nll or empty");


            IActionResult response = Unauthorized();

            // check user existence
            var user = _dbcontext.customers.FirstOrDefault(x => x.Mail == model.Mail);

            if (user == null)
                return NotFound("Credential email is not Exist! ");

            // check if password is correct
            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
                return NotFound("Credential is not Correct! ");

            // Generate userJWT
            var token = GenerateJWT(user);

            return Ok(token);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]CustomerRegister model )
        {
            if (string.IsNullOrEmpty(model.Mail) || (string.IsNullOrEmpty(model.Password)))
                return StatusCode(400, "email or password could nor be null or empty");

            
            if(ModelState.IsValid)
            {
                var customer = new Customer
                {
                    Mail = model.Mail,
                    Password = model.Password,
                    UserNAME = model.UserNAME,
                    CustomerRole = model.CustomerRole.Equals("Admin".ToLower()) ? 1 : 2
                };

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(customer.Password, out passwordHash, out passwordSalt);
                customer.PasswordHash = passwordHash;
                customer.PasswordSalt = passwordSalt;
                try
                {
                    // save customer into db
                    await _dbcontext.customers.AddAsync(customer);
                    await _dbcontext.SaveChangesAsync();

                    //generate token 
                    string token = GenerateJWT(customer);
                    return Ok(token);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "could not register customer");
                }
            }

          return StatusCode(400, "model is not valid");
           
            
        }

        private string GenerateJWT(Customer customer)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , customer.UserNAME),
                new Claim(JwtRegisteredClaimNames.Email,customer.Mail),
                new Claim(ClaimTypes.Role , customer.CustomerRole == 1 ? "Admin" : "Customer")
            };
            var token = new JwtSecurityToken(
                issuer: "https://www.teleperformance.com/",
                audience: "https://www.teleperformance.com/",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials:credentials);

            var JWtoken = new JwtSecurityTokenHandler().WriteToken(token);

            return JWtoken; 
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password could not be null");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
