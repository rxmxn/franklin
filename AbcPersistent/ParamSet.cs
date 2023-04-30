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
    
    public partial class ParamSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ParamSet()
        {
            this.GroupSet = new HashSet<GroupSet>();
            this.PackageSet = new HashSet<PackageSet>();
            this.AckRouteSet = new HashSet<AckRouteSet>();
        }
    
        public int Id { get; set; }
        public bool Active { get; set; }
        public Nullable<int> MaxPermitedLimit { get; set; }
        public Nullable<int> PerTurnCapacity { get; set; }
        public Nullable<int> PerWeekCapacity { get; set; }
        public string AutolabAssignedAreaKey { get; set; }
        public string AutolabAssignedAreaName { get; set; }
        public string ParamUniquekey { get; set; }
        public string GenericKeyForStatistic { get; set; }
        public Nullable<int> Analista_Id { get; set; }
        public Nullable<int> Matrix_Id { get; set; }
        public Nullable<int> BaseParam_Id { get; set; }
        public Nullable<int> Unit_Id { get; set; }
        public Nullable<int> CentroCosto_Id { get; set; }
        public Nullable<int> Metodo_Id { get; set; }
        public Nullable<int> Precio_Id { get; set; }
    
        public virtual AnalyticsAreaSet AnalyticsAreaSet { get; set; }
        public virtual AnnalistSet AnnalistSet { get; set; }
        public virtual BaseParamSet BaseParamSet { get; set; }
        public virtual MatrixSet MatrixSet { get; set; }
        public virtual MeasureUnitSet MeasureUnitSet { get; set; }
        public virtual MethodSet MethodSet { get; set; }
        public virtual PriceSet PriceSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupSet> GroupSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PackageSet> PackageSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AckRouteSet> AckRouteSet { get; set; }
    }
}
