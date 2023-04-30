using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcPersistent.Models
{
    [Table("RightSet")]
    public class Right
    {
        public Right()
        {
            Role = new HashSet<Role>();
        }
		
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
		public int Level { get; set; }

		// many to many relationship: 1 role can have many rights and 1 right can be in many roles.
		public virtual ICollection<Role> Role { get; set; }

	    public dynamic ToJson()
	    {
		    return new
		    {
			    Id,
			    Name,
			    Value,
			    Description,
			    Level
		    };
	    }
    }

	[Table("ParamColSet")]
	public class ParamCol
	{
		public ParamCol()
		{
			Role = new HashSet<Role>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }
		public string Description { get; set; }

		// many to many relationship: 1 role can have many rights and 1 right can be in many roles.
		public virtual ICollection<Role> Role { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Key,
				Description
			};
		}
	}
}