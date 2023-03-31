using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JWTSettings _jwtSettings;
        Uri address;
        string url;

        public UserController(IUserService userService, IOptions<JWTSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Add a new Student/Admin Account to Library
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(UserModel model)
        {

            address = new Uri(Request.Host.ToString());
            url = address.ToString() + "/account";

            if (model == null)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.CreateAccount(model, url);
            return Ok(result);
        }

        /// <summary>
        /// Get the list of Users
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> GetAllAccount()
        {
            long id = CurrentUserId;
            address = new Uri(Request.Host.ToString());
            url = address.ToString() + "/account";

            var accountList = await _userService.GetAllUsers(url);
            if (accountList == null)
            {
                return NotFound();
            }
            return Ok(accountList);
        }



        /// <summary>
        /// Login to Library
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login model)
        {
            if(model == null)
            {
                return BadRequest("Enter username and password");
            }
            var result =  _userService.GetLogin(model);
            if (result == null)
            {
                return BadRequest("Invalid Username or Password");
            }
            var token = await GenerateToken(result);
            var role = result.Role == 1 ? "ADMINISTRATOR" : "STUDENT";
            var data = new Dictionary<string, object>();
            data.Add("username", result.Username);
            data.Add("role",role);
            data.Add("token", token);
            data.Add("isAuthenticated", result.isAuthenticated);
            return Ok(data);
            
        }

        private async Task<string> GenerateToken(User user)
        {
            var role = "";
            if (user.Role == 1)
            {
                role = "ADMINSTRATOR";
            }
            else
            {
                role = "STUDENT";
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,             
                claims,
                 expires: DateTime.Now.AddHours(1),
                // notBefore:DateTime.UtcNow,
                signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;

        }

        public long CurrentUserId
        {
            get
            {
                var _user = HttpContext.User;

                var _id = 0L;
                long.TryParse(_user.FindFirst(ClaimTypes.PrimarySid).Value, out _id);
                return _id;
            }
        }




    }
}
