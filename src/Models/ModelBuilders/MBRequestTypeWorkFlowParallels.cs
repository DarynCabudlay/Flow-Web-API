using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestTypeWorkFlowParallels
    {
        public static void RequestTypeWorkFlowParallelsEntity(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RequestTypeWorkFlowParallel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.ParallelNo)
                    .IsRequired();

                entity.Property(e => e.RequestTypeWorkFlowId)
                   .IsRequired();

                entity.HasOne(d => d.WorkFlow)
                    .WithOne(p => p.WorkFlowParallel)
                    .HasForeignKey<RequestTypeWorkFlowParallel>(d => d.RequestTypeWorkFlowId);

            });
        }
    }
}
