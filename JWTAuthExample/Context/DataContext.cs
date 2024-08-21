using JWTAuthExample.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthExample.DataContext
{
    public class DataContext(DbContextOptions options) 
        : IdentityDbContext<User>(options);
}
