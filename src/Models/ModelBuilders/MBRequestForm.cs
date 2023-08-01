using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestForm
    {
        public static void RequestFormEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestForm>(entity =>
            {
                entity.HasKey(e => new { e.RequestTypeId, e.Version, e.FieldId });

                entity.Property(e => e.RequestTypeId)
                    .IsRequired();

                entity.Property(e => e.FieldId)
                    .IsRequired();

                entity.Property(e => e.Version)
                 .IsRequired();

                entity.Property(e => e.Published)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.IsRequired)
                    .IsRequired();

                entity.Property(e => e.Sequence)
                    .IsRequired();

                entity.Property(e => e.DefaultValue)
                   .IsRequired(false);

                entity.Property(e => e.DefaultValue2)
                   .IsRequired(false);

                entity.Property(e => e.Editable)
                   .IsRequired(false)
                   .HasMaxLength(3);

                entity.HasOne(d => d.GeneralField)
                   .WithMany(p => p.RequestForms)
                   .HasForeignKey(d => d.FieldId)
                   .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RequestType)
                   .WithMany(p => p.RequestForms)
                   .HasForeignKey(d => new { d.RequestTypeId, d.Version })
                   .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
