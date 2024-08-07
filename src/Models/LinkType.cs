﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class LinkType
    {
        public LinkType()
        {
            TicketLinks = new HashSet<TicketLink>();
            UserNotAllowedLinkTypes = new HashSet<UserNotAllowedLinkType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ICollection<TicketLink> TicketLinks { get; set; }
        public virtual ICollection<UserNotAllowedLinkType> UserNotAllowedLinkTypes { get; set; }
    }
}
