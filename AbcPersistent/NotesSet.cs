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
    
    public partial class NotesSet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> DoneTime { get; set; }
        public Nullable<int> User_Id { get; set; }
    
        public virtual UserSet UserSet { get; set; }
    }
}
