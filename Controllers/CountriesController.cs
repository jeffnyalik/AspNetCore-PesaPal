using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AngOauth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
       
        [HttpGet]
        //[Authorize]
        public IActionResult Getcountries()
        {
            string[] countries = new string[]
            {
                "country1",
                "country2",
                "country3",
                "country4",
                "country5",
                "country6",
            };

            var res = countries;
            return Ok(res);
        }
    }
}
