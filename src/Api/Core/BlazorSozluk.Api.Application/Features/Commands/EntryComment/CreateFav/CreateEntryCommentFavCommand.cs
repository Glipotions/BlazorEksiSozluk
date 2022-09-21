using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.EntryComment;
using BlazorSozluk.Common.Infrastructure;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.CreateFav
{
    public class CreateEntryCommentFavCommand : IRequest<bool>
    {

        public Guid EntryCommentId { get; set; }
        public Guid UserId { get; set; }

        public CreateEntryCommentFavCommand(Guid userId, Guid entryCommentId)
        {
            UserId = userId;
            EntryCommentId = entryCommentId;
        }


        public class CreateEntryCommentFavCommandHandler : IRequestHandler<CreateEntryCommentFavCommand, bool>
        {

            public async Task<bool> Handle(CreateEntryCommentFavCommand request, CancellationToken cancellationToken)
            {
                QueueFactory.SendMessageToExchange(exchangeName: ProjectConstants.FavExchangeName,
                    exchangeType: ProjectConstants.DefaultExchangeType,
                    queueName: ProjectConstants.CreateEntryCommentFavQueueName,
                    obj: new CreateEntryCommentFavEvent()
                    {
                        CreatedBy = request.UserId,
                        EntryCommentId = request.EntryCommentId
                    });

                return await Task.FromResult(true);
            }
        }
    }
}
