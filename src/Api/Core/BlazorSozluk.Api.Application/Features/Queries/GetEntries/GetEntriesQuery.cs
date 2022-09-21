using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetEntries
{
    public class GetEntriesQuery : IRequest<List<GetEntriesViewModel>>
    {
        public bool TodayEntries { get; set; }
        public int Count { get; set; }

        public class GetEntriesQueryHandler : IRequestHandler<GetEntriesQuery, List<GetEntriesViewModel>>
        {
            private readonly IEntryRepository entryRepository;
            private readonly IMapper mapper;

            public GetEntriesQueryHandler(IEntryRepository entryRepository, IMapper mapper)
            {
                this.entryRepository = entryRepository;
                this.mapper = mapper;
            }

            public async Task<List<GetEntriesViewModel>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
            {
                var query = entryRepository.AsQueryable();

                if (request.TodayEntries)
                {
                    query = query
                        .Where(x => x.CreateDate >= DateTime.Now.Date)
                        .Where(x => x.CreateDate <= DateTime.Now.AddDays(1).Date);

                }

                query.Include(x => x.EntryComments)
                        .OrderBy(x => Guid.NewGuid())
                        .Take(request.Count); // rastgele count kadar döndürür

                return await query.ProjectTo<GetEntriesViewModel>(mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);
            }
        }
    }


}
