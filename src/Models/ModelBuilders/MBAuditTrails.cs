using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBAuditTrails
    {
        public static void AuditTrailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditTrail>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.TableName, e.Key, e.Date, e.UserId });

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Key2)
                    .IsRequired(false)
                    .HasMaxLength(200);

                entity.Property(e => e.Key3)
                    .IsRequired(false)
                    .HasMaxLength(200);

                entity.Property(e => e.Transaction)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IP)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Device)
                   .IsRequired()
                   .HasMaxLength(100);

                entity.Property(e => e.UsedApp)
                   .IsRequired()
                   .HasMaxLength(100);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.ReasonOfDeletion)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.Property(e => e.DeletionRemarks)
                   .IsRequired(false)
                   .HasMaxLength(8000);

            });
        }
    }
}
