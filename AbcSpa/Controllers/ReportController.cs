using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using AbcPersistent.Models;
using Parameter = Microsoft.Office.Interop.Excel.Parameter;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using Microsoft.Office.Core;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Antlr.Runtime;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;

namespace AbcSpa.Controllers
{
    public class ReportController : Controller
    {
        // variable to do queries on the DB
        private readonly AbcContext _dbStore = new AbcContext();
		
        [HttpGet, ActionName("GenerateReport")]
        public FileResult GenerateReport(/*int options, bool isExcel, int orderId)*/
                                            bool viewParameters, bool viewGroups, bool viewPackages,
                                            int marketId = 0, int matrixId = 0, int sucursalId = 0, bool isExcel = true)
        {
            try
            {
                //--------------------------------------------
                var generatedPath = SaveSamplingRequest(viewParameters, viewGroups, viewPackages,
                                                                marketId, matrixId, sucursalId, isExcel);
                //--------------------------------------------

                var fileBytes = GetFile(generatedPath);
                var fileName = generatedPath.Substring(generatedPath.LastIndexOf('\\'));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
		
        //---------------------------------------------
        internal string SaveSamplingRequest(bool viewParameters, bool viewGroups, bool viewPackages,
											int marketId = 0, int matrixId = 0, int sucursalId = 0, bool isExcel = true)
        {
            var appExl = new Excel.Application();
            var templatePath = Path.Combine(Server.MapPath("~/Content/templates"), "Reporte.xlsx");
            var workbook = appExl.Workbooks.Open(templatePath, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            SetupSamplingRequest(workbook, viewParameters, viewGroups, viewPackages,
                                                                marketId, matrixId, sucursalId, isExcel);

            //////Salvar el Archivo Temporal
            var docname = "Reporte";
            var docPathExcel = @ConfigurationManager.AppSettings["TempReportsDirectory"] + "generated\\excel\\" + docname + ".xlsx";
            if (System.IO.File.Exists(docPathExcel))
                try
                {
                    System.IO.File.Delete(docPathExcel);
                }
                catch (Exception e)
                {
                    var errmsg = e.Message;
                }
            workbook.SaveAs(docPathExcel);

            try
            {
                workbook.Close();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
                workbook = null;

                appExl.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(appExl);
                appExl = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception)
            {
                workbook = null;
                appExl = null;
            }

            var idproc = GetIdProcces("EXCEL");

            if (idproc != -1)
            {
                Process.GetProcessById(idproc).Kill();
            }

            if (isExcel)
                return docPathExcel;

            var docPathPdf = @ConfigurationManager.AppSettings["TempReportsDirectory"] + docname + ".pdf";
            ConvertExcelToPdf(docPathExcel, docPathPdf);
            return docPathPdf;
        }
        //---------------------------------------------
        private void SetupSamplingRequest(Excel._Workbook workbook, bool viewParameters, bool viewGroups, bool viewPackages,
											int marketId = 0, int matrixId = 0, int sucursalId = 0, bool isExcel = true)
        {
            var ws1 = workbook.Sheets["Sheet1"];

            var elemList = Getdata(viewParameters, viewGroups, viewPackages,
                marketId, matrixId, sucursalId);

            #region DATOS GENERALES
            var index = 3;

            foreach (dynamic t in elemList.ToList())
            {
                if (t.elemType == "matrix")
                {
                    ws1.Range("A" + index, "L" + index).Merge(false);
                    ws1.Range("A" + index).Value = "Matriz: " + t.Name;
                    ws1.Range("A" + index).HorizontalAlignment = 1;
                    //ws1.Range("B" + index).Value = t.Name;
                    index++;
                    continue;
                }
                ws1.Range("A" + index).Value = t.Name;
                ws1.Range("A" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                ws1.Range("A" + index).IndentLevel = t.level;
                ws1.Range("A" + index, "AG" + index).BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                ws1.Range("B" + index).Value = t.elemType;
                ws1.Range("B" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                ws1.Range("C" + index).Value = t.Description ?? "N/A";
                ws1.Range("C" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                // ws1.Range("B" + index).IndentLevel = t.level;


                try
                {
                    ws1.Range("H" + index).Value = t.Precio.Value + "(" + t.Precio.Currency.Name + ")";
                    ws1.Range("H" + index).BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                }
                catch (Exception)
                {
                    ws1.Range("H" + index).Value = "";
                    ws1.Range("H" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                }

                if (t.elemType == "parameter")
                {
                    //ws1.Range("B" + index).Value = t.elemType;
                    //ws1.Range("B" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("D" + index).Value = t.Metodo != null ? t/*.Metodo*/.AnalyticsMethod : "N/A";
                    ws1.Range("D" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("E" + index).Value = t.Metodo != null ? t.Metodo.Name : "N/A";
                    ws1.Range("E" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("F" + index).Value = t.MaxPermitedLimit != null ? t.MaxPermitedLimit.Value : "N/A";
                    ws1.Range("F" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //ws1.Range("H" + index).Value = t.AutolabAssignedAreaName;
                    //ws1.Range("H" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("I" + index).Value = t.GenericKeyForStatistic ?? "N/A";
                    ws1.Range("I" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
					//TODO: Aqui deberia ir el Area Analitica!!!
					ws1.Range("J" + index).Value = t.CentroCosto?.Key;
                    ws1.Range("J" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("K" + index).Value = t.CentroCosto?.Number;
                    ws1.Range("K" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //ws1.Range("L" + index).Value = t.Annalists?.Name;
                    //ws1.Range("L" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("M" + index).Value = t.Unit != null ? t.Unit.Name : "N/A";
                    ws1.Range("M" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //ws1.Range("N" + index).Value = t.AckRoutes;
                    //ws1.Range("N" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("O" + index).Value = t.SucursalRealiza != null ? t.SucursalRealiza.Name : "N/A";
                    ws1.Range("O" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("P" + index).Value = t.SucursalVende != null ? t.SucursalVende.Name : "N/A";
                    ws1.Range("P" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("Q" + index).Value = (t.Metodo.InternetPublish) ? "SI" : "NO";
                    ws1.Range("Q" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("R" + index).Value = t.Metodo.RequiredVolume ?? "N/A";
                    ws1.Range("R" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("S" + index).Value = t.Metodo.MinimumVolume ?? "N/A";
                    ws1.Range("S" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("T" + index).Value = t./*Metodo.*/Formula ?? "N/A";
                    ws1.Range("T" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("U" + index).Value = t.Metodo.DeliverTime ?? "N/A";
                    ws1.Range("U" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("V" + index).Value = t.Metodo.ReportTime ?? "N/A";
                    ws1.Range("V" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("W" + index).Value = t.Metodo.MaxTimeBeforeAnalysis ?? "N/A";
                    ws1.Range("W" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("X" + index).Value = t.Metodo.LabDeliverTime ?? "N/A";
                    ws1.Range("X" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("Y" + index).Value = t.Metodo.Container ?? "N/A";
                    ws1.Range("Y" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("Z" + index).Value = t.Metodo.Preserver ?? "N/A";
                    ws1.Range("Z" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AA" + index).Value = t.Metodo.Residue;
                    ws1.Range("AA" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    // ws1.Range("AB" + index).Value = t.Metodo.AnalyticsMethod;
                    // ws1.Range("AB" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AB" + index).Value = t.Metodo.DetectionLimit.Value == null ? "N/A" : t.Metodo.DetectionLimit.Value + "(" + t.Metodo.DetectionLimit.Decimals + ")";
                    ws1.Range("AB" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AC" + index).Value = t.Metodo.CuantificationLimit.Value == null ? "N/A" : t.Metodo.CuantificationLimit.Value + "(" + t.Metodo.CuantificationLimit.Decimals + ")";
                    ws1.Range("AC" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AD" + index).Value = t/*.Metodo*/.Uncertainty.Value == null ? "N/A" : t./*Metodo.*/Uncertainty.Value + "(" + t./*Metodo.*/Uncertainty.Decimals + ")";
                    ws1.Range("AD" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AE" + index).Value = (t./*Metodo.*/Qc.HasQc) ? "Si" : "No";
                    ws1.Range("AE" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AF" + index).Value = t./*Metodo.*/Qc.HasQc ? t./*Metodo.*/Qc.UpperLimit : "N/A";
                    ws1.Range("AF" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws1.Range("AG" + index).Value = t./*Metodo.*/Qc.HasQc ? t./*Metodo.*/Qc.LowerLimit : "N/A";
                    ws1.Range("AG" + index).Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;

                }

                ws1.Range("A" + index, "AG" + index).BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin);
                //var range = ws1.Range("A" + index, "AH" + index);
                //for (int i = 1; i < ws1.UsedRange.Columns.Count-1; i++)
                //{

                //    range.Cells[i].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                //        Excel.XlLineStyle.xlContinuous;
                //range.Cells[i].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                //     Excel.XlLineStyle.xlContinuous;
                //range.Cells[i].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                //    Excel.XlLineStyle.xlContinuous;
                //range.Cells[i].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;

                //ws1.Range("A" + index, "AJ" + index).Cells[i].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                //    Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells[i].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                //    Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells[i].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                //    Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells[i].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;

                // }
                //ws1.Range("A" + index, "AJ" + index)
                //   .BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                //    Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;
                //ws1.Range("A" + index, "AJ" + index).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                //   Excel.XlLineStyle.xlContinuous;
                index++;
            }

            #endregion


            GC.Collect();
            GC.WaitForPendingFinalizers();

            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(ws1);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        //-----------------------------------------------------------------------------------------------------------------------
        internal string Existelem(dynamic elem)
        {
            try
            {
                return elem;
            }
            catch (Exception)
            {
                return "";
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------
        internal static void DeleteWbSheet(Excel.Application excelApp, Excel._Worksheet ws)
        {
            excelApp.DisplayAlerts = false;

            var wsDel = ws;
            wsDel.Delete();

            excelApp.DisplayAlerts = true;
        }
        //-----------------------------------------------------------------------------------------------------------------------
        internal static void SaveAndCloseWorkbook(Excel.Application excelApp, Excel.Workbook wb, string wbPath)
        {
            if (System.IO.File.Exists(wbPath))
                try
                {
                    System.IO.File.Delete(wbPath);
                }
                catch (Exception e)
                {
                    var errmsg = e.Message;
                }

            if (wb == null) return;
            wb.SaveAs(wbPath);

            try
            {
                wb.Close();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wb);

                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                var idproc = GetIdProcces("EXCEL");

                if (idproc != -1)
                {
                    Process.GetProcessById(idproc).Kill();
                }
            }
            catch (Exception e)
            {
                var err = e.Message;
            }
        }
        internal static int GetIdProcces(string nameProcces)
        {

            try
            {
                var asProccess = Process.GetProcessesByName(nameProcces);

                foreach (var pProccess in asProccess.Where(pProccess => pProccess.MainWindowTitle == ""))
                {
                    return pProccess.Id;
                }

                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        internal static void ConvertExcelToPdf(string excelFileIn, string pdfFileOut)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Excel.Application excel;
            Excel.Workbook workbook;
            excel = new Excel.Application();

            if (System.IO.File.Exists(pdfFileOut))
                try
                {
                    System.IO.File.Delete(pdfFileOut);
                }
                catch (Exception e)
                {
                    var errMsg = e.Message;
                    //MessageBox.Show("No se puede crear el archivo PDF.\nVerifique que no se encuentre abierto por otra aplicación.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            try
            {
                /*excel.Visible = false;
                excel.ScreenUpdating = false;*/
                excel.DisplayAlerts = false;

                /*FileInfo excelFile = new FileInfo(excelFileIn);

                string filename = excelSystem.IO.File.FullName;*/

                workbook = excel.Workbooks.Open(excelFileIn, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing);
                //wbk.Activate();

                object outputFileName = pdfFileOut;
                var fileFormat = Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF;

                // Save document into PDF Format
                workbook.ExportAsFixedFormat(fileFormat, outputFileName,
                Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing,
                Type.Missing);

                object saveChanges = Microsoft.Office.Interop.Excel.XlSaveAction.xlDoNotSaveChanges;
                workbook.Close(saveChanges, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
                workbook = null;
            }
            catch (Exception)
            {

                //  MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                excel.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excel);
                workbook = null;
                excel = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        internal static void DeleteReportAllSheets(Excel.Application excelApp, Excel._Workbook wbReport)
        {
            DeleteWbSheet(excelApp, wbReport.Sheets["FRONTAL AS"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["FRONTAL MC"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["ATRAS MC"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["ALIMENTOS PREPARADOS"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["SUPERFICIES"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["MEDIO AMBIENTE"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["SUELOS"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["SEDIMENTO"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["RESIDUOS"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["LODOS"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["Dirigido"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["Aleatorio Simple"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["Sistemático"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["Estratificado"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["Por Atributos"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["CADENA"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["BITACORA"]);
            DeleteWbSheet(excelApp, wbReport.Sheets["EVIDENCIA"]);
        }
        static byte[] GetFile(string s)
        {
            var fs = System.IO.File.OpenRead(s);
            var data = new byte[fs.Length];
            var br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(s);
            return data;
        }
        //------------------------------------------------------------------------
        public List<dynamic> Getchildren(IEnumerable<dynamic> children, int parentId = 0, int level = 0)
        {
            try
            {
                var elemList = new List<dynamic>();
                foreach (dynamic ch in children.ToList())
                {
                    if (ch.elemType == "parameter")
                    {
                        elemList.Add(ch.ToJson(parentId, false, level));
                        continue;
                    }
                    elemList.Add(new
                    {
                        level,
                        ch.elemType,
                        ch.Id,
                        ch.Name
                    });
                    elemList.Add(Getchildren(ch.children, level + 1));
                }
                return elemList;
            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public List<dynamic> Getdata(bool viewParameters, bool viewGroups, bool viewPackages,
										int marketId = 0, int matrixId = 0, int sucursalId = 0)
        {
            try
            {
                int elemIndex = 0;
                int pkListCount = 0, gListCount = 0;

                var elemList = new List<dynamic>();

                //if (sucursalId != 0)
                //    sucursales = sucursales.Where(s => s.Id == sucursalId);

                //if (marketId != 0)
                //    sucursales = sucursales.Where(s => s.Office.Market.Id == marketId);

                dynamic currUsr = Session["curr_User"];
                int userId = currUsr.Id;

                var sucursales = User.Identity.Name == "root"
                    ? _dbStore.SucursalSet.Where(s => s.Active)
                    : _dbStore.SucursalSet.Where(s => s.Active &&
                                s.Users.Any(u => u.Id.Equals(userId)));





                //IEnumerable<Sucursal> sucursals = sucursales as IList<Sucursal> ?? sucursales.ToList();

                // ReSharper disable once CollectionNeverUpdated.Local
                //var packageChildren = new List<dynamic>();

                //const int skip = 0; //(page - 1) * pageSize;
                //const int pageSize = 100;
                //var packageCount = sucursals.Sum(s => s.Matrixes?
                //    .Where(m => m.Active && m.Packages.Any()
                //    && (matrixId == 0 || m.BaseMatrix.Id == matrixId))
                //    .Sum(m => m.Packages.Count(pk => pk.Active
                //        && (pk.Groups.Any(g => g.Parameters.Any(p => p.Active &&
                //            (methodId == 0 || p.Metodo.Id == methodId))) ||
                //            pk.Parameters.Any(p => p.Active &&
                //            (methodId == 0 || p.Metodo.Id == methodId)))))) ?? 0;

                //---------------------new----------------------------------------------------
				//TODO: Revisar bien consulta para que funcione bien en todos los casos.


                var matrixList = _dbStore.MatrixSet.Where(m => m.Active && (matrixId == 0 || m.BaseMatrix.Id == matrixId) &&
                                                               m.Parameters.Any(p => p.Active &&
                                                                                    sucursales.Any(
                                                                                        s => s.Active &&
                                                                                            (p.SucursalRealiza != null &&
                                                                                            s.Id.Equals(
                                                                                                p.SucursalRealiza.Id) ||
                                                                                            p.SucursalVende != null &&
                                                                                            s.Id.Equals(
                                                                                                p.SucursalVende.Id)))));


                //var packList = _dbStore.PackageSet.Where(pk => pk.Active && viewPackages &&
                //                                            (pk.Parameters.Any(p =>p.Active && 
                //                                                                  (p.SucursalVende != null &&
                //                                                                  (sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)))) ||
                //                                                                  (p.SucursalRealiza != null && 
                //                                                                  (sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))))) ||
                //                                            pk.Groups.Any(g => g.Active && 
                //                                                               g.Parameters.Any(p => p.Active && 
                //                                                                                    (p.SucursalVende != null &&
                //                                                                                    (sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id)))) ||
                //                                                                                    (p.SucursalRealiza != null && 
                //                                                                                    (sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))))))))
                //                                .OrderBy(pk => pk.Name).ToList()?
                //                                .Select(pk => pk.ToMiniJson(_dbStore.ParamRouteSet.Where(pr => pr.Package != null && 
                //                                                                                               pk.Id == pr.Package.Id).AsEnumerable()));

                //var groupList = _dbStore.GroupSet.Where(g => g.Active && viewGroups &&
                //                                             g.Parameters.Any(p => p.Active &&
                //                                                                  (p.SucursalVende != null && 
                //                                                                   sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id))) ||
                //                                                                   p.SucursalRealiza != null && 
                //                                                                   sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id))))
                //                                       .OrderBy(g => g.Name).ToList()
                //                                       .Select(g => g.ToMiniJson(_dbStore.ParamRouteSet.Where(pro => pro.Package == null && 
                //                                                                                                     pro.Group != null && 
                //                                                                                                     pro.Group.Id == g.Id).AsEnumerable()));

                //var paramList = _dbStore.ParamSet.Where(p => p.Active && viewParameters &&
                //                                             (p.SucursalVende != null && 
                //                                             sucursales.Any(s => s.Id.Equals(p.SucursalVende.Id))) ||
                //                                             p.SucursalRealiza != null && 
                //                                             sucursales.Any(s => s.Id.Equals(p.SucursalRealiza.Id)))

                //                                  .OrderBy(p => p.BaseParam.Name).ToList()
                //                                  .Select(p => p.ToMiniJson(
                //                                            _dbStore.ParamRouteSet.FirstOrDefault(
                //                                                pr => pr.Package == null && 
                //                                                pr.Group == null &&
                //                                                    pr.Parameter.Id == p.Id), 0));

                foreach (var mtrx in matrixList.ToList())
                {
                    elemList.Add(new
                    {
                        elemType = "matrix",
                        level = 0,
                        mtrx.BaseMatrix.Name
                    });
                    if (viewPackages)
                        foreach (var pk in mtrx.Packages.ToList())
                        {
                            elemList.Add(new
                            {
                                elemType = "package",
                                level = 1,
                                pk.Name,
                                pk.Description,
                                Precio = new
                                {
                                    Value = pk.Groups.Where(g => g.Active).Sum(g => g.Parameters.Sum(p => p.Precio.Value)) + pk.Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
                                    Currency = new
                                    {
                                        Name = pk.Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ??
                                              pk.Groups.FirstOrDefault(g => g.Active)?.Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                                    }
                                }
                            });
                            foreach (var g in pk.Groups.ToList())
                            {
                                elemList.Add(new
                                {
                                    elemType = "group",
                                    level = 2,
                                    g.Name,
                                    g.Description,
                                    Precio = new
                                    {
                                        Value = g.Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
                                        Currency = new
                                        {
                                            Name = g.Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                                        }
                                    }
                                });
                                elemList.AddRange(g.Parameters.Where(p => p.Active).ToList()
                            .Select(p => ParamToJson(p, 3, g.Id, null/*g.DecimalesReporte*/, 0,
                                                   _dbStore.ParamRouteSet.FirstOrDefault(pr =>
                                                                                            pr.Parameter.Id == p.Id &&
                                                                                            pr.Group != null &&
                                                                                            pr.Group.Id == g.Id &&
                                                                                            pr.Package != null &&
                                                                                            pr.Package.Id == pk.Id))/*ParamToJson(p, 3)*/));
                            }
                            elemList.AddRange(pk.Parameters.Where(p => p.Active).ToList()
                            .Select(p => ParamToJson(p, 2, 0, pk.DecimalesReporte, 0, _dbStore.ParamRouteSet.FirstOrDefault(pr =>
                                                                                            pr.Parameter.Id == p.Id &&
                                                                                            pr.Group == null &&
                                                                                            pr.Package != null &&
                                                                                            pr.Package.Id == pk.Id))/*ParamToJson(p, 2)*/));
                        }
                    if (viewGroups)
                        foreach (var g in mtrx.Groups.ToList())
                        {
                            elemList.Add(new
                            {
                                elemType = "group",
                                level = 1,
                                g.Name,
                                g.Description,
                                Precio = new
                                {
                                    Value = g.Parameters.Where(p => p.Active).Sum(p => p.Precio.Value),
                                    Currency = new
                                    {
                                        Name = g.Parameters.FirstOrDefault(p => p.Active)?.Precio.Currency.Name ?? "MX"
                                    }
                                }
                            });
                            elemList.AddRange(g.Parameters.Where(p => p.Active)
                             .Select(p => ParamToJson(p, 2, 0, null/*g.DecimalesReporte*/, 0, _dbStore.ParamRouteSet.FirstOrDefault(pr =>
                                                                                            pr.Parameter.Id == p.Id &&
                                                                                            pr.Group != null &&
                                                                                            pr.Group.Id == g.Id &&
                                                                                            pr.Package == null))/*ParamToJson(p, 2)*/));
                        }
                    if (viewParameters)
                        elemList.AddRange(mtrx.Parameters.Where(p => p.Active)
                            .Select(p => ParamToJson(p, 1, 0, null, 0, _dbStore.ParamRouteSet.FirstOrDefault(pr =>
                                                                                            pr.Parameter.Id == p.Id &&
                                                                                            pr.Group == null &&
                                                                                            pr.Package == null))/*ParamToJson(p, 1)*/));
                }

                return elemList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //public List<dynamic> MatrixToJsonList(Matrix matrix, int level)
        //{
        //    var elemList = new List<dynamic>
        //    {
        //        new
        //        {
        //            elemType = "matrix",
        //            level,
        //            @matrix.BaseMatrix.Name
        //        },

        //    };

        //    //var pkts = matrix.Packages.Where(pk => pk.Active).Select(pk => PackToJsonList(pk, level + 1));
        //    //var gr = matrix.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1, null));
        //    //var par = matrix.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1));

        //    //elemList= elemList.Concat(pkts)
        //    //    .Concat(gr)
        //    //    .Concat(par).ToList();
        //    elemList.AddRange(matrix.Packages.Where(pk => pk.Active).Select(pk => PackToJsonList(pk, level + 1)));
        //    elemList.AddRange(matrix.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1)));
        //    elemList.AddRange(matrix.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, 
        //                                                                                        level + 1,
        //                                                                                        0,
        //                                                                                        null,
        //                                                                                        0,
        //                                     _dbStore.ParamRouteSet.FirstOrDefault(pr => pr.Parameter.Id == p.Id &&
        //                                                                                        pr.Group == null &&
        //                                                                                        pr.Package == null))));

        //    return elemList;
        //    //return elemList.Concat(
        //    //    matrix.Packages.Where(pk => pk.Active).Select(pk => PackToJsonList(pk, level + 1))).ToList()
        //    //    .Concat(matrix.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1))).ToList()
        //    //    .Concat(matrix.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1))).ToList();
        //}

        //public List<dynamic> PackToJsonList(Package pack, int level)
        //{
        //    var elemList = new List<dynamic>
        //    {
        //        new
        //        {
        //            elemType = "package",
        //            level,
        //            @pack.Name
        //        }
        //    };

        //    //var gr = pack.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1)).ToList();
        //    //var par=pack.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1)).ToList();

        //    //elemList= elemList.Concat(gr)
        //    //               .Concat(par).ToList();
        //    elemList.AddRange(pack.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1, pack)));
        //    elemList.AddRange(pack.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, 
        //                                                                                    level + 1, 
        //                                                                                    0, 
        //                                                                                    pack.DecimalesReporte,
        //                                                                                    0,
        //                                    _dbStore.ParamRouteSet.FirstOrDefault(pr => pr.Parameter.Id == p.Id &&
        //                                                                                        pr.Group == null &&
        //                                                                                        pack != null &&
        //                                                                                        pr.Package.Id == pack.Id))));
        //    return elemList;

        //    //return elemList.Concat(pack.Groups.Where(g => g.Active).Select(g => GroupToJsonList(g, level + 1))).ToList()
        //    //                .Concat(pack.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1))).ToList();
        //}
        //public List<dynamic> GroupToJsonList(Group group, int level, Package package=null)
        //{
        //    var elemList = new List<dynamic>
        //    {
        //        new
        //        {
        //            elemType = "group",
        //            level,
        //            @group.Name
        //        }
        //    };
        //    //var par = group.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1)).ToList();
        //    elemList.AddRange(group.Parameters.Where(p => p.Active).Select(p => ParamToJson(p, level + 1, 
        //                                                                                       group.Id, 
        //                                                                                       group.DecimalesReporte, 0,
        //                            _dbStore.ParamRouteSet.FirstOrDefault(pr=>pr.Parameter.Id==p.Id &&
        //                                                                      pr.Group!=null && 
        //                                                                      pr.Group.Id==group.Id &&
        //                                                                      package!=null &&
        //                                                                      pr.Package.Id==package.Id))));
        //    return elemList;
        //    //return elemList.Concat(par).ToList();
        //    //return elemList.Concat(group.Parameters.Where(p => p.Active).ToList().Select(p => ParamToJson(p, level + 1))).ToList();

        //}

        public dynamic ParamToJson(Param param, int level = 0, int grupoId = 0,
            int? DReporte = null, int numeroMuestreo = 0, ParamRoute paramRoute = null)
        {
            var routList = param.RecOtorgs.Where(ro => ro.Enterprise != null)
				.Select(ro => ro.ToString() + ", ");
            string routes = routList.Aggregate("", (current, r) => current + (r));

	        var annalistsList = param.Annalists.Select(a => a.ToString() + ",");
	        string annalists = annalistsList?.Aggregate("", (current, r) => current + (r));

            var printResult = grupoId == 0
                ? _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group == null && prr.Parameter.Id == param.Id)
                : _dbStore.ParamPrintResultSet.FirstOrDefault(prr => prr.Group != null && prr.Group.Id == grupoId && prr.Parameter.Id == param.Id);
            var dispParamsId = (grupoId != 0) ? _dbStore.GroupSet.Find(grupoId).DispParamId : 0;

			return new
            {

                elemType = "parameter",
                level,
                param.Id,
                param.Active,
                Name = param.ParamUniquekey,  // para mostrar como nombre en la tabla
                param.Description,
                MaxPermitedLimit = paramRoute != null ? new { Value = paramRoute.Value } : null,
                param.PerTurnCapacity,
                param.PerWeekCapacity,
                ParamPrintResults = printResult != null ? new { printResult.Id, printResult.Yes } : null,
                param.AutolabAssignedAreaName,
                param.GenericKeyForStatistic,
                Precio = new { param.Precio.Value, Currency = new { param.Precio.Currency.Name } },
              //  param.GenericDescription,
                // param.GenericKey,
                param.Rama,
                DecimalesReporte = param.DecimalesReporte ?? DReporte,
                param.ResiduoPeligroso,
                param.ReportaCliente,
                param.PublishInAutolab,
                param.SellSeparated,
                param.CuentaEstadistica,
                param.Formula,
                Qc = new
                {
                    param.QcObj.HasQc,
                    param.QcObj.UpperLimit,
                    param.QcObj.LowerLimit
                },
                AnalyticsMethod = param.AnalyticsMethod?.Name,
                Uncertainty = new
                {
                    param.Uncertainty.Value,
                    param.Uncertainty.Decimals
                },
                Week = param.Week.ToJson(),
                ClasificacionQuimica1 = param.BaseParam.ClasificacionQuimica1?.ToJson(),
                ClasificacionQuimica2 = param.BaseParam.ClasificacionQuimica2?.ToJson(),
                ClasificacionQuimica3 = param.BaseParam.ClasificacionQuimica3?.ToJson(),
                DispParam = (dispParamsId != 0) ? new { Name = _dbStore.ParamSet.Find(dispParamsId).ParamUniquekey, Id = _dbStore.ParamSet.Find(dispParamsId).Id } : null,
                //BaseParam = new
                //{
                //    param.BaseParam.Id,
                //    param.BaseParam.Name,
                //    param.BaseParam.Description
                //},
                CentroCosto = (param.CentroCosto != null)
                    ? new
                {
                        param.CentroCosto.Id,
                        param.CentroCosto.Number,
						param.CentroCosto.AreasAnaliticas.FirstOrDefault(a => a.Active)?.Key
                }
                    : null,
                Signatarios = param.Annalists.Where(a => a.Active && a.RecAdqs
                    .Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Signatario)
                    && ra.RecOtorgs.Any(ro => ro.Enterprise.Tipo
                    && ro.Params.Any(p => p.Id.Equals(param.Id))))/*&& a.AnnalistKey!=null*/).Select(a => a.Key/*a.AnnalistKey.Clave*/),
                Eidas = param.Annalists.Where(a => a.Active && a.RecAdqs
                    .Any(ra => ra.NivelAdquirido.Equals(RecAdq.AcquiredLevel.Eidas)
                    && ra.RecOtorgs.Any(ro => !ro.Enterprise.Tipo
                    && ro.Params.Any(p => p.Id.Equals(param.Id)))) /*&& a.AnnalistKey != null*/).Select(a => a.Key/*a.AnnalistKey.Clave*/),
                Annalists = param.Annalists?.Select(a => new
                {
                    a.Id,
                    Name = a.ToString(),
                    a.Key// Key=a.AnnalistKey!=null? a.AnnalistKey.Clave:""
                }),
				param.AnalysisTime,
                Metodo = (param.Metodo != null)
                    ? MethodToJson(param.Metodo)
                    //new
                    //{
                    //    //  param.Metodo.Id,
                    //    param.Metodo.Name,
                    //    AnalyticsMethod = param.Metodo.AnalyticsMethod?.Name,
                    //    param.Metodo.InternetPublish,
                    //    param.Metodo.DeliverTime,
                    //    param.Metodo.AnalysisTime,
                    //    Container=param.Metodo.Container.Name,
                    //    param.Metodo.LabDeliverTime,
                    //    param.Metodo.Formula,
                    //    CuantificationLimit=param.Metodo.CuantificationLimit.Value,
                    //    param.Metodo.MaxTimeBeforeAnalysis,
                    //    param.Metodo.MinimumVolume,
                    //    Preserver=param.Metodo.Preserver.Name,
                    //    param.Metodo.QcObj.LowerLimit,
                    //    param.Metodo.QcObj.UpperLimit,
                    //    param.Metodo.RequiredVolume,
                    //    param.Metodo.Residue



                    //}
                    : null,
                SucursalVende = (param.SucursalVende != null) ? new { param.SucursalVende.Id, param.SucursalVende.Name } : null,
                SucursalRealiza = (param.SucursalRealiza != null) ? new { param.SucursalRealiza.Id, param.SucursalRealiza.Name } : null,
                Unit = param.Unit != null ? new { param.Unit.Name } : null,
                RecOtorgs = param.RecOtorgs.Where(ro => ro.Enterprise != null).Select(ro => ro.ToJson()),
                TipoServicio = param.TipoServicio?.ToJson(),
                Matrix = param.Matrixes?.FirstOrDefault().Id,
                Groups = param.Groups?.Where(g => g.Active).Select(g => new { g.Id, g.Name }),
                Packages = param.Packages?.Where(pk => pk.Active).Select(pk => new { pk.Id, pk.Name }),
                //param.ComplexSamplings.FirstOrDefault(cs => cs.Group.Id.Equals(grupoId))?.CantidadMuestreos
            };
        }
        public dynamic MethodToJson(Method method)
        {
            return new
            {
                method.Id,
                method.Name,
                //method.RequiredVolume,
                //method.MinimumVolume,
              //  method.Formula,
                //method.InternetPublish,
                //method.DeliverTime,
                //DetectionLimit = new
                //{
                //    method.DetectionLimit.Value,
                //    method.DetectionLimit.Decimals
                //},
                //CuantificationLimit = new
                //{
                //    method.CuantificationLimit.Value,
                //    method.CuantificationLimit.Decimals
                //},
                //Uncertainty = new
                //{
                //    method.Uncertainty.Value,
                //    method.Uncertainty.Decimals
                //},
                //method.MaxTimeBeforeAnalysis,
                //method.LabDeliverTime,
                //Qc = new
                //{
                //    method.QcObj.HasQc,
                //    method.QcObj.UpperLimit,
                //    method.QcObj.LowerLimit
                //},
                //method.ReportTime,
                ////method.AnalysisTime,
                //Container = method.Container?.Name,
                //Preserver = method.Preserver?.Name,
               // Residue = method.Residue?.Name,
             //   AnalyticsMethod = method.AnalyticsMethod?.Name
            };
        }
    }
}