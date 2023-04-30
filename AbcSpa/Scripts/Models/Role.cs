using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    [Table("RoleSet")]
    public class Role
    {
        public Role()
        {
            UserColl = new HashSet<User>();
            RightColl = new HashSet<Right>();
			ParamCols = new HashSet<ParamCol>();
        }
		
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // 1 to many relationship: 1 role can be in many users, but an user can only have 1 role.
        public virtual ICollection<User> UserColl { get; set; }
        // many to many relationship: 1 role can have many rights and 1 right can be in many roles.
        public virtual ICollection<Right> RightColl { get; set; }
		// many to many relationship: 1 role can have many rights and 1 right can be in many roles.
		public virtual ICollection<ParamCol> ParamCols { get; set; }

	    public dynamic ToJson()
	    {
		    return new
		    {
			    Id,
			    Name,
			    Description,
			    Active,
			    RightColl = RightColl.Select(rc => rc.ToJson()),
			    ParamCols = ParamCols.Select(p => p.ToJson())
		    };
	    }
	}
}