using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets cho các entity
        public DbSet<server.Core.Entities.User> Users { get; set; }
        public DbSet<server.Core.Entities.Photo> Photos { get; set; }
        public DbSet<server.Core.Entities.UserDevice> UserDevices { get; set; }
        public DbSet<server.Core.Entities.RefreshToken> RefreshTokens { get; set; }
        public DbSet<server.Core.Entities.Friendship> Friendships { get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tự động tìm và áp dụng tất cả các class implement IEntityTypeConfiguration trong Assembly này
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}