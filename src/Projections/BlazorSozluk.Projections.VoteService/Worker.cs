using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.Entry;
using BlazorSozluk.Common.Events.EntryComment;
using BlazorSozluk.Common.Infrastructure;

namespace BlazorSozluk.Projections.VoteService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connStr = configuration.GetConnectionString("SqlServer");

            var voteService = new Services.VoteService(connStr);

            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.VoteExchangeName)
                .EnsureQueue(ProjectConstants.CreateEntryVoteQueueName, ProjectConstants.VoteExchangeName)
                .Receive<CreateEntryVoteEvent>(vote =>
                {
                    voteService.CreateEntryVote(vote).GetAwaiter().GetResult();
                    logger.LogInformation("Create Entry Received EntryId: {0}, VoteType: {1}", vote.EntryId, vote.VoteType);
                })
                .StartConsuming(ProjectConstants.CreateEntryVoteQueueName);

            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.VoteExchangeName)
                .EnsureQueue(ProjectConstants.DeleteEntryVoteQueueName, ProjectConstants.VoteExchangeName)
                .Receive<DeleteEntryVoteEvent>(vote =>
                {
                    voteService.DeleteEntryVote(vote.EntryId, vote.CreatedBy).GetAwaiter().GetResult();
                    logger.LogInformation("Delete Entry Received EntryId: {0}", vote.EntryId);
                })
                .StartConsuming(ProjectConstants.DeleteEntryVoteQueueName);


            QueueFactory.CreateBasicConsumer()
                    .EnsureExchange(ProjectConstants.VoteExchangeName)
                    .EnsureQueue(ProjectConstants.CreateEntryCommentVoteQueueName, ProjectConstants.VoteExchangeName)
                    .Receive<CreateEntryCommentVoteEvent>(vote =>
                    {
                        voteService.CreateEntryCommentVote(vote).GetAwaiter().GetResult();
                        logger.LogInformation("Create Entry Comment Received EntryCommentId: {0}, VoteType: {1}", vote.EntryCommentId, vote.VoteType);
                    })
                    .StartConsuming(ProjectConstants.CreateEntryCommentVoteQueueName);

            QueueFactory.CreateBasicConsumer()
                    .EnsureExchange(ProjectConstants.VoteExchangeName)
                    .EnsureQueue(ProjectConstants.DeleteEntryCommentVoteQueueName, ProjectConstants.VoteExchangeName)
                    .Receive<DeleteEntryCommentVoteEvent>(vote =>
                    {
                        voteService.DeleteEntryCommentVote(vote.EntryCommentId, vote.CreatedBy).GetAwaiter().GetResult();
                        logger.LogInformation("Delete Entry Comment Received EntryCommentId: {0}", vote.EntryCommentId);
                    })
                    .StartConsuming(ProjectConstants.DeleteEntryCommentVoteQueueName);
        }
    }

}