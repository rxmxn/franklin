using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    // Empresa (EMA, CONAGUA, ...). Estas empresas son las encargadas de realizar acreditaciones y reconocimientos.
	// En el sistema se les llama Instituciones.
    [Table("EnterpriseSet")]
    public class Enterprise
    {
        //entrada por admin
        public Enterprise()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
			RecOtorgs = new HashSet<RecOtorg>();
	        Tipo = false;
        }
		
		public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
		public bool Tipo { get; set; }  // interna = false o externa = true
		public string Sede { get; set; }

		// one to many relationship
		public virtual Ack Ack { get; set; }

		// many to one
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }

		public virtual Rama Rama { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Tipo = Tipo ? 0 : 1,
				Ack = Ack != null ? new
				{
					Ack.Id,
					Ack.Key,
					Ack.Name
				} : null,
				Rama = Rama != null ? new
				{
					Rama.Id,
					Rama.Name
				} : null,
				Sede,
				RecOtorg = RecOtorgs.Select(ro => ro.ToJson())
			};
		}
	}

    //Categorias (Acredita, Aprueba, Autoriza, ...)
    [Table("AccionSet")]
    public class Accion
    {
        //entrada por admin
        public Accion()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
            Acks = new HashSet<Ack>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        
		// many to one relationship
        public virtual ICollection<Ack> Acks { get; set; }
		
		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Acks = Acks.Select(e => new
				{
					e.Id,
					e.Name
				})
			};
		}
	}

    //Reconocimientos (Federal, Local Regional)
    [Table("AckSet")]
    public class Ack
    {
        //entrada por admin
        public Ack()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
            Enterprises = new HashSet<Enterprise>();
			RecOtorgs = new HashSet<RecOtorg>();
			//Estado = State.Alta;
		}

		//enum State { Alta, Baja, Cancelacion, Suspension };

		public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
		public string Key { get; set; }
		//public State Estado { get; set; }

		public DateTime? VigenciaInicial { get; set; }
		public DateTime? VigenciaFinal { get; set; }

		// one to one relationship
		public virtual ICollection<Enterprise> Enterprises { get; set; }

		// many to one
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }

		// many to one relationship
		public virtual Accion Accion { get; set; }

		public virtual Alcance Alcance { get; set; }

		public dynamic ToJson()
	    {
		    return new
		    {
			    Id,
			    Name,
			    Active,
			    Description,
				Key,
				Accion = Accion != null ? new
				{
					Accion.Id,
					Accion.Name
				} : null,
				Alcance = Alcance != null ? new
				{
					Alcance.Id,
					Alcance.Name
				} : null,
				//Estado,
				Enterprises = Enterprises.Where(e => e.Active).Select(e => e.ToJson()),
				VigenciaInicial = VigenciaInicial?.ToString("dd/MM/yyyy") ?? "",
				VigenciaFinal = VigenciaFinal?.ToString("dd/MM/yyyy") ?? "",
				Expired = VigenciaFinal < DateTime.Now
			};
	    }
    }

	//Alcance de Reconocimientos (Regional, Local, Federal, Internacional)
	[Table("AlcanceSet")]
	public class Alcance
	{
		//entrada por admin
		public Alcance()
		{
			Id = 0;
			Active = true;
		}

		public int Id { get; set; }
		public string Name { get; set; }    // Alcance (Regional, Local, Federal, Internacional)
		public string Description { get; set; }
		public bool Active { get; set; }
		public string ZonaGeografica { get; set; }

		// many to one
		public virtual ICollection<Ack> Acks { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				ZonaGeografica
			};
		}
	}

	// Reconocimientos Adquiridos
	[Table("RecAdqSet")]
	public class RecAdq
	{
		public RecAdq()
		{
			Id = 0;
			RecOtorgs = new HashSet<RecOtorg>();
        }

		public enum AcquiredLevel { Signatario, Eidas };

		public int Id { get; set; }
		public AcquiredLevel NivelAdquirido { get; set; }

		// many to one
		public virtual TipoSignatario TipoSignatario { get; set; }
		// many to one
		public virtual Annalist Annalist { get; set; }
		// many to one
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }
		
		public dynamic ToJson()
		{
			return new
			{
				Id,
				TipoSignatario = new
				{
					Id = TipoSignatario?.Id ?? null,
					Tipo = NivelAdquirido == AcquiredLevel.Signatario
					? "Signatario " + TipoSignatario?.Name
					: "Reconocido Internamente"
				},
				Annalist = new
				{
					Annalist.Id,
					Name = Annalist.ToString(),
					Annalist.Key
				},
				RecOtorgsToShow = RecOtorgs.Select(ro => new
				{
					ro.Id,
					AcquiredDate = ro.AcquiredDate.ToString("dd/MM/yyyy HH:mm:ss"),
					Ack = ro.Ack.Name,
					Enterprise = ro.Enterprise?.Name,
					Sucursal = ro.Sucursal.ToString(),
					Annalist = ro.RecAdq.Annalist.Key,
					Expired = ro.Ack.VigenciaFinal < DateTime.Now
				}).Distinct(),
				RecOtorgs = RecOtorgs.Select(ro => new
				{
					ro.Id,
					Ack = new
					{
						ro.Ack.Id,
						ro.Ack.Key,
						ro.Ack.Name,
						Expired = ro.Ack.VigenciaFinal < DateTime.Now
					},
					Enterprise = ro.Enterprise != null ? new
					{
						ro.Enterprise.Id,
						ro.Enterprise.Name
					} : null,
					Sucursal = new
					{
						ro.Sucursal.Id,
						Name = ro.Sucursal.ToString()
					},
					Params = ro.Params.Select(p => new
					{
						p.Id,
						p.ParamUniquekey
					})
				})
			};
		}
		
	}

	// Reconocimientos Adquiridos
	[Table("RecOtorgSet")]
	public class RecOtorg
	{
		public RecOtorg()
		{
			Id = 0;
			AcquiredDate = DateTime.Now;
			Params = new HashSet<Param>();
        }
		
		public int Id { get; set; }
		public DateTime AcquiredDate { get; set; }

		// many to one
		public virtual Enterprise Enterprise { get; set; }
		// many to one
		public virtual Ack Ack { get; set; }
		//many to many
		public virtual ICollection<Param> Params { get; set; }
		// many to one
		public virtual RecAdq RecAdq { get; set; }
		// many to one
		public virtual Sucursal Sucursal { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
                Ack = new
				{
					Ack.Id,
					Ack.Key,
					Ack.Name,
					Expired = Ack.VigenciaFinal < DateTime.Now
				},
				Enterprise = Enterprise != null ? new
				{
					Enterprise.Id,
					Enterprise.Name
				} : null,
				Sucursal = new
				{
					Sucursal.Id,
					Name = Sucursal.ToString()
				},
				Params = Params.Select(p => new
				{
					p.Id,
					p.ParamUniquekey
				}),
				RecAdq = RecAdq.ToJson()
			};
		}

		public override string ToString()
		{
			return Enterprise != null ? Enterprise.Name + "/" + Ack.Key : Ack.Key;
		}

	}
}
