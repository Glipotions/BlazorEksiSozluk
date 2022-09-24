using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetMainPageEntries
{
    public class GetUserDetailQuery : IRequest<UserDetailViewModel>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public GetUserDetailQuery(Guid userId, string userName = null)
        {
            UserId = userId;
            UserName = userName;
        }

        public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailViewModel>
        {
            private readonly IUserRepository userRepository;
            private readonly IMapper mapper;

            public GetUserDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
            {
                this.userRepository = userRepository;
                this.mapper = mapper;
            }

            public async Task<UserDetailViewModel> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
            {
                Domain.Models.User dbUser = null;

                if (request.UserId != Guid.Empty)
                    dbUser = await userRepository.GetByIdAsync(request.UserId);
                else if (!string.IsNullOrEmpty(request.UserName))
                    dbUser = await userRepository.GetSingleAsync(i => i.Username == request.UserName);

                return mapper.Map<UserDetailViewModel>(dbUser);
            }
        }
    }
}
