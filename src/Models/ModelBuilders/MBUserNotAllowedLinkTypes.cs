using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBUserNotAllowedLinkTypes
    {
        public static void UserNotAllowedLinkTypesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserNotAllowedLinkType>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.LinkTypeId, e.Category }, "IX_UserIdLinkTypeCategory").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.UserId)
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(e => e.LinkTypeId)
                   .IsRequired();

                entity.Property(e => e.Category)
                  .IsRequired();

                entity.Property(e => e.Published)
                  .IsRequired();

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .IsRequired(false)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(d => d.LinkType)
                    .WithMany(p => p.UserNotAllowedLinkTypes)
                    .HasForeignKey(d => d.LinkTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotAllowedLinkTypes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
