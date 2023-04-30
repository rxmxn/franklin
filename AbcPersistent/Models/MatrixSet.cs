//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AbcPersistent.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatrixSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MatrixSet()
        {
            this.MatrixChangesSet = new HashSet<MatrixChangesSet>();
            this.ParameterSet = new HashSet<ParameterSet>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Active { get; set; }
        public System.DateTime MatrixCreateDate { get; set; }
        public int UserId { get; set; }
        public Nullable<int> Office_Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MatrixChangesSet> MatrixChangesSet { get; set; }
        public virtual OfficeSet OfficeSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterSet> ParameterSet { get; set; }
    }
}
