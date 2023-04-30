//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AbcPersistent
{
    using System;
    using System.Collections.Generic;
    
    public partial class BaseParamSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaseParamSet()
        {
            this.ParamSet = new HashSet<ParamSet>();
            this.MeasureUnitSet = new HashSet<MeasureUnitSet>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool PrintInResultReport { get; set; }
        public Nullable<int> ParamFamily_Id { get; set; }
    
        public virtual BaseParamFamilySet BaseParamFamilySet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParamSet> ParamSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeasureUnitSet> MeasureUnitSet { get; set; }
    }
}
