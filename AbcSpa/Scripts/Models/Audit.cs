using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AbcPersistent.Models
{
    [Table("AuditSet")]
    public class Audit
    {
		public Guid AuditId { get; set; }
		public string IpAddress { get; set; }
		public string UserName { get; set; }
		public DateTime EventDate { get; set; }
		public int ElementId { get; set; }	//modified element Id (before it was an string)
		
		public string EventType { get; set; }
		public string TableName { get; set; }
		public string ColumnName { get; set; }
		public string NewValue { get; set; }
		public string OriginalValue { get; set; }

	}
}
