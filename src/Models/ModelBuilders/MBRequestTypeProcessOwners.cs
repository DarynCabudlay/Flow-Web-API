using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestTypeProcessOwners
    {
        public static void RequestTypeProcessOwnersEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestTypeProcessOwner>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.RequestTypeId, e.Version, e.Owner }, "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.RequestTypeId)
                   .IsRequired();

                entity.Property(e => e.Version)
                   .IsRequired();

                entity.Property(e => e.Owner)
                  .IsRequired()
                  .HasMaxLength(128);

                entity.HasOne(d => d.RequestType)
                  .WithMany(p => p.RequestTypeProcessOwners)
                  .HasForeignKey(d => new { d.RequestTypeId, d.Version })
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                   .WithMany(p => p.RequestTypeProcessOwners)
                   .HasForeignKey(d => d.Owner);
            });
        }
    }
}
