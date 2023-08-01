using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTickets
    {
        public static void TicketsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.TicketNo }, "IX_TicketNo").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.TicketNo)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.RushTicketNo)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.RequestTypeId)
                   .IsRequired();

                entity.Property(e => e.Status)
                  .IsRequired();

                entity.Property(e => e.StatusDate)
                  .HasColumnType("datetime")
                  .IsRequired();

                entity.Property(e => e.DetailedStatus)
                  .IsRequired(false);

                entity.Property(e => e.CancellationReason)
                  .IsRequired(false);

                entity.Property(e => e.CancellationRemarks)
                 .HasMaxLength(8000)
                 .IsRequired(false);

                entity.Property(e => e.UserOrganizationalStructureId)
                    .IsRequired();

                entity.Property(e => e.UserOrganizationalStructureName)
                    .HasMaxLength(100)
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

                entity.Property(e => e.DateSubmitted)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(d => d.RequestType)
                  .WithMany(p => p.Tickets)
                  .HasForeignKey(d => new { d.RequestTypeId, d.Version })
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UserOrganizationalStructure)
                 .WithMany(p => p.Tickets)
                 .HasForeignKey(d => d.UserOrganizationalStructureId)
                 .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
