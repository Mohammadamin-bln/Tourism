using Microsoft.EntityFrameworkCore;
using Tourism.Core.Entitiy;

namespace Tourism.Infrastructure.Data
{
    public class TourismDbContext : DbContext
    {
        public TourismDbContext(DbContextOptions<TourismDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserArticle> Articles { get; set; }

        public DbSet<UserTicket> Tickets { get; set; }

    }


}
