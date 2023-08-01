using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBHolidays
    {
        public static void HolidaysEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name }, "IX_HolidayName").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Published)
                    .IsRequired();

                entity.Property(e => e.WithFixedSchedule)
                   .IsRequired();

                entity.Property(e => e.FixedScheduleMonth)
                   .IsRequired(false);

                entity.Property(e => e.FixedScheduleDay)
                  .IsRequired(false);

                entity.Property(e => e.CreatedByPK)
                   .IsRequired()
                   .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128)
                    .IsRequired(false);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);
            });
        }
    }
}
