using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;

namespace BlazorSozluk.Infrastructure.Persistence.Repositories
{
    public class EntryCommentVoteRepository : GenericRepository<EntryCommentVote>, IEntryCommentVoteRepository
    {
        public EntryCommentVoteRepository(BlazorSozlukContext dbContext) : base(dbContext)
        {
        }
    }
}
