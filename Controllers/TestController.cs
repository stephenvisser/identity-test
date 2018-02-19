using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2CWebApi.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        // GET: api/test
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new string[] { "One", "Two"});
            // var scopes = HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;
            // if (!string.IsNullOrEmpty(identity_test.Startup.ScopeRead) && scopes != null
            //         && scopes.Split(' ').Any(s => s.Equals(Startup.ScopeRead)))
            //     return Ok(new string[] { "value1", "value2" });
            // else
            //     return Unauthorized();
        }

    }
}