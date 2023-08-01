using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBAuditTrailDetails
    {
        public static void AuditTrailDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditTrailDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.Property(e => e.DetailId)
                   .UseIdentityColumn();

                entity.Property(e => e.Id)
                    .IsRequired();

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Field)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OldValue)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.Property(e => e.NewValue)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.HasOne(d => d.AuditTrail)
                   .WithMany(p => p.AuditTrailDetails)
                   .HasForeignKey(d => new { d.Id, d.TableName, d.Key, d.Date, d.UserId });

            });
        }
    }
}
