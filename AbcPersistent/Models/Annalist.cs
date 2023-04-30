using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcPersistent.Models
{
    [Table("AnnalistSet")]
    public class Annalist
    {
        public Annalist()
        {
            Id = 0;
            Active = true;
            Gender = false;
            Params = new HashSet<Param>();
            Sucursales = new HashSet<Sucursal>();
            RecAdqs = new HashSet<RecAdq>();
        }

        //entrada por admin
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastNameFather { get; set; }
        public string LastNameMother { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }   // true->femenino, false->masculino
                                           //public string Key { get; set; }
        public string Curriculum { get; set; }
        public string Firma { get; set; }
        // De Intelesis (nuevos campos)
        public DateTime? FechaAlta { get; set; }
        public string Puesto { get; set; }
        public string NivelAcademico { get; set; }
        public string Departamento { get; set; }
        public string NoEmpleado { get; set; }
        // public virtual AnnalistKey AnnalistKey { get; set; }
        public string Key { get; set; }
        public virtual Jornada Jornada { get; set; }

        //Todos los parámetros de un mismo método son examinados por un mismo analista.
        //Puede ser MAS DE UN ANALISTA los que los realicen,
        //pero TODOS los analistas harían TODOS los parámetros.
        // De cualquier manera, los usuarios prefieren asignar los analistas a cada
        // parametro en vez de a metodo.
        // many to many
        //public virtual ICollection<Method> Methods { get; set; }
        public virtual ICollection<Param> Params { get; set; }

        // many to many relationship
        public virtual ICollection<Sucursal> Sucursales { get; set; }

        public virtual ICollection<RecAdq> RecAdqs { get; set; }

        public dynamic ToJson()
        {
            return new
            {
                Id,
                Name,
                LastName = LastNameFather + " " + LastNameMother,
                LastNameFather,
                LastNameMother,
                Key,//= AnnalistKey?.ToJson(),
                Description,
                Gender,
                Active,
                Phone,
                Email,
                Photo,
                Curriculum,
                Firma,
                FechaAlta = FechaAlta?.ToString("dd/MM/yyyy") ?? "",
                Puesto,
                NoEmpleado,
                Sucursales = Sucursales.Select(u => u.ToJson()),
                RecAdqs = RecAdqs.Select(ra => ra.ToJson())
            };
        }

        public override string ToString()
        {
            return Name + " " + LastNameFather + " " + LastNameMother;
        }
    }

    [Table("TipoSignatarioSet")]
    public class TipoSignatario
    {
        public TipoSignatario()
        {
            Id = 0;
            Active = true;
            RecAdqs = new HashSet<RecAdq>();
        }

        //entrada por admin
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<RecAdq> RecAdqs { get; set; }

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

    [Table("JornadaSet")]
    public class Jornada
    {
        public Jornada()
        {
            Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = Sunday = SaleSiguienteDia = EsRotativo = false;
        }

        [Key, ForeignKey("Annalist")]
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool SaleSiguienteDia { get; set; }
        public bool EsRotativo { get; set; }

        public virtual Annalist Annalist { get; set; }

        public dynamic ToJson()
        {
            return new
            {
                Id,
                Description,
                Monday,
                Tuesday,
                Wednesday,
                Thursday,
                Friday,
                Saturday,
                Sunday,
                SaleSiguienteDia,
                EsRotativo
            };
        }
    }

    [Table("AnnalistKeySet")]
    public class AnnalistKey
    {
        public AnnalistKey()
        {
            Active = true;
            Parameters=new HashSet<Param>();
            //Annalists= new HashSet<Annalist>();
        }
        public int Id { get; set; }
        public string Clave { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        //public virtual ICollection<Annalist> Annalists { get; set; }
        public virtual UnidadAnalitica UnidadAnalitica { get; set; }
        public virtual ICollection<Param> Parameters { get; set; }
        public dynamic ToJson()
        {
            return new
            {
                Id,
                Clave,
                Active,
                Description

            };
        }
    }
}
