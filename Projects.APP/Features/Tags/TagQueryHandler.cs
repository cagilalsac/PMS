using Projects.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Projects.APP.Services;

namespace Projects.APP.Features.Tags
{
    /// <summary>
    /// Represents a request to query tags, inheriting from <see cref="Request"/>.
    /// </summary>
    public class TagQueryRequest : Request, IRequest<IQueryable<TagQueryResponse>>
    {
    }

    /// <summary>
    /// Represents a response to a tag query, inheriting from <see cref="QueryResponse"/>.
    /// </summary>
    public class TagQueryResponse : QueryResponse
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the tag query request, inheriting from <see cref="ProjectsDbService"/>.
    /// </summary>
    public class TagQueryHandler : ProjectsDbService, IRequestHandler<TagQueryRequest, IQueryable<TagQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagQueryHandler"/> class with the specified database context.
        /// </summary>
        /// <param name="db">The database context to use.</param>
        public TagQueryHandler(ProjectsDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles the tag query request and returns the queryable result.
        /// </summary>
        /// <param name="request">The tag query request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the queryable result of tag query responses.</returns>
        public Task<IQueryable<TagQueryResponse>> Handle(TagQueryRequest request, CancellationToken cancellationToken)
        {
            // SQL: select Id, Name from Tags order by Name
            return Task.FromResult(_db.Tags.OrderBy(tag => tag.Name).Select(tag => new TagQueryResponse()
            {
                Id = tag.Id,
                Name = tag.Name
            }));
        }
    }
}
