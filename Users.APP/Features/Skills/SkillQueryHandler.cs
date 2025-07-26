using CORE.APP.Features;
using CORE.APP.Services;
using MediatR;
using Users.APP.Domain;

namespace Users.APP.Features.Skills
{
    /// <summary>
    /// Represents a request to query the list of skills.
    /// </summary>
    public class SkillQueryRequest : Request, IRequest<IQueryable<SkillQueryResponse>>
    {
        // No additional properties are required for this request.
    }

    /// <summary>
    /// Represents the response structure for querying skills.
    /// </summary>
    public class SkillQueryResponse : QueryResponse
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the querying of skills from the system.
    /// </summary>
    public class SkillQueryHandler : Service<Skill>, IRequestHandler<SkillQueryRequest, IQueryable<SkillQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillQueryHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for querying skills.</param>
        public SkillQueryHandler(UsersDb db) : base(db)
        {
        }

        /// <summary>
        /// Overrides the base Query method to retrieve Skill entities, ordered by name.
        /// </summary>
        /// <param name="isNoTracking">
        /// A boolean flag indicating whether the query should disable change tracking (default is true).
        /// When set to true, Entity Framework does not track the returned entities, improving performance for read-only operations.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{Skill}"/> containing Skill entities sorted in ascending order by their Name property.
        /// The query also respects the tracking behavior as specified by the <paramref name="isNoTracking"/> parameter.
        /// </returns>
        protected override IQueryable<Skill> Query(bool isNoTracking = true)
        {
            // Retrieves the base query for Skill entities with optional tracking,
            // and orders the result by the Name property alphabetically.
            return base.Query(isNoTracking).OrderBy(s => s.Name);
        }


        /// <summary>
        /// Handles the request to retrieve the list of skills.
        /// </summary>
        /// <param name="request">The request to retrieve skills.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A queryable collection of <see cref="SkillQueryResponse"/> representing the skills.</returns>
        public Task<IQueryable<SkillQueryResponse>> Handle(SkillQueryRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the list of skills ordered by name and project them into the response model
            var query = Query().Select(s => new SkillQueryResponse()
            {
                Id = s.Id,
                Name = s.Name
            });

            return Task.FromResult(query);
        }
    }
}
