#nullable disable
using Users.APP.Features.Users;
using CORE.APP.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

//Generated from Custom Template.
namespace Users.API.Controllers
{
    /// <summary>
    /// Controller for managing users in the application.
    /// Provides endpoints to create, update, delete, and retrieve users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging errors and information.</param>
        /// <param name="mediator">The mediator instance for handling requests.</param>
        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A list of users, or no content if no users are found.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var list = await response.ToListAsync();
                if (list.Any())
                    return Ok(list);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersGet Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during UsersGet."));
            }
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The requested user, or no content if the user is not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                if (item is not null)
                    return Ok(item);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersGetById Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during UsersGetById."));
            }
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="request">The request containing the user data to create.</param>
        /// <returns>The result of the user creation, including success or failure.</returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(UserCreateRequest request)
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
                    ModelState.AddModelError("UsersPost", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersPost Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during UsersPost."));
            }
        }

        /// <summary>
        /// Updates an existing user in the system.
        /// </summary>
        /// <param name="request">The request containing the updated user data.</param>
        /// <returns>The result of the user update, including success or failure.</returns>
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(UserUpdateRequest request)
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
                    ModelState.AddModelError("UsersPut", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersPut Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during UsersPut."));
            }
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>The result of the user deletion, including success or failure.</returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserDeleteRequest() { Id = id });
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                ModelState.AddModelError("UsersDelete", response.Message);
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersDelete Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during UsersDelete."));
            }
        }



        /// <summary>
        /// Authenticates the user and issues a JWT access token upon successful login.
        /// </summary>
        /// <param name="request">The login request containing username and password.</param>
        /// <returns>
        /// - 200 OK with a <see cref="TokenResponse"/> if authentication succeeds.  
        /// - 400 Bad Request with a detailed <see cref="CommandResponse"/> if credentials are invalid or model state is incorrect.
        /// </returns>
        [HttpPost, Route("~/api/[action]")] // Resolves to: POST /api/Token
        [AllowAnonymous]
        public async Task<IActionResult> Token(TokenRequest request)
        {
            // Validate incoming model (username & password) using data annotations
            if (ModelState.IsValid)
            {
                // Use MediatR to send the TokenRequest to its handler
                var response = await _mediator.Send(request);

                // Return token if authentication was successful
                if (response.IsSuccessful)
                    return Ok(response);

                // Add error to model state if user credentials are invalid
                ModelState.AddModelError("UsersToken", response.Message);
            }

            // If we reach this point, model validation or authentication failed, get the error messages from the ModelState
            var errorMessages = string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return BadRequest(new CommandResponse(false, errorMessages));
        }

        /// <summary>
        /// Checks the current authentication status and returns user identity and role information.
        /// </summary>
        /// <remarks>
        /// This endpoint is useful for verifying that a token is valid and extracting identity claims.
        /// </remarks>
        /// <returns>
        /// - 200 OK with a detailed <see cref="CommandResponse"/> if user is authenticated.  
        /// - 400 Bad Request if user is not authenticated or token is invalid.
        /// </returns>
        [HttpGet]
        [Route("~/api/[action]")] // Resolves to: GET /api/Authorize
        [AllowAnonymous]
        public IActionResult Authorize()
        {
            // Check if the request's identity (User) is authenticated
            var isAuthenticated = User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                // Extract username from identity
                var userName = User.Identity.Name;

                // Check if user has the "Admin" role
                var isAdmin = User.IsInRole("Admin");

                // Read custom claims from JWT token
                var role = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var id = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;

                // Construct a friendly message to return to the caller
                var message = $"User authenticated. " +
                              $"User Name: {userName}, " +
                              $"Is Admin?: {(isAdmin ? "Yes" : "No")}, " +
                              $"Role: {role}, " +
                              $"Id: {id}";

                return Ok(new CommandResponse(true, message));
            }

            // Token was not valid or missing — user is unauthenticated
            return BadRequest(new CommandResponse(false, "User not authenticated!"));
        }

        /// <summary>
        /// Handles the refresh token request by validating the input model and forwarding it to the mediator.
        /// If the operation is successful, returns HTTP 200 (OK) with the response.
        /// If validation fails or the operation is unsuccessful, returns HTTP 400 (Bad Request) with error messages.
        /// </summary>
        /// <param name="request">The refresh token request containing necessary token data.</param>
        /// <returns>
        /// IActionResult indicating the result of the operation:
        /// - 200 OK with response if successful
        /// - 400 Bad Request with error messages otherwise
        /// </returns>
        [HttpPost]
        [Route("~/api/[action]")] // Maps to: POST /api/RefreshToken
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            // Check if the model passed in the request is valid according to the data annotations
            if (ModelState.IsValid)
            {
                // Forward the request to the mediator for processing
                var response = await _mediator.Send(request);

                // If the mediator operation succeeded, return an OK response
                if (response.IsSuccessful)
                    return Ok(response);

                // If it failed, register the error message under a specific key
                ModelState.AddModelError("UsersRefreshToken", response.Message);
            }

            // Gather all error messages from the model state
            var errorMessages = string.Join("|", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            // Return a bad request with a concatenated string of all error messages
            return BadRequest(new CommandResponse(false, errorMessages));
        }
    }
}
