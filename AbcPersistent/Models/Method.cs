using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AbcPersistent.Models
{
    // Tipo de dato necesario para metodo
    public class Limit
    {
        public Limit()
        {
            Decimals = 8;
        }
        public double? Value { get; set; }
        public int Decimals { get; set; }

	    public override string ToString()
	    {
		    return Value + "(" + Decimals + ")";
		    //return base.ToString();
	    }

	    public dynamic ToJson()
	    {
		    return new
		    {
				Value,
				Decimals
		    };
	    }
    }

    // Si acepta QC, determinar los limites superiores e inferiores del criterio de aceptacion.
    public class Qc
    {
        public Qc()
        {
            HasQc = false;
        }

        public bool HasQc { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }

		public override string ToString()
		{
			return HasQc ? "[" + LowerLimit + "," + UpperLimit + "]" : "No Qc";
			//return base.ToString();
		}

		public dynamic ToJson()
		{
			return new
			{
				HasQc,
				UpperLimit,
				LowerLimit
			};
		}
	}

    //Un Metodo es la "receta" con la cual se hace la prueba (el analisis del parametro).
    [Table("MethodSet")]
    public class Method
    {
        //entrada por admin (revisar los metodos que son necesarios entrar por admin, inicialmente nombre y descripcion)
        public Method()
        {
            Id = 0;
            Active = true;
            
            //DetectionLimit = new Limit();
            //CuantificationLimit = new Limit();
            // Uncertainty = new Limit();
           // QcObj = new Qc();
            //Formula = "NA";
            Parameters = new HashSet<Param>();
			//Annalists = new HashSet<Annalist>();
			//Matrixes = new HashSet<Matrix>();
			Norms = new HashSet<Norm>();
            TiposServicios = new HashSet<TipoServicio>();
		}
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        //public double? RequiredVolume { get; set; }
        //public double? MinimumVolume { get; set; }
        //public double? ReportLimit { get; set; }
        //public int? DeliverTime { get; set; }  // tiempo de entrega al cliente en dias
        //public Limit DetectionLimit { get; set; }   // Limite de deteccion del metodo
        //public Limit CuantificationLimit { get; set; }  //Limite practico de cuantificacion
        //public Limit Uncertainty { get; set; }  // Incertidumbre
        //public int? MaxTimeBeforeAnalysis { get; set; }  // Tiempo maximo previo al analisis [dias]
        //public int? LabDeliverTime { get; set; }  // Tiempo de entrega al laboratorio [dias]
        //public Qc QcObj { get; set; }
        //public int? ReportTime { get; set; }  // tiempo que tiene el analista para reportar [dias]

		public DateTime? EntradaEnVigor { get; set; }
        public DateTime? FechaCambioStatus { get; set; }
		// many to one relationship
		// un metodo puede estar en varios parametros pero un parametro solamente tendra un metodo
		public virtual ICollection<Param> Parameters { get; set; }

        // one to many relationship: a method will have its container, but the same container can be in many methods
        //public virtual Container Container { get; set; }
        // one to many relationship: a method will have its preserver, but the same preserver can be in many methods
       // public virtual Preserver Preserver { get; set; }
        public virtual ICollection<TipoServicio> TiposServicios { get; set; } 
		public virtual ICollection<Norm> Norms { get; set; }
		//public virtual ICollection<Matrix> Matrixes { get; set; }

        public virtual Status Estado { get; set; }

		public dynamic ToJson(bool sheet = false)
		{
			if (!sheet)
			{
				return new
				{
					Id,
					Name,
					Active,
					Description,
					//RequiredVolume,
					//MinimumVolume,
					//Formula,
					//DeliverTime,
                    //ReportLimit,
                    //DetectionLimit = DetectionLimit.ToJson(),
					//CuantificationLimit = CuantificationLimit.ToJson(),
					//Uncertainty = Uncertainty.ToJson(),
					//MaxTimeBeforeAnalysis,
					//LabDeliverTime,
					//QcObj = QcObj.ToJson(),
					//ReportTime,
					//Container = Container?.ToJson(),
					//Preserver = Preserver?.ToJson(),
					//Matrixes = Matrixes?.Where(m => m.Active)
					//.Select(m => new
					//{
					//	m.Id,
					//	BaseMatrix = new
					//	{
					//		m.BaseMatrix.Id,
					//		Mercado = m.BaseMatrix.Mercado.Name
					//	}
					//}),
					Norms = Norms?.Where(n => n.Active).Select(n => n.ToJson()),
					EntradaEnVigor =  EntradaEnVigor?.ToString("dd/MM/yyyy") ?? "",
                    FechaCambioStatus = FechaCambioStatus?.ToString("dd/MM/yyyy") ?? "",
                    Estado = Estado?.ToJson(),
                    TiposServicios=TiposServicios?.Where(ts=>ts.Active).Select(ts=>ts.ToJson())
				};
			}

			return new
			{
				Id,
				Name,
				Active,
				Description,
				//RequiredVolume,
				//MinimumVolume,
				//Formula,
				//DeliverTime,
               //ReportLimit,
                //DetectionLimit = DetectionLimit.ToJson(),
				//CuantificationLimit = CuantificationLimit.ToJson(),
				//Uncertainty = Uncertainty.ToJson(),
				//MaxTimeBeforeAnalysis,
				//LabDeliverTime,
				//QcObj = QcObj.ToJson(),
				//ReportTime,
				//Container = Container?.Name,
				//Preserver = Preserver?.Name
				//AnalyticsMethod = AnalyticsMethod?.Name
				//Annalists = Annalists.Select(a => a.ToString())
			};
		}

        public override string ToString()
        {
            return Name ;
        }

    }
	
	//Envase dependiente del metodo
	[Table("ContainerSet")]
    public class Container   // no se puede heredar de una clase sealed. (sealed se debe poner ya que se esta inicializando un metodo virtual en el constructor).
    {
        //entrada por admin
        public Container()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
			//Methods = new HashSet<Method>();
			Params = new HashSet<Param>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
		public double? Capacity { get; set; } //Es la capacidad del volumen del Envase en la unidad de medida: mL.

		// one to many relationship
		public virtual Material Material { get; set; }
        // one to many relationship: a method will have its container, but the same container can be in many methods
		//public virtual ICollection<Method> Methods { get; set; }
		public virtual ICollection<Param> Params { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Capacity,
				Material = Material.ToJson()
			};
		}
	}

    //Preservador dependiente del metodo
    [Table("PreserverSet")]
    public class Preserver
    {
        //entrada por admin
        public Preserver()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
			//Methods = new HashSet<Method>();
			Params = new HashSet<Param>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // one to many relationship: a method will have its preserver, but the same preserver can be in many methods
		//public virtual ICollection<Method> Methods { get; set; }

		public virtual ICollection<Param> Params { get; set; }

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

    // Desecho
    [Table("ResidueSet")]
    public class Residue
    {
        //entrada por admin
        public Residue()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
			Params = new HashSet<Param>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // one to many relationship
        public virtual ICollection<Param> Params { get; set; }

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

	//Material del Envase
	[Table("MaterialSet")]
	public class Material   
	{
		//entrada por admin
		public Material()
		{
			Id = 0;
		    Name = "";
		    Description = "";
			Active = true;
			Containers = new HashSet<Container>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		
		// one to many relationship
		public virtual ICollection<Container> Containers { get; set; }

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
