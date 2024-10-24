using Microsoft.EntityFrameworkCore;
using JWTAuth.Entities;

namespace JWTAuth.Db.DataContext
{
    public partial class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
    }

}
