using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Update
{
    public class UpdateUserCommandHandle : IRequestHandler<UpdateUserCommand, Guid>
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public UpdateUserCommandHandle(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dbUser = await userRepository.GetByIdAsync(request.Id);

            var dbEmailAddress = dbUser.EmailAddress;

            var emailChanged = string.CompareOrdinal(dbEmailAddress, request.EmailAddress) != 0;

            if (dbUser is null)
            {
                throw new DatabaseValidationException("User not found");
            }
            mapper.Map(request, dbUser);

            var rows = await userRepository.UpdateAsync(dbUser);

            if (emailChanged && rows > 0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAddress = dbEmailAddress,
                    NewEmailAddress = request.EmailAddress
                };

                QueueFactory.SendMessageToExchange(exchangeName: ProjectConstants.UserExchangeName,
                    exchangeType: ProjectConstants.DefaultExchangeType,
                    queueName: ProjectConstants.UserEmailChangedQueueName,
                    obj: @event);

                dbUser.EmailConfirmed = false;
                await userRepository.UpdateAsync(dbUser);
            }

            return dbUser.Id;
        }
    }
}
