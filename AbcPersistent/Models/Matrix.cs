using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcPersistent.Models
{
    [Table("MatrixSet")]
    public class Matrix
    {
        //entrada por admin (nombre y descripcion)
        public Matrix()
        {
            Id = 0;
            Active = true;
            Parameters = new HashSet<Param>();
            //Methods = new HashSet<Method>();
			BaseParams = new HashSet<BaseParam>();
            Groups = new HashSet<Group>();
            Packages = new HashSet<Package>();
            ParamRoutes = new HashSet<ParamRoute>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SubMatrix { get; set; }
        public string SubMtrxDescription { get; set; }
        public string Description { get; set; }
        // Variables to filter the data
        public bool Active { get; set; }
        public DateTime MatrixCreateDate { get; set; }
        public virtual ICollection<ParamRoute> ParamRoutes { get; set; }
        //one to many:
        public virtual ICollection<Param> Parameters { get; set; }
		public virtual ICollection<BaseParam> BaseParams { get; set; }
		//public virtual ICollection<Method> Methods { get; set; }
		//many to many:
		public virtual ICollection<Group> Groups { get; set; }
        //many to many:
        public virtual ICollection<Package> Packages { get; set; }
        // many to one relationship 
        //(se supone que una matriz esta directamente asociada a una sucursal, por lo que la misma matriz no 
        //estara en mas de una sucursal)
        public virtual Sucursal Sucursal { get; set; }
        public virtual BaseMatrix BaseMatrix { get; set; }
        public virtual Rama Rama { get; set; }

        public dynamic ToJson()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            //var matrixChildren = new List<dynamic>();

            return new
            {
                elemType = "matrix",
                Id,
                Name,
                SubMatrix,
                SubMtrxDescription,
                BaseMatrix = this.BaseMatrix.ToJson(),
                Description,
                Active,
                MatrixCreateDate = MatrixCreateDate.ToString("dd/MM/yyyy HH:mm"),
                Sucursal = new
                {
                    Sucursal.Id,
                    Sucursal.Name
                }
                //children = matrixChildren
                //    .Concat(Packages.Select(pk => pk.ToJson(Id)))
                //    .Concat(Groups.Select(g => g.ToJson(Id)))
                //    .Concat(Parameters.Select(p => p.ToJson(Id)))
            };
        }

        public dynamic ToMiniJson()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            //  var matrixChildren = new List<dynamic>();

            return new
            {
                elemType = "matrix",
                Id,
                Name,
                Active,
                SubMatrix,
                SubMtrxDescription,
                Description,
                BaseMatrix = BaseMatrix.ToMiniJson()

                //children = matrixChildren
                //    .Concat(Packages.Select(pk => pk.ToMiniJson(Id)))
                //    .Concat(Groups.Select(g => g.ToMiniJson()))
                //    .Concat(Parameters.Select(p => p.ToMiniJson(Id)))

            };
        }

        public string whereareyou()
        {
            if (Sucursal != null)
            {
                return Sucursal.Region.Name + "/" + Sucursal.Name + "/" + BaseMatrix.Name;
            }
            else
            {
                return "";
            }
        }

    }

    //entrada por admin (revisar los metodos que son necesarios entrar por admin, inicialmente nombre y descripcion)
    //OJO El modelo de MatrizBase describe a los diferentes Grupos de Matrices
    [Table("BaseMatrixSet")]
    public class BaseMatrix
    {
        public BaseMatrix()
        {
            Id = 0;
            Name = "";
            Description = "";
            Active = true;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // many to one relationship
        // un metodo puede estar en varios parametros pero un parametro solamente tendra un metodo
        public virtual ICollection<Matrix> Matrixes { get; set; }
        public virtual Market Mercado { get; set; }
        public dynamic ToJson()
        {
            return new
            {
                Id,
                Name,
                Active,
                Description,
                Mercado = Mercado.ToJson()
            };
        }
        public dynamic ToMiniJson()
        {
            return new
            {
                Id,
                Name,
                Mercado = new { Mercado.Id, Mercado.Name }
            };
        }
    }
}
