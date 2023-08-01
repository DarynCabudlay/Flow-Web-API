using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketRequestDetails
    {
        public static void TicketRequestDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketRequestDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.TicketRequestId, e.FieldId }, "IX_TicketRequestAndField").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.TicketRequestId)
                    .IsRequired();

                entity.Property(e => e.FieldId)
                   .IsRequired();

                entity.Property(e => e.IsLov)
                  .IsRequired();

                entity.Property(e => e.ValueCode)
                  .HasMaxLength(8000)
                  .IsRequired(false);

                entity.Property(e => e.Value)
                   .HasMaxLength(8000)
                   .IsRequired();

                entity.Property(e => e.ValueCode2)
                  .HasMaxLength(8000)
                  .IsRequired(false);

                entity.Property(e => e.Value2)
                   .HasMaxLength(8000)
                   .IsRequired(false);

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

                entity.HasOne(d => d.TicketRequest)
                    .WithMany(p => p.TicketRequestDetails)
                    .HasForeignKey(d => d.TicketRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.GeneralField)
                    .WithMany(p => p.TicketRequestDetails)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
