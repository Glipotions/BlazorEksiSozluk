using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.Entry;
using BlazorSozluk.Common.Infrastructure;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.Entry.CreateFav
{
    public class CreateEntryFavCommand : IRequest<bool>
    {
        public Guid? EntryId { get; set; }

        public Guid? UserId { get; set; }

        public CreateEntryFavCommand(Guid? entryId, Guid? userId)
        {
            EntryId = entryId;
            UserId = userId;
        }

        public class CreateEntryFavCommandHandler : IRequestHandler<CreateEntryFavCommand, bool>
        {

            public Task<bool> Handle(CreateEntryFavCommand request, CancellationToken cancellationToken)
            {
                QueueFactory.SendMessageToExchange(exchangeName: ProjectConstants.FavExchangeName,
                    exchangeType: ProjectConstants.DefaultExchangeType,
                    queueName: ProjectConstants.CreateEntryFavQueueName,
                    obj: new CreateEntryFavEvent()
                    {
                        EntryId = request.EntryId.Value,
                        CreatedBy = request.UserId.Value
                    });

                return Task.FromResult(true);
            }
        }
    }
}
