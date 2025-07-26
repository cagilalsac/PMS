using CORE.APP.Features;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents the request for refreshing a JWT access token.
    /// Contains both the current access token and the refresh token.
    /// </summary>
    public class RefreshTokenRequest : Request, IRequest<RefreshTokenResponse>
    {
        /// <summary>
        /// The expired or soon-to-expire JWT access token that needs refreshing.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// The refresh token used to validate the user and issue a new access token.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Represents the response returned after a refresh token request.
    /// Inherits from TokenResponse and may include a new access token and refresh token.
    /// </summary>
    public class RefreshTokenResponse : TokenResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenResponse"/> class.
        /// </summary>
        /// <param name="isSuccessful">Indicates if the token refresh operation was successful.</param>
        /// <param name="message">Optional message providing status or error details.</param>
        /// <param name="id">Optional ID related to the user or entity.</param>
        public RefreshTokenResponse(bool isSuccessful, string message = "", int id = 0)
            : base(isSuccessful, message, id)
        {
        }
    }

    /// <summary>
    /// Handles the logic for processing a refresh token request.
    /// Validates the refresh token and issues a new access token if valid.
    /// </summary>
    public class RefreshTokenHandler : UserService, IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class with the given database context.
        /// </summary>
        /// <param name="db">The UsersDb database context.</param>
        public RefreshTokenHandler(UsersDb db) : base(db)
        {
        }

        /// <summary>
        /// Returns a queryable collection of <see cref="User"/> entities, including related <see cref="Role"/> data.
        /// Overrides the base query to eagerly load the associated role for each user.
        /// </summary>
        /// <param name="isNoTracking">
        /// Indicates whether the query should track changes to the entities.
        /// Set to <c>true</c> for read-only scenarios to improve performance (default is <c>true</c>).
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{User}"/> with the <see cref="Role"/> navigation property included.
        /// </returns>
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(user => user.Role);
        }

        /// <summary>
        /// Handles the refresh token logic: verifies the user and refresh token, then issues a new access token.
        /// </summary>
        /// <param name="request">The refresh token request object containing the token and refresh token.</param>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="RefreshTokenResponse"/> containing the result of the operation.</returns>
        public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            // Extract the user's principal from the expired token
            var principal = GetPrincipal(request.Token);

            // Extract the user ID from claims
            var userId = Convert.ToInt32(principal.Claims.SingleOrDefault(claim => claim.Type == "Id").Value);

            // Find user in DB that matches the ID and has a valid refresh token
            var user = await Query().SingleOrDefaultAsync(user => user.Id == userId &&
                                                                  user.RefreshToken == request.RefreshToken &&
                                                                  user.RefreshTokenExpiration >= DateTime.Now, cancellationToken);

            // If user is not found or token is invalid, return a failed response
            if (user is null)
                return new RefreshTokenResponse(false, "User not found!");

            // Generate new claims and create a new access token
            var claims = GetClaims(user);
            var expiration = DateTime.Now.AddMinutes(AppSettings.ExpirationInMinutes);
            var accessToken = CreateAccessToken(claims, expiration);

            // Generate a new refresh token (for added security)
            user.RefreshToken = CreateRefreshToken();

            // Optional: Enable sliding expiration for refresh tokens
            // user.RefreshTokenExpiration = DateTime.Now.AddDays(AppSettings.RefreshTokenExpirationInDays);

            // Save the updated user state to the database
            await Update(user, cancellationToken);

            // Return a successful response with the new tokens
            return new RefreshTokenResponse(true, "Token created successfully.", user.Id)
            {
                Token = $"{JwtBearerDefaults.AuthenticationScheme} {accessToken}",
                RefreshToken = user.RefreshToken
            };
        }
    }
}
