using JWTAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Db.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
    }
}
