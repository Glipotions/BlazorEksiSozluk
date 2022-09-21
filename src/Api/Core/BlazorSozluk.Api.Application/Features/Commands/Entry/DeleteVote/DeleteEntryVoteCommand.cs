using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.Entry;
using BlazorSozluk.Common.Infrastructure;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.Entry.DeleteVote
{
    public class DeleteEntryVoteCommand : IRequest<bool>
    {
        public Guid EntryId { get; set; }

        public Guid UserId { get; set; }

        public DeleteEntryVoteCommand(Guid entryId, Guid userId)
        {
            EntryId = entryId;
            UserId = userId;
        }

        public class DeleteEntryVoteCommandHandler : IRequestHandler<DeleteEntryVoteCommand, bool>
        {
            public async Task<bool> Handle(DeleteEntryVoteCommand request, CancellationToken cancellationToken)
            {
                QueueFactory.SendMessageToExchange(
                    exchangeName: ProjectConstants.VoteExchangeName,
                    exchangeType: ProjectConstants.DefaultExchangeType,
                    queueName: ProjectConstants.DeleteEntryVoteQueueName,
                    obj: new DeleteEntryVoteEvent
                    {
                        CreatedBy = request.UserId,
                        EntryId = request.EntryId,
                    });

                return await Task.FromResult(true);
            }
        }
    }
}
