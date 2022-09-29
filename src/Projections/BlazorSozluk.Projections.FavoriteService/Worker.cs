using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.Entry;
using BlazorSozluk.Common.Events.EntryComment;
using BlazorSozluk.Common.Infrastructure;
//using FluentAssertions.Common;

namespace BlazorSozluk.Projections.FavoriteService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connStr = configuration.GetConnectionString("SqlServer");

            var favService = new Services.FavoriteService(connStr);

            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.FavExchangeName)
                .EnsureQueue(ProjectConstants.CreateEntryFavQueueName, ProjectConstants.FavExchangeName)
                .Receive<CreateEntryFavEvent>(fav =>
                {
                    favService.CreateEntryFav(fav).GetAwaiter().GetResult();
                    _logger.LogInformation($"Received EntryId {fav.EntryId}");
                })
                .StartConsuming(ProjectConstants.CreateEntryFavQueueName);

            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.FavExchangeName)
                .EnsureQueue(ProjectConstants.DeleteEntryFavQueueName, ProjectConstants.FavExchangeName)
                .Receive<DeleteEntryFavEvent>(fav =>
                {
                    favService.DeleteEntryFav(fav).GetAwaiter().GetResult();
                    _logger.LogInformation($"Deleted Received EntryId {fav.EntryId}");
                })
                .StartConsuming(ProjectConstants.DeleteEntryFavQueueName);



            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.FavExchangeName)
                .EnsureQueue(ProjectConstants.CreateEntryCommentFavQueueName, ProjectConstants.FavExchangeName)
                .Receive<CreateEntryCommentFavEvent>(fav =>
                {
                    favService.CreateEntryCommentFav(fav).GetAwaiter().GetResult();
                    _logger.LogInformation($"Create EntryComment Received EntryCommentId {fav.EntryCommentId}");
                })
                .StartConsuming(ProjectConstants.CreateEntryCommentFavQueueName);


            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(ProjectConstants.FavExchangeName)
                .EnsureQueue(ProjectConstants.DeleteEntryCommentFavQueueName, ProjectConstants.FavExchangeName)
                .Receive<DeleteEntryCommentFavEvent>(fav =>
                {
                    favService.DeleteEntryCommentFav(fav).GetAwaiter().GetResult();
                    _logger.LogInformation($"Deleted Received EntryCommentId {fav.EntryCommentId}");
                })
                .StartConsuming(ProjectConstants.DeleteEntryCommentFavQueueName);
        }

    }
}