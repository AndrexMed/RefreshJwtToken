using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefreshJwtToken.Models.Custom;
using RefreshJwtToken.Repository;

namespace RefreshJwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authorizationService;

        public UserController(IAuthService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AutorizacionRequest autorizacion)
        {
            var resultOfAuth = await _authorizationService.ReturnToken(autorizacion);

            if (resultOfAuth == null)
            {
                return Unauthorized();
            }
            
            return Ok(resultOfAuth);
        }
    }
}
