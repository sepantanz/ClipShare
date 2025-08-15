using ClipShare.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Data.Configs
{
    public class SubscribeConfig : IEntityTypeConfiguration<Subscribe>
    {
        public void Configure(EntityTypeBuilder<Subscribe> builder)
        {
            // Define the primary key which is a combination of AppUserId and ChannelId
            builder.HasKey(c => new { c.AppUserId, c.ChannelId });

            builder.HasOne(a => a.AppUser).WithMany(s => s.Subscriptions)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Channel).WithMany(c => c.Subscribers).HasForeignKey(c => c.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
