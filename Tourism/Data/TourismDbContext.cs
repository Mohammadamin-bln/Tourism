using Microsoft.EntityFrameworkCore;
using Tourism.Entitiy.Dto;

namespace Tourism.Data
{
    public class TourismDbContext : DbContext
    {
        public TourismDbContext(DbContextOptions<TourismDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

    }
}
