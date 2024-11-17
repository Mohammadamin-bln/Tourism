using Microsoft.EntityFrameworkCore;
using Tourism.Entitiy;


namespace Tourism.Data
{
    public class TourismDbContext : DbContext
    {
        public TourismDbContext(DbContextOptions<TourismDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserArticle> Articles { get; set; }

    }

 
}
