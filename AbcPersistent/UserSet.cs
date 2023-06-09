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
    
    public partial class UserSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserSet()
        {
            this.NotesSet = new HashSet<NotesSet>();
            this.SessionLogSet = new HashSet<SessionLogSet>();
            this.SucursalSet = new HashSet<SucursalSet>();
        }
    
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public string Photo { get; set; }
        public bool Active { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public System.DateTime UserCreateDate { get; set; }
        public Nullable<int> Role_Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotesSet> NotesSet { get; set; }
        public virtual RoleSet RoleSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionLogSet> SessionLogSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SucursalSet> SucursalSet { get; set; }
    }
}
