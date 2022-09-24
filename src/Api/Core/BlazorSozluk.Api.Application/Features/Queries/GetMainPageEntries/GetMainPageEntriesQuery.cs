using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetMainPageEntries
{
    public class GetMainPageEntriesQuery : BasePagedQuery, IRequest<PagedViewModel<GetEntryDetailViewModel>>
    {
        public GetMainPageEntriesQuery(Guid? userId, int page, int pageSize) : base(page, pageSize)
        {
            UserId = userId;
        }

        public Guid? UserId { get; set; }


        public class GetMainPageEntriesQueryHandler : IRequestHandler<GetMainPageEntriesQuery, PagedViewModel<GetEntryDetailViewModel>>
        {
            private readonly IEntryRepository entryRepository;

            public GetMainPageEntriesQueryHandler(IEntryRepository entryRepository)
            {
                this.entryRepository = entryRepository;
            }

            public async Task<PagedViewModel<GetEntryDetailViewModel>> Handle(GetMainPageEntriesQuery request, CancellationToken cancellationToken)
            {
                var query = entryRepository.AsQueryable();

                query = query.Include(i => i.EntryFavorites)
                             .Include(i => i.CreatedBy)
                             .Include(i => i.EntryVotes);

                var list = query.Select(i => new GetEntryDetailViewModel()
                {
                    Id = i.Id,
                    Subject = i.Subject,
                    Content = i.Content,
                    IsFavorited = request.UserId.HasValue && i.EntryFavorites.Any(j => j.CreatedById == request.UserId),
                    FavoritedCount = i.EntryFavorites.Count,
                    CreatedDate = i.CreateDate,
                    CreatedByUserName = i.CreatedBy.Username,
                    VoteType =
                        request.UserId.HasValue && i.EntryVotes.Any(j => j.CreatedById == request.UserId)
                        ? i.EntryVotes.FirstOrDefault(j => j.CreatedById == request.UserId).VoteType
                        : VoteType.None // Dışardan userId gönderilmemişse VoteType gözükme durumu ayarlandı
                });

                var entries = await list.GetPaged(request.Page, request.PageSize);

                return entries;
            }
        }
    }
}
