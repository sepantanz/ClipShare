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
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // Define the primary key which is a combination of AppUserId and VideoId
            builder.HasKey(c => new { c.AppUserId, c.VideoId });

            builder.HasOne(a => a.AppUser).WithMany(c => c.Comments)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Video).WithMany(c => c.Comments).HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
