using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JewelleryStore.Common;
using JewelleryStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JewelleryStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route(RouteConstants.Authenticate)]
        public IActionResult Authenticate(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = false;
                var loginResponse = new LoginResponseModel();
                
                var user = _context.Users.FirstOrDefault(x => string.Equals(x.Username, login.Username, StringComparison.OrdinalIgnoreCase));

                if(user != null)
                {
                    isValidUser = string.Equals(user.Password, login.Password);
                }

                if (isValidUser)
                {
                    string access_token = HelperClass.GetAccessToken(user);
                    loginResponse.IsAuthorized = true;
                    loginResponse.AccessToken = access_token;
                    return Ok(loginResponse);
                }
                else
                {
                    loginResponse.IsAuthorized = false;
                    loginResponse.ErrorMessage = ErrorMessageConstants.InvalidUser;
                    return Unauthorized(loginResponse);
                }
            }

            return BadRequest();
        }
    }
}
