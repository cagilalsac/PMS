using Microsoft.AspNetCore.Mvc;

namespace Projects.API.Controllers
{
    /// <summary>
    /// Controller for testing the availability and basic functionality of the Projects API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsTestController : ControllerBase
    {
        /// <summary>
        /// GET endpoint to verify that the Projects API is operational.
        /// </summary>
        /// <returns>HTTP 200 OK response with a success message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Projects test successful.");
        }
    }
}
