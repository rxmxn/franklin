using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcPersistent.Models
{
    [Table("AnalyticsMethodSet")]
    public class AnalyticsMethod
    {
        public AnalyticsMethod()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
            Methods = new HashSet<Method>();
        }

        //entrada por admin
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set;  }
        public bool Active { get; set; }
        //una técnica analítica puede estar en varios métodos pero un método responde a una técnica analítica
        public virtual ICollection<Method> Methods { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description
			};
		}
	}
}
