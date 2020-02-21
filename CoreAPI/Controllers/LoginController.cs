using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreAPI.Model;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }


        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            //return new string[] { _config["Jwt:Key"], _config["Jwt:Issuer"], _config["ConnectionStrings:Default"] };
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            return new string[] { claim[0].Value, claim[1].Value, claim[2].Value };
        }


        [HttpPost]
        public IActionResult Post([FromBody] UserModel data)
        {

            UserModel login = new UserModel();
            login.UserID = data.UserID;
            login.Password = data.Password;
            IActionResult response = Unauthorized();

            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenStr = GenerateJasonWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;

            
        }

       
        private string GenerateJasonWebToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserID),
                new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            string connstr = _config["ConnectionStrings:Default"].ToString();

            using (IDbConnection conn = new SqlConnection(connstr))
            {
                string query = "SELECT UserID, UserName, Email FROM AppUser Where UserID = @UserID AND Password = @Password ";
                UserModel usr = SqlMapper.Query<UserModel>(conn, query, new { login.UserID,login.Password  }).FirstOrDefault();

                return usr;
            }

        }
    }
}