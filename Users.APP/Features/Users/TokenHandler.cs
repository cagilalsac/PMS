using CORE.APP.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents a request to generate a JWT token, containing user credentials.
    /// </summary>
    public class TokenRequest : Request, IRequest<TokenResponse>
    {
        /// <summary>
        /// The username of the user requesting the token.
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; }

        /// <summary>
        /// The password of the user requesting the token.
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents the response to a <see cref="TokenRequest"/>, including a JWT token if successful.
    /// </summary>
    public class TokenResponse : CommandResponse
    {
        /// <summary>
        /// The generated JWT token.
        /// </summary>
        public string Token { get; set; }

        // REFRESH TOKEN
        /// <summary>
        /// Gets or sets the refresh token assigned to the user.
        /// This token is used to request a new access token without requiring re-authentication,
        /// typically after the original access token has expired.
        /// </summary>
        public string RefreshToken { get; set; } 

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        /// <param name="isSuccessful">Indicates whether the request was successful.</param>
        /// <param name="message">A message about the result of the operation.</param>
        /// <param name="id">The ID associated with the result (typically the user ID).</param>
        public TokenResponse(bool isSuccessful, string message = "", int id = 0)
            : base(isSuccessful, message, id)
        {
        }
    }

    /// <summary>
    /// Handles <see cref="TokenRequest"/>s by validating credentials and generating a JWT token.
    /// </summary>
    public class TokenHandler : UserService, IRequestHandler<TokenRequest, TokenResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenHandler"/> class.
        /// </summary>
        /// <param name="db">The application's user database context.</param>
        public TokenHandler(UsersDb db) : base(db) // DO NOT FORGET TO CHANGE THE CONSTRUCTOR'S PARAMETER from "DbContext db" to "UsersDb db"!
        {
        }

        /// <summary>
        /// Returns a queryable collection of <see cref="User"/> entities with their associated <see cref="Role"/> included.
        /// Overrides the base method to apply eager loading for the <see cref="User.Role"/> navigation property.
        /// </summary>
        /// <param name="isNoTracking">
        /// If <c>true</c>, disables change tracking for better performance in read-only operations.
        /// If <c>false</c>, enables change tracking to allow updates to the queried entities.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{User}"/> with the <see cref="Role"/> navigation property eagerly loaded.
        /// </returns>
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.Role);
        }

        /// <summary>
        /// Handles the token request by authenticating the user and returning a JWT token.
        /// </summary>
        /// <param name="request">The token request containing username and password.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="TokenResponse"/> indicating the result.</returns>
        public async Task<TokenResponse> Handle(TokenRequest request, CancellationToken cancellationToken)
        {
            // Attempt to retrieve the user with the given credentials
            var user = await Query().SingleOrDefaultAsync(u => u.UserName == request.UserName &&
                                                               u.Password == request.Password &&
                                                               u.IsActive, cancellationToken);

            // If user not found or inactive, return failure response
            if (user is null)
                return new TokenResponse(false, "Active user with the user name and password not found!");

            // REFRESH TOKEN
            // Generate refresh token and save it to the Users table for the retrieved user
            user.RefreshToken = CreateRefreshToken();
            user.RefreshTokenExpiration = DateTime.Now.AddDays(AppSettings.RefreshTokenExpirationInDays);
            await Update(user, cancellationToken);

            // Generate claims and token
            var claims = GetClaims(user);
            var expiration = DateTime.Now.AddMinutes(AppSettings.ExpirationInMinutes);
            var token = CreateAccessToken(claims, expiration);

            // Return the token and refresh token in a successful response
            return new TokenResponse(true, "Token created successfully.", user.Id)
            {
                Token = $"{JwtBearerDefaults.AuthenticationScheme} {token}",

                // REFRESH TOKEN
                RefreshToken = user.RefreshToken 
            };
        }
    }
}
