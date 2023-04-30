using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace AbcPersistent.Models
{
	[Table("PriceSet")]
	public class Price
	{
		public Price()
		{
			Id = 0;
			Value = -1;
			Description = "";
		}

		public int Id { get; set; }

		public int Value { get; set; }
		public string Description { get; set; }
		public virtual Currency Currency { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Value,
				Currency = Currency.ToJson()
			};
		}

		public override string ToString()
		{
			return Value + Currency.Name;
		}
	}

	[Table("CurrencySet")]
	public class Currency
	{
		public Currency()
		{
			Id = 0;
			Name = "";
			Description = "";
			Active = true;
			Prices = new HashSet<Price>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		//one to many relationship:
		public virtual ICollection<Price> Prices { get; set; }

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

	[Table("MeasureUnitSet")]
	public class MeasureUnit
	{
		public MeasureUnit()
		{
			Id = 0;
			Active = true;
			ParametersUnits = new HashSet<Param>();
			BaseParams = new HashSet<BaseParam>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }

		//may to many relationship
		public virtual ICollection<BaseParam> BaseParams { get; set; }
		public virtual ICollection<Param> ParametersUnits { get; set; }

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

		public override string ToString()
		{
			return Name;
		}

	}

	//Esta tabla almacena los variables físicas que integran los parámetros (ejemplo ph,). En esta clase se relaciona con otra tabla
	//donde se almacena la familia de materiales: estas serían metales, metales alcalinos, metales solubles, minerales, etc.
	//Todos estos datos deben ser introducidos por la sección de administración antes de poder ser utilizados en el sistema.
	[Table("BaseParamSet")]
	public class BaseParam
	{
		public BaseParam()
		{
			Id = 0;
			Name = "";
			Description = "";
			// PrintInResultReport = false;
			Active = true;
			Parameters = new HashSet<Param>();
			Units = new HashSet<MeasureUnit>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		// public bool PrintInResultReport { get; set; }
		//one to many relationship: one BaseParam has one BaseParamFamily and one BaseParaFamily has many BaseParam
		//public virtual BaseParamFamily ParamFamily { get; set; }

		//many to one
		[InverseProperty("BaseParamsCq1")]
		public virtual ClasificacionQuimica ClasificacionQuimica1 { get; set; }
		[InverseProperty("BaseParamsCq2")]
		public virtual ClasificacionQuimica ClasificacionQuimica2 { get; set; }
		[InverseProperty("BaseParamsCq3")]
		public virtual ClasificacionQuimica ClasificacionQuimica3 { get; set; }

		//one to many relationship:
		public virtual ICollection<Param> Parameters { get; set; }

		//un parámetro base (de ahora en adelante magnitud física) puede tener varias unidades de medida pero una unidad de medida 
		//solo corresponde a una magnitud física. me refiero a que la temperatura se puede medir en C o F o K y sigue siendo temperatura,
		// pero el grado c solo pertenece a temperatura. quizás sea mejor definir un listado de unidades de medidas 
		//para cada magnitud física y a la hora de conformar el parámetro como tal entonces el usuario seleccione una unidad de medida 
		//dentro de todas las que puede tener la magnitud física. entonces parámetro tiene  una relación de uno a mucho con unides de medida o sea un parámetro
		//ses mide una unidad de medida y una unidad de medida puede utilizarse para varios parámetros
		public virtual ICollection<MeasureUnit> Units { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Description,
				Active,
				//PrintInResultReport,
				//ParamFamily = ParamFamily?.ToJson(),
				Units = Units.Select(u => u.ToJson()),
				ClasificacionQuimica1 = ClasificacionQuimica1?.ToJson(),
				ClasificacionQuimica2 = ClasificacionQuimica2?.ToJson(),
				ClasificacionQuimica3 = ClasificacionQuimica3?.ToJson()
			};
		}
		private readonly AbcContext _dbStore = new AbcContext();
		public dynamic ToJsonTree(bool alta, bool baja, string userName, string searchGeneral)
		{
			var sucursales = _dbStore.SucursalSet.Where(s => s.Users.Any(u => u.UserName.Equals(userName)));
			return new
			{
				Id,
				Name,
				Description,
				//PrintInResultReport,
				//ParamFamily = ParamFamily?.ToJson(),
				Units = Units.Select(u => u.ToJson()),
				elemType = "baseparam",
				children = Parameters.Where(p => ((alta && baja) ||
							(alta && p.Active) || (baja && !p.Active))
							&& (sucursales.Any(s => s.AnalyticsArea.Any(aa => aa.Id.Equals(p.CentroCosto.Id))))
							&& (string.IsNullOrEmpty(searchGeneral) ||
									(p.ParamUniquekey.ToUpper().Contains(searchGeneral.ToUpper()))
									|| (p.BaseParam.Name.ToUpper().Contains(searchGeneral.ToUpper()))))
							//.Select(p => p.ToJson(Id)).ToList()
							.Select(p => p.ToJson())
			};
		}

		public override string ToString()
		{
			return Name;
		}
	}

	[Table("ClasificacionQuimicaSet")]
	public class ClasificacionQuimica
	{
		public ClasificacionQuimica()
		{
			Id = 0;
			Active = true;
			Level = 1;
			BaseParamsCq1 = new HashSet<BaseParam>();
			BaseParamsCq2 = new HashSet<BaseParam>();
			BaseParamsCq3 = new HashSet<BaseParam>();
			GroupsCq1 = new HashSet<Group>();
			GroupsCq2 = new HashSet<Group>();
			GroupsCq3 = new HashSet<Group>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public int Level { get; set; }  // cada clas. quim. puede tener 3 niveles.

		//many to one
		public virtual ICollection<BaseParam> BaseParamsCq1 { get; set; }
		public virtual ICollection<BaseParam> BaseParamsCq2 { get; set; }
		public virtual ICollection<BaseParam> BaseParamsCq3 { get; set; }

		//many to one
		public virtual ICollection<Group> GroupsCq1 { get; set; }
		public virtual ICollection<Group> GroupsCq2 { get; set; }
		public virtual ICollection<Group> GroupsCq3 { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Level
			};
		}
	}

	[Table("ParamSet")]
	public class Param
	{
		private readonly AbcContext _dbStore = new AbcContext();
		public Param()
		{
			Id = 0;
			Active = true;
			Formula = "NA";
			InternetPublish = false;
			Uncertainty = new Limit();
			QcObj = new Qc();
			Packages = new HashSet<Package>();
			Groups = new HashSet<Group>();
			ParamRoutes = new HashSet<ParamRoute>();
			ParamPrintResults = new HashSet<ParamPrintResult>();
			Annalists = new HashSet<Annalist>();
			RecOtorgs = new HashSet<RecOtorg>();
			PublishInAutolab = false;
			SellSeparated = true;
			CuentaEstadistica = false;
			DecimalesReporte = null;
			//Matrixes = new HashSet<Matrix>();
			Week = new WeekProgram();
			ComplexSamplings = new HashSet<ComplexSampling>();
		}
		public int Id { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public int? MaxPermitedLimit { get; set; }
		public int? PerTurnCapacity { get; set; }
		public int? PerWeekCapacity { get; set; }
		public int? DecimalesReporte { get; set; }
		//nombre de área asignada en autolab
		public string AutolabAssignedAreaName { get; set; }
		//Clave única del parámetro.Ojo no es el Id es para el trabajo de los clientes con otro soft
		public string ParamUniquekey { get; set; }
		//clave genérica para estadística
		public string GenericKeyForStatistic { get; set; }
		//	public string GenericDescription { get; set; }
		public string GenericKey { get; set; }
		public bool ResiduoPeligroso { get; set; }
		public bool ReportaCliente { get; set; }
		public bool PublishInAutolab { get; set; }
		public bool SellSeparated { get; set; }
		public bool CuentaEstadistica { get; set; }
		public WeekProgram Week { get; set; }
		public string Formula { get; set; }
		public bool InternetPublish { get; set; }  // true si se puede publicar en internet
		public int? AnalysisTime { get; set; }  // tiempo para el analisis

		//El precio es un tipo de datos
		public virtual Price Precio { get; set; }
		public Qc QcObj { get; set; }
		//one to many relationship: one Parameter has one BaseParam but one BaseParam has many Parameters
		public Limit Uncertainty { get; set; }  // Incertidumbre
		public virtual BaseParam BaseParam { get; set; }
		//El centro de costo es una tabla que guarda los datos de cada una de las áreas analiticas ej: metales etc
		public virtual CentroCosto CentroCosto { get; set; }

		public virtual Rama Rama { get; set; }

		public virtual UnidadAnalitica UnidadAnalitica { get; set; }

		//Todos los parámetros de un mismo método son examinados por un mismo analista.
		//Puede ser MAS DE UN ANALISTA los que los realicen,
		//pero TODOS los analistas harían TODOS los parámetros.
		// De cualquier manera, los usuarios prefieren asignar los analistas a cada
		// parametro en vez de a metodo.
		// many to many
		public virtual ICollection<Annalist> Annalists { get; set; }

		//Cada parametro tiene un método con el que es muestreado
		//one to many ralationship: 
		public virtual Method Metodo { get; set; }

		public virtual MeasureUnit Unit { get; set; }

		//  public virtual BaseMatrix BaseMatrix { get; set; }
		// many to many: Un parámetro puede estar en varias matrices siempre que estas pertenezcan al mismo grupo de matrices
		//public virtual ICollection<Matrix> Matrixes { get; set; }
		public virtual Matrix Matrix { get; set; }

		//many to many relationship
		public virtual ICollection<RecOtorg> RecOtorgs { get; set; }
		// many to many relationship
		public virtual ICollection<Package> Packages { get; set; }
		// many to many relationship
		public virtual ICollection<Group> Groups { get; set; }
		public virtual ICollection<ParamRoute> ParamRoutes { get; set; }
		public virtual ICollection<ParamPrintResult> ParamPrintResults { get; set; }
		//many to one
		public virtual TipoServicio TipoServicio { get; set; }
		public virtual AnalyticsMethod AnalyticsMethod { get; set; }
		//many to one
		public virtual ICollection<ComplexSampling> ComplexSamplings { get; set; }

		[InverseProperty("SucursalVendeParams")]
		public virtual Sucursal SucursalVende { get; set; }
		[InverseProperty("SucursalRealizaParams")]
		public virtual Sucursal SucursalRealiza { get; set; }

		// one to many relationship
		public virtual Residue Residue { get; set; }

		public dynamic ToJson(int rootId = 0, bool sheet = false, int level = 0, int grupoId = 0,
			int? DReporte = null, int numeroMuestreo = 0)
		{
			var printResult = grupoId == 0
				? _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group == null && prr.Parameter.Id == Id)
				: _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group != null && prr.Group.Id == grupoId && prr.Parameter.Id == Id);
			var dispParamsId = (grupoId != 0) ? _dbStore.GroupSet.Find(grupoId).DispParamId : 0;

			if (!sheet)
			{
				return new
				{
					rootId,
					elemType = "parameter",
					level,
					Id,
					Active,
					Name = ParamUniquekey,  // para mostrar como nombre en la tabla
					Description,
					ParamUniquekey,         // para editar el parametro
					Formula,
					InternetPublish,
					//MaxPermitedLimit = paramRoute!=null?new {Value=paramRoute.Value}:null,
					PerTurnCapacity,
					PerWeekCapacity,
					ParamPrintResults = printResult != null ? new { printResult.Id, printResult.Yes } : null,
					AutolabAssignedAreaName,
					GenericKeyForStatistic,
					Precio = Precio?.ToJson(),
					GenericDescription = BaseParam.Name,
					GenericKey,
					AnalysisTime,
					UnidadAnalitica = (UnidadAnalitica != null)
					? new
					{
						UnidadAnalitica.Id,
						AreaAnalitica = UnidadAnalitica.AreaAnalitica.Key,
						UnidadAnalitica.AnnalistKey
					} : null,
					Rama = (Rama != null)
					? new
					{
						Rama.Id,
						Rama.Name
					} : null,
					DecimalesReporte = DecimalesReporte ?? DReporte,
					ResiduoPeligroso,
					ReportaCliente,
					PublishInAutolab,
					SellSeparated,
					CuentaEstadistica,
					Week = Week.ToJson(),
					ClasificacionQuimica1 = BaseParam.ClasificacionQuimica1?.ToJson(),
					ClasificacionQuimica2 = BaseParam.ClasificacionQuimica2?.ToJson(),
					ClasificacionQuimica3 = BaseParam.ClasificacionQuimica3?.ToJson(),
					AnalyticsMethod = AnalyticsMethod?.ToJson(),
					QcObj = QcObj.ToJson(),
					Uncertainty = Uncertainty.ToJson(),
					DispParam = (dispParamsId != 0) ? new { Name = _dbStore.ParamSet.Find(dispParamsId).ParamUniquekey, Id = _dbStore.ParamSet.Find(dispParamsId).Id } : null,
					BaseParam = new
					{
						BaseParam.Id,
						BaseParam.Name,
						BaseParam.Description
					},
					CentroCosto = (CentroCosto != null)
					? new
					{
						CentroCosto.Id,
						CentroCosto.Number,
						CentroCosto.Description
					}
					: null,
					Signatarios = Annalists.Where(a => a.Active && a.RecAdqs
						.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Signatario)
						&& ra.RecOtorgs.Any(ro => ro.Enterprise.Tipo
						&& ro.Params.Any(p => p.Id.Equals(Id))))).Select(a => a.Key),
					Eidas = Annalists.Where(a => a.Active && a.RecAdqs
						.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Eidas)
						&& ra.RecOtorgs.Any(ro => !ro.Enterprise.Tipo
						&& ro.Params.Any(p => p.Id.Equals(Id))))).Select(a => a.Key),
					Annalists = Annalists.Select(a => new
					{
						a.Id,
						Name = a.ToString(),
						a.Key
					}),
					Metodo = (Metodo != null)
					? new
					{
						Metodo.Id,
						Metodo.Name,
						Matrixes = Metodo.Matrixes.Select(m => new {m.Id, m.Name})
						//	AnalyticsMethod = Metodo.AnalyticsMethod!=null? Metodo.AnalyticsMethod.Name:"N/A"
					}
					: null,
					SucursalVende = (SucursalVende != null) ? new { SucursalVende.Id, SucursalVende.Name } : null,
					SucursalRealiza = (SucursalRealiza != null) ? new { SucursalRealiza.Id, SucursalRealiza.Name } : null,
					Unit = Unit?.ToJson(),
					Residue = Residue?.ToJson(),
					//RecOtorgs = RecOtorgs.Where(ro => ro.Enterprise != null).Select(ro => ro.ToJson()),
					RecOtorgs = RecOtorgs.Where(ro => ro.Enterprise != null).Select(ro => new { ro.Ack.Key, Expired = ro.Ack.VigenciaFinal < DateTime.Now }).Distinct().OrderBy(ro => ro.Key),
					ParamRoutes = ParamRoutes.Select(pr => pr.ToJson()),
					TipoServicio = TipoServicio?.ToJson(),
					//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
					Matrix = Matrix != null ? new
					{
						Matrix.Id,
						Matrix.Name,
						BaseMatrix = new
						{
							Matrix.BaseMatrix.Id,
							Mercado = Matrix.BaseMatrix.Mercado.Name
						}
					} : null,
					Groups = Groups?.Where(g => g.Active).Select(g => new { g.Id, g.Name }),
					Packages = Packages?.Where(pk => pk.Active).Select(pk => new { pk.Id, pk.Name }),
					ComplexSamplings.FirstOrDefault(cs => cs.Group.Id.Equals(grupoId))?.CantidadMuestreos
				};
			}

			//var routList = RecOtorgs.Where(ro => ro.Enterprise != null).Select(ar => ar.ToString() + ", ");
			//var routes = routList.Aggregate("", (current, r) => current + (r));

			return new
			{
				rootId,
				elemType = "parameter",
				Id,
				//BaseParam.Name,
				MaxPermitedLimit,
				PerTurnCapacity,
				PerWeekCapacity,
				AutolabAssignedAreaName,
				Name = ParamUniquekey + (numeroMuestreo == 0 ? "" : numeroMuestreo.ToString()),
				Description = Description + (numeroMuestreo == 0 ? "" : "-" + numeroMuestreo),
				ParamUniquekey,
				UnidadAnalitica?.AnnalistKey,
				Rama = Rama?.Name,
				ResiduoPeligroso,
				ReportaCliente,
				PublishInAutolab,
				SellSeparated,
				CuentaEstadistica,
				Week = Week.ToJson(),
				GenericKeyForStatistic,
				GenericDescription = BaseParam.Name,
				GenericKey,
				InternetPublish,
				AnalysisTime,
				Residue = Residue?.Name,
				DecimalesReporte = DecimalesReporte ?? DReporte,
				TipoServicio = TipoServicio?.ToJson(),
				ClasificacionQuimica1 = BaseParam.ClasificacionQuimica1?.ToJson(),
				ClasificacionQuimica2 = BaseParam.ClasificacionQuimica2?.ToJson(),
				ClasificacionQuimica3 = BaseParam.ClasificacionQuimica3?.ToJson(),
				QcObj = QcObj.ToJson(),
				Uncertainty = Uncertainty.ToJson(),
				AnalyticsMethod = AnalyticsMethod?.ToJson(),
				DispParam = (dispParamsId != 0) ? new { Name = _dbStore.ParamSet.Find(dispParamsId).ParamUniquekey, Id = _dbStore.ParamSet.Find(dispParamsId).Id } : null,
				Precio = new
				{
					Precio.Value,
					Currency = new { Precio.Currency.Name }
				},
				CentroCosto = new
				{
					CentroCosto?.Number,
					CentroCosto?.AreasAnaliticas.FirstOrDefault(a => a.Active)?.Key
				},
				Signatarios = Annalists.Where(a => a.Active && a.RecAdqs
					.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Signatario)
					&& ra.RecOtorgs.Any(ro => ro.Enterprise.Tipo
					&& ro.Params.Any(p => p.Id.Equals(Id))))).Select(a => a.Key),
				Eidas = Annalists.Where(a => a.Active && a.RecAdqs
					.Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Eidas)
					&& ra.RecOtorgs.Any(ro => !ro.Enterprise.Tipo
					&& ro.Params.Any(p => p.Id.Equals(Id))))).Select(a => a.Key),
				SucursalVende = (SucursalVende != null) ? new { SucursalVende.Id, Name = SucursalVende.ToString() } : null,
				SucursalRealiza = (SucursalRealiza != null) ? new { SucursalRealiza.Id, Name = SucursalRealiza.ToString() } : null,
				//Annalists = Annalists.Select(a => a.ToString()),
				Metodo = Metodo?.ToJson(true),
				Unit = Unit?.Name,
				//RecOtorgs = RecOtorgs.Where(ro => ro.Enterprise != null).Select(ro => ro.Ack.Key).Distinct(),
				RecOtorgs = RecOtorgs.Where(ro => ro.Enterprise != null).Select(ro => new { ro.Ack.Key, Expired = ro.Ack.VigenciaFinal < DateTime.Now }).Distinct().OrderBy(ro => ro.Key),
				//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, m.Name, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
				Matrix = Matrix != null ? new
				{
					Matrix.Id,
					Matrix.Name,
					BaseMatrix = new
					{
						Matrix.BaseMatrix.Id,
						Mercado = Matrix.BaseMatrix.Mercado.Name
					}
				} : null,
				Groups = Groups?.Where(g => g.Active).Select(g => new { g.Id, g.Name }),
				Packages = Packages?.Where(pk => pk.Active).Select(pk => new { pk.Id, pk.Name })
			};
		}

		public dynamic ToMiniJson(ParamRoute paramRoute, int? DReporte=null, int groupId = 0)
		{
			return new
			{
				//rootId,
				elemType = "parameter",
				Id,
				Name = ParamUniquekey,
				AutolabAssignedAreaName,
                DecimalesReporte = DecimalesReporte ?? DReporte,
                Description,
				GenericKey,
				Precio = Precio?.ToString(),
				ParamPrintResults = groupId != 0 ? _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group != null && prr.Group.Id == groupId && prr.Parameter.Id == Id)?.ToJson() :
													_dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group == null && prr.Parameter.Id == Id)?.ToJson(),
				BaseParam = BaseParam?.ToString(),
				CentroCosto = CentroCosto?.ToString(),
				Annalists = Annalists.Select(a => a.Key),
				Metodo = Metodo?.ToString(),
				Unit = Unit?.ToString(),
				MaxPermitedLimit = paramRoute != null ? new { paramRoute.Id, paramRoute.Value, paramRoute.DecimalsPoints } : null,
				//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id })
				Matrix = Matrix != null ? new { Matrix.Id, Matrix.Name } : null
			};
		}

		public dynamic ToMainInfoJson(int rootId = 0, int grupoId = 0)
		{
			return new
			{
				rootId,
				elemType = "parameter",
				Id,
				AutolabAssignedAreaName,
				Name = ParamUniquekey,
				Description,
				Precio = new
				{
					Precio.Value,
					Currency = new { Precio.Currency.Name }
				},
				CentroCosto = new
				{
					CentroCosto?.Number
				},
				SucursalVende = (SucursalVende != null) ? new { SucursalVende.Id, SucursalVende.Name } : null,
				SucursalRealiza = (SucursalRealiza != null) ? new { SucursalRealiza.Id, SucursalRealiza.Name } : null,
				Metodo = Metodo?.ToJson(true),
				//Matrixes = Matrixes?.Where(m => m.Active).Select(m => new { m.Id, BaseMatrix = new { m.BaseMatrix.Id, Mercado = m.BaseMatrix.Mercado.Name } }),
				Matrix = Matrix != null ? new
				{
					Matrix.Id,
					Matrix.Name,
					BaseMatrix = new
					{
						Matrix.BaseMatrix.Id,
						Mercado = Matrix.BaseMatrix.Mercado.Name
					}
				} : null,
				//Groups = Groups?.Where(g => g.Active).Select(g => new { g.Id, g.Name }),
				//Packages = Packages?.Where(pk => pk.Active).Select(pk => new { pk.Id, pk.Name }),
				ComplexSamplings.FirstOrDefault(cs => cs.Group.Id.Equals(grupoId))?.CantidadMuestreos
			};
		}
	}

	[Table("ParamRouteSet")]
	public class ParamRoute
	{
		public ParamRoute()
		{
			Id = 0;
			Value = 0;
			DecimalsPoints = 8;

		}
		public int Id { get; set; }
		public double Value { get; set; }
		public int DecimalsPoints { get; set; }
		public virtual Param Parameter { get; set; }
		public virtual Group Group { get; set; }
		public virtual Package Package { get; set; }
		// public virtual Matrix Matrix { get; set; }
		public virtual MaxPermitedLimit MaxPermitedLimit { get; set; }
		public dynamic ToJson()
		{
			return new
			{
				Id,
				// Matrix = Matrix != null ? new { Matrix.Id } : null,
				Package = Package != null ? new { Package.Id } : null,
				Group = Group != null ? new { Group.Id } : null,
				Parameter = new { Parameter.Id },
				MaxPermitedLimit = new { MaxPermitedLimit.Id },
				Value,
				DecimalsPoints
			};
		}
	}

	[Table("MaxPermitedLimitSet")]
	public class MaxPermitedLimit
	{
		private readonly AbcContext _dbStore = new AbcContext();
		public MaxPermitedLimit()
		{
			Id = 0;
			Name = "";
			Description = "";
			Active = true;
			ParamRoutes = new HashSet<ParamRoute>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public virtual ICollection<ParamRoute> ParamRoutes { get; set; }

		public dynamic ToJson()
		{
			var LMPChildren = new List<dynamic>();

			return new
			{
				Id,
				Name,
				Active,
				Description,

				children = LMPChildren
						 .Concat(ParamRoutes.Where(pr => pr.MaxPermitedLimit.Id == Id && pr.Package != null).Select(pr => pr.Package).Distinct().Select(pk => pk.ToMiniJson(ParamRoutes.Where(pr => pr.Package != null && pk.Id == pr.Package.Id).AsEnumerable())))
						 .Concat(ParamRoutes.Where(pr => pr.MaxPermitedLimit.Id == Id && pr.Package == null && pr.Group != null).Select(pr => pr.Group).Distinct()
								.Select(g => g.ToMiniJson(ParamRoutes.Where(pro => (pro.Package == null) && (pro.Group != null) && g.Id == pro.Group.Id).AsEnumerable())))
						 .Concat(ParamRoutes.Where(pr => pr.MaxPermitedLimit.Id == Id && pr.Package == null && pr.Group == null).Select(pr => pr.Parameter.ToMiniJson(pr)))

			};
		}

		public dynamic ToMiniJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description

			};
		}

		public override string ToString()
		{
			return Name;
		}

	}

	[Table("ParamPrintResultSet")]
	public class ParamPrintResult
	{
		public ParamPrintResult()
		{
			Id = 0;
			Active = true;
			Yes = false;

		}
		public int Id { get; set; }
		public bool Active { get; set; }
		public bool Yes { get; set; }
		public virtual Param Parameter { get; set; }
		public virtual Group Group { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Yes,
				Parameter = Parameter != null ? new { Parameter.Id, Parameter.ParamUniquekey } : null,
				Group = Group != null ? new { Group.Id, Group.Name } : null,
				Active
			};
		}

	}

	// Programación por días de la semana
	public class WeekProgram
	{
		public WeekProgram()
		{
			Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = Sunday = false;
		}

		public bool Monday { get; set; }
		public bool Tuesday { get; set; }
		public bool Wednesday { get; set; }
		public bool Thursday { get; set; }
		public bool Friday { get; set; }
		public bool Saturday { get; set; }
		public bool Sunday { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Monday,
				Tuesday,
				Wednesday,
				Thursday,
				Friday,
				Saturday,
				Sunday
			};
		}
	}

	[Table("RamaSet")]
	public class Rama
	{
		public Rama()
		{
			Id = 0;
			Active = true;
			Enterprises = new HashSet<Enterprise>();
			Matrixes = new HashSet<Matrix>();
			Params = new HashSet<Param>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }

		//one to many relationship
		public virtual ICollection<Matrix> Matrixes { get; set; }
		public virtual ICollection<Enterprise> Enterprises { get; set; }
		public virtual ICollection<Param> Params { get; set; }

		public dynamic ToJson()
		{
			return new
			{
				Id,
				Name,
				Active,
				Description,
				Matrixes = Matrixes.Where(m => m.Active).Select(m => new { m.Id, m.Name }),
				Enterprises = Enterprises.Where(e => e.Active).Select(e => new { e.Id, e.Name })
			};
		}

		public override string ToString()
		{
			return Name;
		}

	}
}
