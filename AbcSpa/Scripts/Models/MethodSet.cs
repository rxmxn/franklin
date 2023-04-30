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
    
    public partial class MethodSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MethodSet()
        {
            this.ParameterSet = new HashSet<ParameterSet>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Active { get; set; }
        public double RequiredVolume { get; set; }
        public double MinimumVolume { get; set; }
        public string Formula { get; set; }
        public Nullable<bool> InternetPublish { get; set; }
        public int DeliverTime { get; set; }
        public double DetectionLimit_Value { get; set; }
        public int DetectionLimit_Decimals { get; set; }
        public double CuantificationLimit_Value { get; set; }
        public int CuantificationLimit_Decimals { get; set; }
        public double Uncertainty_Value { get; set; }
        public int Uncertainty_Decimals { get; set; }
        public int MaxTimeBeforeAnalysis { get; set; }
        public int LabDeliverTime { get; set; }
        public bool QcObj_HasQc { get; set; }
        public int QcObj_UpperLimit { get; set; }
        public int QcObj_LowerLimit { get; set; }
        public int ReportTime { get; set; }
        public int AnalysisTime { get; set; }
        public Nullable<int> AnalyticsMethod_Id { get; set; }
        public Nullable<int> Container_Id { get; set; }
        public Nullable<int> Preserver_Id { get; set; }
        public Nullable<int> Residue_Id { get; set; }
    
        public virtual AnalyticsMethodSet AnalyticsMethodSet { get; set; }
        public virtual ContainerSet ContainerSet { get; set; }
        public virtual PreserverSet PreserverSet { get; set; }
        public virtual ResidueSet ResidueSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterSet> ParameterSet { get; set; }
    }
}