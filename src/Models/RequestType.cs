using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class RequestType
    {
        public RequestType()
        {
            RequestForms = new HashSet<RequestForm>();
            RequestTypeProcessOwners = new HashSet<RequestTypeProcessOwner>();
            RequestSteps = new HashSet<RequestStep>();
            Tickets = new HashSet<Ticket>();
            RequestTypesGroupingDetails = new HashSet<RequestTypesGroupingDetail>();
        }

        public int Id { get; set; }
        public int RequestCategoryId { get; set; }
        public int Version { get; set; }
        public int Version2 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set;  }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual RequestCategory RequestCategory { get; set; }
        public virtual ICollection<RequestForm> RequestForms { get; set; }
        public virtual ICollection<RequestTypeProcessOwner> RequestTypeProcessOwners { get; set; }
        public virtual LockedRequestType LockedRequestType { get; set; }
        public int? DeletionReasonId { get; set; }
        public string DeletionRemarks { get; set; }
        public virtual ICollection<RequestStep> RequestSteps { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<RequestTypesGroupingDetail> RequestTypesGroupingDetails { get; set; }
    }
}
