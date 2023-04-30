using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    [Table("UserSet")]
    public class User
    {
        public User()
        {
            Id = 0;
	        UserCreateDate = DateTime.Now;
            Gender = false;
            Photo = null;
            Active = true;
            SessionLogColl = new HashSet<SessionLog>();
            Notes = new HashSet<Notes>();
            Sucursales = new HashSet<Sucursal>( );
        }
		
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
		public string LastNameFather { get; set; }
		public string LastNameMother { get; set; }
		public bool Gender { get; set; }   // true->femenino, false->masculino
		public string Photo { get; set; }
        public bool Active { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime UserCreateDate { get; set; }

        // 1 to many relationship: 1 role can be in many users, but an user can only have 1 role.
        public virtual Role Role { get; set; }
        // many to 1 relationship: 1 user can log in many sessions (many times (1 session per time))
        public virtual ICollection<SessionLog> SessionLogColl { get; set; }
        
        // many to one relationship: one user can have many notes but each note belongs to the user that created it.
        public virtual ICollection<Notes> Notes { get; set; }

		// many to many relationship
		public virtual ICollection<Sucursal> Sucursales { get; set; }

	    public dynamic ToJson()
	    {
			return new
			{
				Id,
				Name,
				LastName = LastNameFather + " " + LastNameMother,
				LastNameFather,
				LastNameMother,
				Gender,
				Active,
				UserName,
				Phone,
				Email,
				Photo,
				Role = new
				{
					Role.Id,
					Role.Name,
					Role.Description
				},
				Sucursales = Sucursales.Select(s => s.ToJson())
			};
		}

	}

    // TO_DO list
    [Table("NotesSet")]
    public class Notes
    {
        public Notes()
        {
            Done = false;
            CreatedTime = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        
        public string Description { get; set; }
        public bool Done { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? DoneTime { get; set; }

        // many to one relationship: one user can have many notes but each note belongs to the user that created it.
        public virtual User User { get; set; }
    }

}