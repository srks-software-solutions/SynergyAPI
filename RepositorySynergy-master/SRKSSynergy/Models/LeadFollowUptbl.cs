using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("LeadFollowUptbl")]
    public class LeadFollowUptbl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LFID { get; set; }

        public int LERID { get; set; }

        public Nullable<System.DateTime> FollowUpDate { get; set; }

        public string FollowUp { get; set; }

        public string FollowUpType { get; set; }

        public string FollowUpDescription { get; set; }

        public int CPID { get; set; }

        public int IsStatus { get; set; }

        public int IsDeleted { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public int CreatedBy { get; set; }
        public string TimeLine { get; set; }


        //public virtual ChannelPartners ChannelPartners { get; set; }

        public virtual LeadEnquiryRevised LeadEnquiryRevised { get; set; }

    } 

}