using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcPersistent.Models
{
    [Table("SessionLogSet")]
    public class SessionLog
    {
        public long Id { get; set; }
        public bool Connected { get; set; }
        public DateTime? StartSession { get; set; }
		public DateTime? EndSession { get; set; }
		public string Key { get; set; }
		public string IpAddress { get; set; }

		// many to 1 relationship: 1 user can log in many sessions (many times (1 session per time))
		public virtual User User { get; set; }
    }

	[Table("IntelesisSet")]
	public class Intelesis
	{
		public Intelesis()
		{
			Id = 0;
			UltimaActualizacion = DateTime.Now;
		}

		public int Id { get; set; }

		// Para mostrar la fecha de la ultima actualizacion de la base de datos.
		// Esta variable la puedo utilizar tambien en los centros de costo, ya que cuando 
		// se actualice uno, se actualiza el otro.
		public DateTime? UltimaActualizacion { get; set; }
	}

}