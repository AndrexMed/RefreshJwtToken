using Microsoft.AspNetCore.Mvc;
using RefreshJwtToken.Models.Custom;
using RefreshJwtToken.Repository;
using System.IdentityModel.Tokens.Jwt;

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


        [HttpPost]
        [Route("Get-Refresh-Token")]
        public async Task<IActionResult> Authenticate([FromBody] RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExp = tokenHandler.ReadJwtToken(request.TokenExp);

            if (tokenExp.ValidTo > DateTime.UtcNow)
            {
                return BadRequest(new AutorizacionResponse { Resultado = false, Mensaje = "Token not expired" });
            }

            string idUser = tokenExp
                    .Claims
                    .First(p => p.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

            var authorizationResponse = await _authorizationService
                .ReturnRefreshToken(request, int.Parse(idUser));

            if (authorizationResponse.Resultado)
            {
                return Ok(authorizationResponse);
            }
            else
            {
                return BadRequest(authorizationResponse);
            }
        }

    }
}
