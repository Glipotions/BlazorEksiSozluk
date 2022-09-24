using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetEntryDetail
{
    public class GetEntryDetailQuery : IRequest<GetEntryDetailViewModel>
    {
        public Guid EntryId { get; set; }
        public Guid? UserId { get; set; }

        public GetEntryDetailQuery(Guid entryId, Guid? userId)
        {
            EntryId = entryId;
            UserId = userId;
        }

        public class GetEntryDetailQueryHandler : IRequestHandler<GetEntryDetailQuery, GetEntryDetailViewModel>
        {
            private readonly IEntryRepository entryRepository;

            public GetEntryDetailQueryHandler(IEntryRepository entryRepository)
            {
                this.entryRepository = entryRepository;
            }

            public async Task<GetEntryDetailViewModel> Handle(GetEntryDetailQuery request, CancellationToken cancellationToken)
            {
                var query = entryRepository.AsQueryable();

                query = query.Include(i => i.EntryFavorites)
                            .Include(i => i.CreatedBy)
                            .Include(i => i.EntryVotes)
                            .Where(i => i.Id == request.EntryId);

                var list = query.Select(x => new GetEntryDetailViewModel
                {
                    Id = x.Id,
                    Subject = x.Subject,
                    Content = x.Content,
                    IsFavorited = request.UserId.HasValue && x.EntryFavorites.Any(y => y.CreatedById == request.UserId),
                    FavoritedCount = x.EntryFavorites.Count,
                    CreatedDate = x.CreateDate,
                    CreatedByUserName = x.CreatedBy.Username,
                    VoteType =
                        request.UserId.HasValue && x.EntryVotes.Any(y => y.CreatedById == request.UserId)
                        ? x.EntryVotes.FirstOrDefault(y => y.CreatedById == request.UserId).VoteType
                        : Common.ViewModels.VoteType.None
                });

                return await list.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
