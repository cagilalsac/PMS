#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CORE.APP.Features;
using Users.APP.Features.Skills;
using Microsoft.AspNetCore.Authorization;

//Generated from Custom Template.
namespace Users.API.Controllers
{
    /// <summary>
    /// Controller for managing skills in the application.
    /// Provides endpoints to create, update, delete, and retrieve skills.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SkillsController : ControllerBase
    {
        private readonly ILogger<SkillsController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging errors and information.</param>
        /// <param name="mediator">The mediator instance for handling requests.</param>
        public SkillsController(ILogger<SkillsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all skills in the system.
        /// </summary>
        /// <returns>A list of skills, or no content if no skills are found.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new SkillQueryRequest());
                var list = await response.ToListAsync();
                if (list.Any())
                    return Ok(list);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("SkillsGet Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during SkillsGet."));
            }
        }

        /// <summary>
        /// Retrieves a skill by its ID.
        /// </summary>
        /// <param name="id">The ID of the skill to retrieve.</param>
        /// <returns>The requested skill, or no content if the skill is not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new SkillQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                if (item is not null)
                    return Ok(item);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("SkillsGetById Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during SkillsGetById."));
            }
        }

        /// <summary>
        /// Creates a new skill.
        /// </summary>
        /// <param name="request">The request containing the skill data to create.</param>
        /// <returns>The result of the skill creation, including success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(SkillCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    ModelState.AddModelError("SkillsPost", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("SkillsPost Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during SkillsPost."));
            }
        }

        /// <summary>
        /// Updates an existing skill.
        /// </summary>
        /// <param name="request">The request containing the updated skill data.</param>
        /// <returns>The result of the skill update, including success or failure.</returns>
        [HttpPut]
        public async Task<IActionResult> Put(SkillUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    ModelState.AddModelError("SkillsPut", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("SkillsPut Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during SkillsPut."));
            }
        }

        /// <summary>
        /// Deletes a skill by its ID.
        /// </summary>
        /// <param name="id">The ID of the skill to delete.</param>
        /// <returns>The result of the skill deletion, including success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new SkillDeleteRequest() { Id = id });
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                ModelState.AddModelError("SkillsDelete", response.Message);
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("SkillsDelete Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during SkillsDelete."));
            }
        }
    }
}
