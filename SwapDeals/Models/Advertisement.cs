//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SwapDeals.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;

    public partial class Advertisement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Advertisement()
        {
            this.Bookings = new HashSet<Booking>();
        }
    
        public int AdID { get; set; }
        public int ProductID { get; set; }
        public Nullable<int> UserID { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name="Sell Product")]
        public string SellingProduct { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Target Product")]
        public string TargatedProduct { get; set; }
        [Display(Name = "Date")]
        public Nullable<System.DateTime> Date { get; set; }
        [Display(Name = "Add / Pay (+ve Means buyer will add)")]
        public Nullable<int> AdjustedValue { get; set; }
        
        [Display(Name = "Upload image")]
        public string Images { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public string ProductDescription { get; set; }
        public Nullable<int> PriorityStatus { get; set; }
        public Nullable<int> Payment { get; set; }
        [Required]
        public string Warranty { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
