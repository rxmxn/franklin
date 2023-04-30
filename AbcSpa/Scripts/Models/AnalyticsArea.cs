using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    [Table("AnalyticsAreaSet")]
    public class AnalyticsArea
    {
        public AnalyticsArea()
        {
            Id = 0;
            Active = true;
        }

        //entrada por admin
        public int Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

		// many to one: 1 instalacion tendra muchos centros de costo y cada centro de costo
		// pertenecera solamente a 1 instalacion.
        public virtual Sucursal Sucursal { get; set; }

		// many to one
		public virtual CentroCosto CentroCosto { get; set; }

		//many to one
		public virtual TipoServicio TipoServicio { get; set; } // Clasificacion

		public virtual ICollection<UnidadAnalitica> UnidadesAnaliticas { get; set; }

		public dynamic ToJson()
	    {
		    return new
		    {
				Id,
				Key,
				Active,
				Description,
				Sucursal = Sucursal != null
				? new
				{
					Sucursal.Id,
					Name = Sucursal.ToString()
				} : null,
				CentroCosto = CentroCosto != null
				? new
				{
					CentroCosto.Id,
					CentroCosto.Number
				} : null,
				TipoServicio = TipoServicio != null
				? new
				{
					TipoServicio.Id,
					TipoServicio.Name
				} : null,
				UnidadesAnaliticas = UnidadesAnaliticas.Where(ua => ua.Active)
					.Select(ua => new { ua.Id, ua.Key, ua.AnnalistKey })
			};
	    }

		public override string ToString()
		{
			return Key;
		}
    }

	[Table("CentroCostoSet")]
	public class CentroCosto
	{
		public CentroCosto()
		{
			Id = 0;
			Active = true;
			ParameterToAnalyze = new HashSet<Param>();
		}

		public enum TipoCentroCosto { Mixto, DeGasto, DeIngreso };

		//entrada por admin
		public int Id { get; set; }
		public string Number { get; set; }	
		public string Description { get; set; }
		public bool Active { get; set; }
		public TipoCentroCosto Tipo { get; set; }
		
		public virtual ICollection<Param> ParameterToAnalyze { get; set; }

		public virtual ICollection<AnalyticsArea> AreasAnaliticas { get; set; }
		
		public dynamic ToJson()
		{
			return new
			{
				Id,
				Number,
				Active,
				Description,
				Tipo
			};
		}

		public override string ToString()
		{
			return Number;
		}
	}

	[Table("UnidadAnaliticaSet")]
	public class UnidadAnalitica
	{
		public UnidadAnalitica()
		{
			Id = 0;
			Active = true;
			Params = new HashSet<Param>();
        }

		//entrada por admin
		public int Id { get; set; }
		public string Key { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public string AnnalistKey { get; set; }

		// many to one
		public virtual AnalyticsArea AreaAnalitica { get; set; }
		public virtual ICollection<Param> Params { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Key,
				Active,
				Description,
				AnnalistKey,
				AreaAnalitica = AreaAnalitica != null
				? new
				{
					AreaAnalitica.Id,
					AreaAnalitica.Key
				}
				: null
			};
		}

		public override string ToString()
		{
			return Key;
		}
	}
}
