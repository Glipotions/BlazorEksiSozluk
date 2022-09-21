namespace BlazorSozluk.Api.Domain.Models
{
    public class EntryFavorite : BaseEntity
    {
        public Guid EntryId { get; set; }

        public Guid CreatedById { get; set; }

        public Entry Entry { get; set; }
        public virtual User CreatedUser { get; set; }
    }
}
