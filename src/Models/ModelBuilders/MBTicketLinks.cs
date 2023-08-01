using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketLinks
    {
        public static void TicketLinksEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketLink>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.ReferenceId)
                    .IsRequired();

                entity.Property(e => e.Category)
                   .IsRequired();

                entity.Property(e => e.LinkTypeId)
                  .IsRequired();

                entity.Property(e => e.Link)
                  .IsRequired();

                entity.Property(e => e.LinkText)
                  .IsRequired();

                entity.Property(e => e.Notes)
                  .IsRequired(false)
                  .HasMaxLength(200);

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
                    .WithMany(p => p.TicketLinks)
                    .HasForeignKey(d => d.LinkTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
