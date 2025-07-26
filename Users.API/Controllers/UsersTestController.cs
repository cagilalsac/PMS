using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers
{
    /// <summary>
    /// Controller for testing the availability and basic functionality of the Users API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersTestController : ControllerBase
    {
        /// <summary>
        /// GET endpoint to verify that the Users API is operational.
        /// </summary>
        /// <returns>HTTP 200 OK response with a success message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Users test successful.");
        }
    }
}
