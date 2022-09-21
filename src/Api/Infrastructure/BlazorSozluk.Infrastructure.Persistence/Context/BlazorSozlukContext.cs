using BlazorSozluk.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorSozluk.Infrastructure.Persistence.Context
{
    public class BlazorSozlukContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";

        public BlazorSozlukContext()
        {

        }
        public BlazorSozlukContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryVote> EntryVotes { get; set; }
        public DbSet<EntryFavorite> EntryFavorites { get; set; }
        public DbSet<EntryComment> EntryComments { get; set; }
        public DbSet<EntryCommentVote> EntryCommentVotes { get; set; }
        public DbSet<EntryCommentFavorite> EntryCommentFavorites { get; set; }
        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = "Data Source=DESKTOP-CED7FJ6;Initial Catalog=BlazorSozluk;User ID=hamza;Password=12345";
                optionsBuilder.UseSqlServer(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // şuan çalıştığımız assembly de Ientity den türeyen classları otomatik olarak ekler
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override int SaveChanges()
        {
            OnBeforeSave();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSave()
        {
            var addedEntities = ChangeTracker.Entries()
                                .Where(x => x.State == EntityState.Added)
                                .Select(x => (BaseEntity)x.Entity);


            PrepareAddedEntities(addedEntities);
        }

        private void PrepareAddedEntities(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.CreateDate == DateTime.MinValue)
                    entity.CreateDate = DateTime.Now;
            }
        }

    }
}
