using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    // Oficina o Empresa (Por defecto ABC)
    [Table("OfficeSet")]
    public class Office
    {
        //entrada por admin
        public Office()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
			Sucursales = new HashSet<Sucursal>();
			Regions = new HashSet<Region>();
			RecOtorgs = new HashSet<RecOtorg>();
		}
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // many to one relationship 
        // en estos momentos un Mercado es lo mismo que una sucursal
        // desarrollamos como hipotesis que en un futuro pudieran existir mas sucursales por mercado,
        // por lo que preparamos el codigo para esto.
        public virtual Market Market { get; set; }

		// many to many relationship 
		public virtual ICollection<Region> Regions { get; set; }

		public virtual ICollection<Sucursal> Sucursales { get; set; }

		// many to one
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Market = Market.ToJson(),
				RecOtorgs = RecOtorgs.Select(ro => new
				{
					rec = ro.ToString(),
					Expired = ro.Ack.VigenciaFinal < DateTime.Now
				}).Distinct()
			};
		}

        public override string ToString()
        {
            return Name;
		}
    }

    // Mercado (ahora mismo se interpreta como la sucursal) 
    [Table("MarketSet")]
    public class Market
    {
        //entrada por admin
        public Market()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
            Offices = new HashSet<Office>();
            //Regions = new HashSet<Region>();
            BaseMatrix = new HashSet<BaseMatrix>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // many to one relationship 
        // en estos momentos un Mercado es lo mismo que una oficina
        // desarrollamos como hipotesis que en un futuro pudieran existir mas sucursales por mercado,
        // por lo que preparamos el codigo para esto.
        public virtual ICollection<Office> Offices { get; set; }

        // many to many relationship 
        // una region esta compuesta por varios mercados, y un mercado puede estar en varias regiones
        //public virtual ICollection<Region> Regions { get; set; }
		// (se le asigna el mercado a la region a traves de las oficinas)

        public virtual ICollection<BaseMatrix> BaseMatrix { get; set; }

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

        public dynamic ToMiniJson()
        {
            return new
            {
                Id,
                Name
            };
        }
    }

    // Region (elemento mas amplio que abarca el resto del sistema) 
    [Table("RegionSet")]
    public class Region
    {
        //entrada por admin
        public Region()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
            //Markets = new HashSet<Market>();
			Offices = new HashSet<Office>();
			Sucursales = new HashSet<Sucursal>();
		}
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
		public string Key { get; set; }

		// many to one relationship 
		// una region esta compuesta por varios mercados
		//public virtual ICollection<Market> Markets { get; set; }

		// many to many
		public virtual ICollection<Office> Offices { get; set; }

		public virtual ICollection<Sucursal> Sucursales { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Key,
				Offices = Offices.Select(o => new
				{
					o.Id,
					o.Name,
					MarketName = o.Market.Name
				}),
				Sucursales = Sucursales.Where(s => s.Active.Equals(true))
					.Select(s => new
					{
						s.Id,
						s.Name,
						HaveAckRoutes = s.RecOtorgs.Any(ro => ro.Enterprise != null && (ro.Ack.VigenciaFinal > DateTime.Now))
					})
			};
		}
	}

	// Instalación: Oficinas de Ventas, muestreo y parámetros de campo (OVMPC)
	[Table("SucursalSet")]
	public class Sucursal
	{
		public Sucursal()
		{
			Id = 0;
			Active = true;
			Vende = false;
			Realiza = false;
           // Matrixes = new HashSet<Matrix>();
			Users = new HashSet<User>();
			Annalists = new HashSet<Annalist>();
			AnalyticsArea = new HashSet<AnalyticsArea>();
			RecOtorgs = new HashSet<RecOtorg>();
			SucursalRealizaParams = new HashSet<Param>();
			SucursalVendeParams = new HashSet<Param>();
			Groups = new HashSet<Group>();
			Packages = new HashSet<Package>();
			Offices = new HashSet<Office>();
        }

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public string Key { get; set; }
		public bool Vende { get; set; }
		public bool Realiza { get; set; }
		public int? SucursalIntelesis { get; set; }
		public int? SucursalAutolab { get; set; }

		// one to many relationship. Empresa a la que pertenece la Instalacion.
		//public virtual Office Office { get; set; }
		public virtual ICollection<Office> Offices { get; set; }

		//one to many relationship
		public virtual Region Region { get; set; }

		// many to many: un usuario puede estar en diferentes sucursales y una sucursal tendra muchos usuarios
		public virtual ICollection<User> Users { get; set; }

        // many to one relationship 
        //(se supone que una matriz esta directamente asociada a una sucursal, por lo que la misma matriz no 
        //estara en mas de una sucursal).
       // public virtual ICollection<Matrix> Matrixes { get; set; }

		// many to many
		public virtual ICollection<Annalist> Annalists { get; set; }

		// many to one: 1 instalacion tendra muchos centros de costo y cada centro de costo
		// pertenecera solamente a 1 instalacion.
		public virtual ICollection<AnalyticsArea> AnalyticsArea { get; set; }
		// many to one
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }
		
        public virtual ICollection<Param> SucursalVendeParams { get; set; }
		public virtual ICollection<Param> SucursalRealizaParams { get; set; }

		public virtual ICollection<Group> Groups { get; set; }

		public virtual ICollection<Package> Packages { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Description,
				Key,
				Active,
				Vende,
				Realiza,
				SucursalIntelesis,
				SucursalAutolab,
				Offices = Offices.Where(o => o.Active).Select(o => new
				{
					o.Id,
					o.Name,
					Market = new
					{
						o.Market.Id,
						o.Market.Name
					},
					RecOtorgs = RecOtorgs.Where(ro => ro.Office.Id.Equals(o.Id))
					.Select(ro => new
					{
						rec = ro.ToString(),
						Expired = ro.Ack.VigenciaFinal < DateTime.Now
					}).Distinct(),
				}),
				Region = new
				{
					Region.Id,
					Region.Name
				},
				RecOtorgs = RecOtorgs.Select(ro => new
				{
					rec = ro.ToString(),
					Expired = ro.Ack.VigenciaFinal < DateTime.Now,
					Office = ro.Office.Id
				}).Distinct(),
				AnalyticsArea = AnalyticsArea.Select(a => new
				{
					a.Id,
					a.Key
				})
			};
		}

        public dynamic ToMiniJson()
        {
            //var sucursalChildren = new List<dynamic>();
            return new
            {
                elemType = "Instalacion",
                Id,
                Name,
                Key,
                Offices = Offices.Where(o => o.Active).Select(o => o.Name),
                Region = Region.Name,
                //children = sucursalChildren.Concat(Matrixes.Select(m => m.ToMiniJson()))

            };
        }

        //public dynamic ToJsonForLMP()
        //{
        //    var sucursalChildren = new List<dynamic>();
        //    return new
        //    {
        //        elemType = "Installation",
        //        Id,
        //        Name,
        //        Key,
        //        Office = Office.Name,
        //        Region = Region.Name,
        //        children = sucursalChildren.Concat(Matrixes.Select(m => m.ToMiniJson()))

        //    };
        //}
		public override string ToString()
		{
			//return Region.Name + '/' + Office.Market.Name + '/' + Office.Name;
            return Region.Name + " - " + Name;
		}
	}
}
