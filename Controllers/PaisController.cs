using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RefreshJwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [Route("Lista-Paises")]
        public async Task<IActionResult> List()
        {
            var countryList = await Task.FromResult(new List<string> { "Colombia", "Venezuela", "Ecuador"});
            return Ok(countryList);
        }
    }
}
