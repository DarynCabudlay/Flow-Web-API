using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketAttachments
    {
        public static void TicketAttachmentsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.ReferenceId)
                    .IsRequired();

                entity.Property(e => e.Category)
                   .IsRequired();

                entity.Property(e => e.FileTypeId)
                   .IsRequired();

                entity.Property(e => e.Notes)
                   .IsRequired(false)
                   .HasMaxLength(200);

                entity.Property(e => e.OriginalFileName)
                   .HasMaxLength(100)
                   .IsRequired();

                entity.Property(e => e.MaskedFileName)
                   .HasMaxLength(100)
                   .IsRequired(false);

                entity.Property(e => e.FilePath)
                   .HasMaxLength(5000)
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

                entity.HasOne(d => d.FileType)
                   .WithMany(p => p.TicketAttachments)
                   .HasForeignKey(d => d.FileTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
