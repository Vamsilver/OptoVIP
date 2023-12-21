//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OptoVIP.ADO
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Like = new HashSet<Like>();
        }
    
        public int idProduct { get; set; }
        public string title { get; set; }
        public int idProductCategory { get; set; }
        public decimal minCount { get; set; }
        public int idNotation { get; set; }
        public int idManufacturer { get; set; }
        public string description { get; set; }
        public int idPriceRange { get; set; }
        public Nullable<decimal> approximatePricePerUnit { get; set; }
        public byte[] image { get; set; }
        public string link { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Like { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual Notation Notation { get; set; }
        public virtual PriceRange PriceRange { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
