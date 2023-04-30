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
            //  AnnalistKeys=new HashSet<AnnalistKey>();
            UnidadesAnaliticas=new HashSet<UnidadAnalitica>();
        }

        //entrada por admin
        public int Id { get; set; }
        public string Key { get; set; }
        //public virtual ICollection<AnnalistKey>  AnnalistKeys { get; set; }
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
                //AnnalistKeys=AnnalistKeys?.Select(ak=>ak.ToJson()),
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
                    .Select(ua => new { ua.Id, ua.Key, AnnalistKey = ua.AnnalistKeys?.Where(ak => ak.Active).Select(ak => ak.ToJson()) })
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
            AnnalistKeys = new HashSet<AnnalistKey>();
           //AreaAnalitica = new List<AnalyticsArea>();
        }

        //entrada por admin
        public int Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        //public string AnnalistKey { get; set; }
        public virtual ICollection<AnnalistKey> AnnalistKeys { get; set; }
        // many to one
        // public virtual ICollection<AnalyticsArea> AreaAnalitica { get; set; }
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
                AnnalistKeys = AnnalistKeys?.Where(a => a.Active).Select(a => a.ToJson()),
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
