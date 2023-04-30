using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcPersistent.Models
{
    [Table("TipoServicioSet")]
    public class TipoServicio
	{
        public TipoServicio()
        {
            Id = 0;			 
            Active = true;
			Parameters = new HashSet<Param>();
			Groups = new HashSet<Group>();
			Packages = new HashSet<Package>();
		}
		
		//entrada por admin
		public int Id { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }
        public bool Active { get; set; }

		// many to one
		public virtual ICollection<Param> Parameters { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
		public virtual ICollection<Package> Packages { get; set; }
		public virtual ICollection<AnalyticsArea> AnalyticsAreas { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Description,
				Active
			};
		}

	    public override string ToString()
	    {
			return Name;
	    }
    }
	
}
