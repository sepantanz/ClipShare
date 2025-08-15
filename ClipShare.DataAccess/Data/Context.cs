using ClipShare.Core.Entities;
using ClipShare.DataAccess.Data.Configs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Data
{
    public class Context : IdentityDbContext<AppUser, AppRole, int>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Shorter way of applying manual configurations
            //$ builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Longer way of applying manual configurations
            builder.ApplyConfiguration(new CommentConfig());
            builder.ApplyConfiguration(new SubscribeConfig());
            builder.ApplyConfiguration(new LikeDislikeConfig());
        }
    }
}
