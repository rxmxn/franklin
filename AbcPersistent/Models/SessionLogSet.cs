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
    
    public partial class SessionLogSet
    {
        public long SessionLogId { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Key { get; set; }
        public Nullable<int> User_Id { get; set; }
    
        public virtual UserSet UserSet { get; set; }
    }
}
