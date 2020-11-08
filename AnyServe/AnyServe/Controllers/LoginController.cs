using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnyServe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnyServe.Controllers
{

    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        //TODO: Where we are going to store AnyServ server users?
        private List<AnyServUser> _appUsers = new List<AnyServUser>
        {
            new AnyServUser { FullName = "Big Admin", UserName = "admin", Password = "1234", UserRole = "Admin" },
            new AnyServUser { FullName = "Test User", UserName = "user", Password = "1234", UserRole = "User" }
        };

        private IConfiguration _config;
        private readonly ILogger<LoginController> _logger;
        

        public LoginController(IConfiguration config, ILogger<LoginController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AnyServUser login)
        {
            IActionResult response = Unauthorized();
            AnyServUser user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                response = Ok(new
                {
                    token = tokenString,
                    userDetails = user,
                });
            }
            return response;
        }


        private AnyServUser AuthenticateUser(AnyServUser loginCredentials)
        {
            AnyServUser user = _appUsers.SingleOrDefault(x => x.UserName == loginCredentials.UserName && x.Password == loginCredentials.Password);
            return user;
        }

        private string GenerateJWTToken(AnyServUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("fullName", userInfo.FullName.ToString()),
                new Claim("role",userInfo.UserRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
