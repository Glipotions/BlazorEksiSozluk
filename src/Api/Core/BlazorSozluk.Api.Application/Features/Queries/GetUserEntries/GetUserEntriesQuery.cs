using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetMainPageEntries
{
    public class GetUserEntriesQuery : BasePagedQuery, IRequest<PagedViewModel<GetUserEntriesDetailViewModel>>
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }

        public GetUserEntriesQuery(Guid? userId, string userName = null, int page = 1, int pageSize = 10) : base(page, pageSize)
        {
            UserId = userId;
            UserName = userName;
        }

        public class GetUserEntriesQueryHandler : IRequestHandler<GetUserEntriesQuery, PagedViewModel<GetUserEntriesDetailViewModel>>
        {
            private readonly IEntryRepository entryRepository;

            public GetUserEntriesQueryHandler(IEntryRepository entryRepository)
            {
                this.entryRepository = entryRepository;
            }

            public async Task<PagedViewModel<GetUserEntriesDetailViewModel>> Handle(GetUserEntriesQuery request, CancellationToken cancellationToken)
            {
                var query = entryRepository.AsQueryable();

                if (request.UserId != null && request.UserId.HasValue && request.UserId != Guid.Empty)
                {
                    query = query.Where(i => i.CreatedById == request.UserId);
                }
                else if (!string.IsNullOrEmpty(request.UserName))
                {
                    query = query.Where(i => i.CreatedBy.Username == request.UserName);
                }
                else
                    return null;

                query = query.Include(i => i.EntryFavorites)
                             .Include(i => i.CreatedBy);

                var list = query.Select(i => new GetUserEntriesDetailViewModel()
                {
                    Id = i.Id,
                    Subject = i.Subject,
                    Content = i.Content,
                    IsFavorited = request.UserId.HasValue && i.EntryFavorites.Any(j => j.CreatedById == request.UserId),
                    FavoritedCount = i.EntryFavorites.Count,
                    CreatedDate = i.CreateDate,
                    CreatedByUserName = i.CreatedBy.Username
                });

                var entries = await list.GetPaged(request.Page, request.PageSize);

                return entries;
            }
        }
    }

}
