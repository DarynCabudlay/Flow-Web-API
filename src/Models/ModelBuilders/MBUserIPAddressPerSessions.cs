using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBUserIPAddressPerSessions
    {
        public static void UserIPAddressPerSessionsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserIPAddressPerSession>(entity =>
            {
                entity.HasKey(e => e.SpId);
                entity.Property(e => e.SpId)
                      .ValueGeneratedNever();

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                   .IsRequired()
                   .HasMaxLength(128);
            });
        }
    }
}
