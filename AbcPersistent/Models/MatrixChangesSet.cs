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
    
    public partial class MatrixChangesSet
    {
        public int Id { get; set; }
        public System.DateTime ChangeDate { get; set; }
        public string LastMatrixName { get; set; }
        public string LastMatrixDescription { get; set; }
        public Nullable<bool> LastActiveStatus { get; set; }
        public Nullable<int> Matrix_Id { get; set; }
        public Nullable<int> User_Id { get; set; }
    
        public virtual MatrixSet MatrixSet { get; set; }
        public virtual UserSet UserSet { get; set; }
    }
}
