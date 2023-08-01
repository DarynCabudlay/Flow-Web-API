using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBLockedRequestTypes
    {
        public static void LockedRequestTypesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LockedRequestType>(entity =>
            {
                entity.HasKey(e => new { e.RequestTypeId, e.Version } );

                entity.Property(e => e.RequestTypeId)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired();

                entity.Property(e => e.User)
                    .IsRequired();

                entity.Property(e => e.DateLocked)
                    .IsRequired()
                    .HasDefaultValue(DateTime.Now);

                entity.HasOne(d => d.UserInfo)
                  .WithMany(p => p.LockedRequestTypes)
                  .HasForeignKey(d => d.User);

                entity.HasOne(d => d.RequestType)
                   .WithOne(p => p.LockedRequestType)
                   .HasForeignKey<LockedRequestType>(d => new { d.RequestTypeId, d.Version })
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
