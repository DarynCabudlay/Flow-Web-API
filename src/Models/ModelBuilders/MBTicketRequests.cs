using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketRequests
    {
        public static void TicketRequestsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.TicketId)
                    .IsRequired();

                entity.Property(e => e.Sequence)
                    .IsRequired();

                entity.Property(e => e.Remarks)
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

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketRequests)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
