using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBUserPasswordPolicies
    {
        public static void UserPasswordPoliciesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPasswordPolicy>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CanChangePassword)
                    .IsRequired();

                entity.Property(e => e.ExpirationDate)
                    .IsRequired();

                entity.Property(e => e.IsPasswordNeverExpires)
                    .IsRequired();

                entity.HasOne(d => d.User)
                   .WithMany(p => p.UserPasswordPolicies)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
