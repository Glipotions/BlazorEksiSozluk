using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;

namespace BlazorSozluk.Infrastructure.Persistence.Repositories
{
    public class EntryVoteRepository : GenericRepository<EntryVote>, IEntryVoteRepository
    {
        public EntryVoteRepository(BlazorSozlukContext dbContext) : base(dbContext)
        {
        }
    }
}
