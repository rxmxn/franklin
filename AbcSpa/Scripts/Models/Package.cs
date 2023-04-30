using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;

namespace AbcPersistent.Models
{
    [Table("PackageSet")]
    public class Package
    {
        private readonly AbcContext _dbStore = new AbcContext();
        public Package()
        {
            Name = "";
            Description = "";
            JustQuotePrice = false;
            Active = true;
            Parameters = new HashSet<Param>();
            Groups = new HashSet<Group>();
            Matrixes = new HashSet<Matrix>();
            ParamRoutes = new HashSet<ParamRoute>();
			PublishInAutolab = false;
			SellSeparated = true;
			CuentaEstadistica = false;
            DecimalesReporte = null;
		}

        // Entrada por admin
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool JustQuotePrice { get; set; }   // solo cotizar
		public bool PublishInAutolab { get; set; }
		public bool SellSeparated { get; set; }
		public bool CuentaEstadistica { get; set; }
        public int? DecimalesReporte { get; set; }
		// one to many relationship
		//public virtual Norm Norm { get; set; }    // relacion con la norma (Standard) Si norm=null no responde a norma
        public virtual ICollection<ParamRoute> ParamRoutes { get; set; }
        // many to many relationship:
        public virtual ICollection<Param> Parameters { get; set; }
        // many to many relationship:
        public virtual ICollection<Group> Groups { get; set; }
        // many to many
        public virtual ICollection<Matrix> Matrixes { get; set; }
		//many to one
		public virtual TipoServicio TipoServicio { get; set; }

		public virtual Sucursal Sucursal { get; set; }

        //public virtual Param DispParam { get; set; }

		public dynamic ToJson(int rootId = 0, bool sheet = false)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var packageChildren = new List<dynamic>();

			if (!sheet)
            return new
            {
                rootId,
                elemType = "package",
                Id,
                Active,
                Name,
                Description,
				PublishInAutolab,
				SellSeparated,
				Sucursal = Sucursal?.ToJson(),
				CuentaEstadistica,
                    DecimalesReporte,
                //Norm = Norm?.ToJson(),
				TipoServicio = TipoServicio?.ToJson(),
                    Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
				Precio = new
                {
                    Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)
                            + Groups.Where(g => g.Active).Sum(g => g.Parameters.Where(p => p.Active)
                            .Sum(p => p.Precio.Value)),
                    Currency = new
                    {
                        Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ??
                                Groups.FirstOrDefault(g => g.Active)?.Parameters
                                .FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                    }
                },
                children = packageChildren
					.Concat(Parameters.Where(p => p.Active).Select(p => p.ToJson(Id, false, 0, 0, DecimalesReporte)))
					.Concat(Groups.Where(g => g.Active).Select(g => g.ToJson(Id)))
            };

			var mtrx = Matrixes?.First();

			return new
            {
                rootId,
                elemType = "package",
                Id,
                Active,
                Name,
                Description,
				PublishInAutolab,
				SellSeparated,
				CuentaEstadistica,
                DecimalesReporte,
				TipoServicio = TipoServicio?.ToJson(),
				//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, m.Name, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
				Matrix = new
				{
					mtrx?.Id,
					mtrx?.Name,
					BaseMatrix = new { mtrx?.BaseMatrix.Id, Mercado = mtrx?.BaseMatrix.Mercado.Name }
				}, 
				//Norm = Norm?.ToJson(),
                Precio = new
                {
                    Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)
                            + Groups.Where(g => g.Active).Sum(g => g.Parameters.Where(p => p.Active)
                            .Sum(p => p.Precio.Value)),
                    Currency = new
                    {
                        Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ??
                                Groups.FirstOrDefault(g => g.Active)?.Parameters
                                .FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                    }
                },
                children = packageChildren
					.Concat(Parameters.Where(p => p.Active).Select(p => p.ToJson(Id, true, 0, 0, DecimalesReporte)))
					.Concat(Groups.Where(g => g.Active).Select(g => g.ToJson(Id, true)))
            };
        }
        public dynamic ToMiniJson(IEnumerable<ParamRoute> paramRoutes)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var packageChildren = new List<dynamic>();

            return new
            {
                elemType = "package",
                Id,
                Name,
                Description,
               // Norm = Norm?.ToString(),
                Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
                Precio = (Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)
                         + Groups.Where(g => g.Active).Sum(g => g.Parameters.Where(p => p.Active)
                             .Sum(p => p.Precio.Value))) + (Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? Groups.FirstOrDefault(g => g.Active)?.Parameters
                                .FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"),
                //Precio = new
                //{
                //    Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)
                //            + Groups.Where(g => g.Active).Sum(g => g.Parameters.Where(p => p.Active)
                //            .Sum(p => p.Precio.Value)),
                //    Currency = new
                //    {
                //        Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ??
                //                Groups.FirstOrDefault(g => g.Active)?.Parameters
                //                .FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                //    }
                //},
                children = packageChildren
                    .Concat(Groups.Where(g => g.Active).Distinct().Select(g => g.ToMiniJson(paramRoutes?.Where(pr => (pr.Group != null) && pr.Group.Id == g.Id || pr.Group == null).AsEnumerable())))
                    .Concat(Parameters.Where(p => p.Active).ToList().Select(p => p.ToMiniJson(paramRoutes?.FirstOrDefault(pr => pr.Group == null && pr.Package.Id == Id))))
                // .Concat(Parameters.Where(p => p.Active).Select(p => p.ToMiniJson(0)))
            };
        }

		public dynamic ToMainInfoJson(int rootId = 0)
		{
			// ReSharper disable once CollectionNeverUpdated.Local
			var packageChildren = new List<dynamic>();

			return new
			{
				rootId,
				elemType = "package",
				Id,
				Name,
				Description,
				Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
				//Norm = Norm?.ToJson(),
				Precio = new
				{
					Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)
							+ Groups.Where(g => g.Active).Sum(g => g.Parameters.Where(p => p.Active)
							.Sum(p => p.Precio.Value)),
					Currency = new
					{
						Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ??
								Groups.FirstOrDefault(g => g.Active)?.Parameters
								.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
    }
				},
				children = packageChildren
					.Concat(Groups.Where(g => g.Active).Select(g => g.ToMainInfoJson(Id)))
					.Concat(Parameters.Where(p => p.Active).Select(p => p.ToMainInfoJson(Id)))
			};
		}

	}

  //  [Table("NormSet")]
  //  public class Norm
  //  {
  //      public Norm()
  //      {
  //          Name = "";
  //          Description = "";
  //          Active = true;
  //          Checked = false;
  //          Packages = new HashSet<Package>();
		//	Methods = new HashSet<Method>();
  //      }

  //      //entrada por admin
  //      public int Id { get; set; }
  //      public string Name { get; set; }
  //      public string Description { get; set; }
  //      public bool Active { get; set; }
  //      public bool Checked { get; set; }   //msg de advertencia si ya existe un paquete que responde a esa norma
  //      //Un paquete responde a una norma y varios paquetes pueden ser creados por la misma norma
  //      public virtual ICollection<Package> Packages { get; set; }
		//public virtual ICollection<Method> Methods { get; set; }

		//public dynamic ToJson()
  //      {
  //          return new
  //          {
  //              Id,
  //              Name,
  //              Active,
  //              Description
  //          };
  //      }

  //      public override string ToString()
  //      {
  //          return Name;
  //      }
  //  }

    [Table("GroupSet")]
    public class Group
    {
        private readonly AbcContext _dbStore = new AbcContext();
        public Group()
        {
            Name = "";
            Description = "";
            Active = true;
            Parameters = new HashSet<Param>();
            Matrixes = new HashSet<Matrix>();
            Packages = new HashSet<Package>();
            ParamRoutes = new HashSet<ParamRoute>();
            ParamPrintResults = new HashSet<ParamPrintResult>();
			PublishInAutolab = false;
			SellSeparated = false;
			CuentaEstadistica = false;
			MostrarLista = false;
	        MuestreosCompuestos = false;
			Week = new WeekProgram();
			ComplexSamplings = new HashSet<ComplexSampling>();
            DispParamId = 0;
        }

        // Entrada por admin
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
		public bool PublishInAutolab { get; set; }
		public bool SellSeparated { get; set; }
		public bool CuentaEstadistica { get; set; }
		public bool MostrarLista { get; set; }
        //public int? DecimalesReporte { get; set; }
        public int DispParamId { get; set; }
		public bool MuestreosCompuestos { get; set; }
        public bool ImpResultado { get; set; }
		public WeekProgram Week { get; set; }

		//one to many relationship:
		public virtual ICollection<ParamRoute> ParamRoutes { get; set; }
        public virtual ICollection<Param> Parameters { get; set; }
        public virtual ICollection<Matrix> Matrixes { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<ParamPrintResult> ParamPrintResults { get; set; }
      
		//many to one
		public virtual TipoServicio TipoServicio { get; set; }
		//many to one
		public virtual ICollection<ComplexSampling> ComplexSamplings { get; set; }

		public virtual Sucursal Sucursal { get; set; }

		//many to one
		[InverseProperty("GroupsCq1")]
		public virtual ClasificacionQuimica ClasificacionQuimica1 { get; set; }
		[InverseProperty("GroupsCq2")]
		public virtual ClasificacionQuimica ClasificacionQuimica2 { get; set; }
		[InverseProperty("GroupsCq3")]
		public virtual ClasificacionQuimica ClasificacionQuimica3 { get; set; }

		public dynamic ToJson(int rootId = 0, bool sheet = false)
        {
	        if (!sheet)
            return new
            {
                elemType = "group",
				MostrarLista,
				Id,
                Name,
                Active,
				MuestreosCompuestos,
				Sucursal = Sucursal?.ToJson(),
				ComplexSamplings = ComplexSamplings.Select(cs => cs.ToJson()),
                Description,
                ParamPrintResults = ParamPrintResults?.Select(prr => prr.ToJson()),
				TipoServicio = TipoServicio?.ToJson(),
				PublishInAutolab,
				SellSeparated,
                ImpResultado,
               // DecimalesReporte,
				CuentaEstadistica,
				Week = Week.ToJson(),
                DispParam = (DispParamId != 0) ? new
				{
					Name = _dbStore.ParamSet.Find(DispParamId).ParamUniquekey,
					DispParamId
				} : null,
                Precio = new
                {
                    Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
                    Currency = new
                    {
                        Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                    }
                },
                ClasificacionQuimica1 = ClasificacionQuimica1?.ToJson(),
				ClasificacionQuimica2 = ClasificacionQuimica2?.ToJson(),
				ClasificacionQuimica3 = ClasificacionQuimica3?.ToJson(),
                children = Parameters.Where(p => p.Active).Select(p => p.ToJson(Id, sheet, 0, Id/*, DecimalesReporte*/)),		// ToList()
                Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
                Packages = Packages?.Where(pk => pk.Active).Select(pk => new { pk.Id, pk.Name })
            };

	       // var tempMatrix = Matrixes.Where(m => m.Active).ToList()[0];

			// ReSharper disable once CollectionNeverUpdated.Local
			var groupChildren = new List<dynamic>();

			foreach (var param in Parameters.Where(p => p.Active &&
				ComplexSamplings.Any(cs => cs.Param.Id.Equals(p.Id) && cs.Group.Id.Equals(Id))))
			{
				for (var i = 0; i < param.ComplexSamplings
					.FirstOrDefault(cs => cs.Group.Id.Equals(Id))?.CantidadMuestreos ; i++)
				{
					// Adicionando los parametros cuyos muestreos son compuestos.
					groupChildren.Add(param.ToJson(Id, true, /*DReporte: DecimalesReporte,*/ numeroMuestreo: i + 1));
				}
			}

			var mtrx = Matrixes?.First();

			return new
	        {
		        elemType = "group",
				MostrarLista,
				Id,
		        Name,
		        Active,
		        Description,
				TipoServicio = TipoServicio?.ToJson(),
				PublishInAutolab,
				SellSeparated,
                ImpResultado,
                //DecimalesReporte,
				CuentaEstadistica,
				MuestreosCompuestos,
				ComplexSamplings = ComplexSamplings.Select(cs => cs.ToJson()),
				Week = Week.ToJson(),
				//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new {
				//	m.Id,
				//	m.Name,
				//	BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name }
				//}),
				Matrix = new
				{
					mtrx?.Id,
					mtrx?.Name,
					BaseMatrix = new { mtrx?.BaseMatrix.Id, Mercado = mtrx?.BaseMatrix.Mercado.Name }
				},
				Precio = new
		        {
			        Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
			        Currency = new
			        {
				        Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
			        }
		        },
				ClasificacionQuimica1 = ClasificacionQuimica1?.ToJson(),
				ClasificacionQuimica2 = ClasificacionQuimica2?.ToJson(),
				ClasificacionQuimica3 = ClasificacionQuimica3?.ToJson(),
				// Adicionando los parametros cuyos muestreos son simples.
				children = groupChildren.Concat(Parameters.Where(p => p.Active &&
					ComplexSamplings.All(cs => cs.Param.Id != p.Id))
					.Select(p => p.ToJson(Id, true/*, DReporte: DecimalesReporte*/))) 
                //children = Parameters.Where(p => p.Active).Select(p => p.ToJson(Id, sheet, 0, 0, DecimalesReporte)).ToList()
	        };
        }

        public dynamic ToMiniJson(IEnumerable<ParamRoute> paramRoutes)
        {
            return new
            {
                //rootId,
                elemType = "group",
				MostrarLista,
				Id,
                Name,
                Description,
                route = Matrixes.Where(m => m.Active).Select(m => m.whereareyou()),
                Precio = (Parameters.Where(p => p.Active).Sum(p => p.Precio.Value)) + (Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"),
                children = Parameters.Where(p => p.Active).Select(p => p.ToMiniJson(paramRoutes?.FirstOrDefault(pr => pr.Parameter.Id == p.Id), null/*DecimalesReporte*/, Id)).ToList()
            };
        }

		public dynamic ToMainInfoJson(int rootId = 0)
		{
			return new
			{
				rootId,
				elemType = "group",
				MostrarLista,
				Id,
				Name,
				Description,
				Matrixes = Matrixes?.Where(m => m.Active).Select(m => new {
					m.Id,
					BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name }
				}),
				Precio = new
				{
					Value = Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
					Currency = new
					{
						Name = Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
					}
				},
				children = Parameters.Where(p => p.Active).Select(p => p.ToMainInfoJson(Id, Id))
			};
		}
    }

	[Table("ComplexSamplingSet")]
	public class ComplexSampling
	{
		public ComplexSampling()
		{
			Id = 0;
		}
		
		public int Id { get; set; }
		
		public int CantidadMuestreos { get; set; }

		//many to one
		public virtual Param Param { get; set; }

		//many to one
		public virtual Group Group { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Param = new
				{
					Param.Id,
					Param.ParamUniquekey
				},
				CantidadMuestreos
			};
		}
	}
}
