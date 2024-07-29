using Common.Interfaces;
using Common.Models.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autheticator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IJwtBearerToken jwtTokenBearerService;

        public AuthController(IJwtBearerToken jwtTokenBearerService)
        {
            this.jwtTokenBearerService = jwtTokenBearerService;
        }

        [HttpGet]
        public ActionResult<JwtToken> GetJwtToken()
        {
            return jwtTokenBearerService.GenerateToken();
        }
    }
}