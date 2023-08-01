using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBHolidayAffectedOffices
    {
        public static void HolidayAffectedOfficesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HolidayAffectedOffice>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.HolidayDateId)
                    .IsRequired();

                entity.Property(e => e.OfficeId)
                   .IsRequired();

                entity.Property(e => e.CreatedByPK)
                   .IsRequired()
                   .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.HasOne(d => d.HolidayDate)
                   .WithMany(p => p.HolidayAffectedOffices)
                   .HasForeignKey(d => d.HolidayDateId);

                entity.HasOne(d => d.Office)
                   .WithMany(p => p.HolidayAffectedOffices)
                   .HasForeignKey(d => d.OfficeId)
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
