using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.Entry;
using BlazorSozluk.Common.Infrastructure;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.Entry.DeleteFav
{
    public class DeleteEntryFavCommand : IRequest<bool>
    {
        public Guid EntryId { get; set; }

        public Guid UserId { get; set; }

        public DeleteEntryFavCommand(Guid entryId, Guid userId)
        {
            EntryId = entryId;
            UserId = userId;
        }

        public class DeleteEntryFavCommandHandler : IRequestHandler<DeleteEntryFavCommand, bool>
        {
            public async Task<bool> Handle(DeleteEntryFavCommand request, CancellationToken cancellationToken)
            {
                QueueFactory.SendMessageToExchange(
                    exchangeName: ProjectConstants.FavExchangeName,
                    exchangeType: ProjectConstants.DefaultExchangeType,
                    queueName: ProjectConstants.DeleteEntryFavQueueName,
                    obj: new DeleteEntryFavEvent
                    { CreatedBy = request.UserId, EntryId = request.EntryId }
                    );

                return await Task.FromResult(true);
            }
        }
    }
}
