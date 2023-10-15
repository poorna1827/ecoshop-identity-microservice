using IdentityMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityMicroservice.DbContexts
{

        public class IdentityDbContext : DbContext
        {
            public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
            {


            }


            public DbSet<User> Users { get; set; }

            public DbSet<Admin> Admins { get; set; }
        }

}
