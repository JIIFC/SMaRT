using System.Data;
using System.Data.SqlClient;

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMARTV3.Helpers;
using SMARTV3.Models;
using SMARTV3.Security;

using static Constants;

namespace SMARTV3.Controllers
{
    [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + ReadOnlyUser)]
    public class ReportGeneratorController : Controller
    {
        private readonly SMARTV3DbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ForcePackageHelper forcePackageHelper;
        public const int dataFontSize = 12;
        private ReportPocinformation? reportPOC;
        private XLWorkbook workbook = new();
        private IXLWorksheet? contentWS;
        private IXLWorksheet? validationWS;
        private int currentExcelRow;
        private string? currentReportType;
        private readonly int PetsOverallGreenId;

        public ReportGeneratorController(SMARTV3DbContext db, IConfiguration configuration)
        {
            _context = db;
            _configuration = configuration;
            forcePackageHelper = new(_context);
            PetsOverallGreenId = _context.PetsoverallStatuses.Where(s => s.StatusDisplayValue == "90% +").First().Id;
        }

        public async Task<IActionResult> CreateForcePackageWeightReport(string? serializedForcePackageIds)
        {
            XLWorkbook workbook = new();
            IXLWorksheet? totalWS = workbook.Worksheets.Add("Force Package Total Weight");
            IXLWorksheet? otherWS = workbook.Worksheets.Add("Force Package Other Weight");
            IXLWorksheet? fpIdWS = workbook.Worksheets.Add("Force Package Id's");
            IXLWorksheet? fpDescWS = workbook.Worksheets.Add("Force Package Description");

            List<int>? deserializedFPIds = JsonConvert.DeserializeObject<List<string>>(serializedForcePackageIds)?.Select(int.Parse).ToList();
            int primaryFPID = deserializedFPIds.FirstOrDefault();

            List<int> forcePackageIds = _context.ForcePackages
                .Include(f => f.DummyForceElements)
                .Where(f => f.DummyForceElements.Count > 0 && deserializedFPIds.Contains(f.Id))
                .Select(s => s.Id).ToList();

            ForcePackageWeightHelper forcePackageWeightHelper = new(deserializedFPIds);
            List<List<int>> forcePackageCombinations = forcePackageWeightHelper.CalculateCombinationsPrimary(primaryFPID);

            int rowColSize = (int)Math.Round(Math.Sqrt(forcePackageCombinations.Count));
            int row = 1;
            int col = 1;
            int descRow = 1;
            int sheetCount = 1;
            

            fpDescWS.Cell(descRow, 1).Value = "FP_ID";
            fpDescWS.Cell(descRow, 2).Value = "FPC_Permutation";
            fpDescWS.Cell(descRow, 3).Value = "FPC_Group";
            fpDescWS.Cell(descRow, 4).Value = "FPC_PkgNum";
            fpDescWS.Cell(descRow, 5).Value = "FPC_PkgName";
            fpDescWS.Cell(descRow, 6).Value = "FPC_PkgDesc";
            fpDescWS.Cell(descRow, 7).Value = "FPC_Shared";
            fpDescWS.Cell(descRow, 8).Value = "Shared_By_PkgNum";
            fpDescWS.Cell(descRow, 9).Value = "Shared_By_PkgNam";
            fpDescWS.Cell(descRow, 10).Value = "ElementID";
            fpDescWS.Cell(descRow, 11).Value = "ElementName";
            fpDescWS.Cell(descRow, 12).Value = "Weight";
            fpDescWS.Cell(descRow, 13).Value = "OrgName";
            fpDescWS.Cell(descRow, 14).Value = "Override_Readiness_Status";
            fpDescWS.Cell(descRow, 15).Value = "FPC_Override_Readiness_Status";
            fpDescWS.Cell(descRow, 16).Value = "Deployed_Status";
            fpDescWS.Cell(descRow, 17).Value = "Pers_Status";
            fpDescWS.Cell(descRow, 18).Value = "Equip_Status";
            fpDescWS.Cell(descRow, 19).Value = "Training_Status";
            fpDescWS.Cell(descRow, 20).Value = "Sustain_Status";

            using (SqlConnection connection = new(_configuration.GetValue<string>("ConnectionStrings:SMARTConnectionString")))
            {
                await connection.OpenAsync();

                foreach (List<int> combination in forcePackageCombinations)
                {
                    
                    string strCombination = string.Join(",", combination);
                    fpIdWS.Cell(row, col).Value = strCombination;
                    List<int> combiFPNum = new List<int>(combination);
                    
                    for (int i = 0; i < combiFPNum.Count(); i++)
                    {
                        combiFPNum[i] = deserializedFPIds.FindIndex(x => x.Equals(combiFPNum[i])) + 1;
                    }
                    if (combiFPNum.Exists(x => x.Equals(1)))
                    {
                        combiFPNum.Sort();
                        string strCombiFPNum = string.Join(";", combiFPNum);
                        List<Tuple<string, int, string>> felms = new List<Tuple<string, int, string>>();
                        if (descRow < 1000000)
                        {
                            SqlCommand descWeightCmd = new(descWeightQuery + strCombination +descWeightMiddleQuery + strCombination + CreateSortOrder(combination), connection);
                            SqlDataReader descReader = await descWeightCmd.ExecuteReaderAsync();
                            if (descReader.HasRows)
                            {

                                while (await descReader.ReadAsync())
                                {
                                    descRow++;
                                    fpDescWS.Cell(descRow, 1).Value = descReader.GetInt32(0);
                                    fpDescWS.Cell(descRow, 2).Value = strCombiFPNum;
                                    fpDescWS.Cell(descRow, 3).Value = combination.Count().ToString();
                                    fpDescWS.Cell(descRow, 4).Value = deserializedFPIds.FindIndex(x => x.Equals(descReader.GetInt32(0))) + 1;
                                    fpDescWS.Cell(descRow, 5).Value = descReader.GetString(1);
                                    try
                                    {
                                        fpDescWS.Cell(descRow, 6).Value = descReader.GetString(2);
                                    }
                                    catch (Exception e)
                                    {
                                        fpDescWS.Cell(descRow, 6).Value = "";
                                    }
                                    if (descReader.GetInt32(14) > 1 && felms.Where(x => x.Item1.Equals(descReader.GetString(3))).Count() > 0)
                                    {
                                        fpDescWS.Cell(descRow, 7).Value = 2;
                                    }else if (descReader.GetInt32(14) > 1)
                                    {
                                        fpDescWS.Cell(descRow, 7).Value = 1;
                                    }
                                    else
                                    {
                                        fpDescWS.Cell(descRow, 7).Value = 0;
                                    }
                                    if (fpDescWS.Cell(descRow, 7).Value.Equals(2))
                                    {
                                        fpDescWS.Cell(descRow, 8).Value = felms.Where(x => x.Item1.Equals(descReader.GetString(3))).First().Item2;
                                        fpDescWS.Cell(descRow, 9).Value = felms.Where(x => x.Item1.Equals(descReader.GetString(3))).First().Item3;
                                    }
                                    else
                                    {
                                        fpDescWS.Cell(descRow, 8).Value = "";
                                        fpDescWS.Cell(descRow, 9).Value = "";
                                    }
                                    fpDescWS.Cell(descRow, 10).Value = descReader.GetString(3);
                                    fpDescWS.Cell(descRow, 11).Value = descReader.GetString(4);
                                    fpDescWS.Cell(descRow, 12).Value = descReader.GetDecimal(5);
                                    fpDescWS.Cell(descRow, 13).Value = descReader.GetString(6);
                                    fpDescWS.Cell(descRow, 14).Value = getStatus(descReader.GetString(7));
                                    fpDescWS.Cell(descRow, 15).Value = getStatus(descReader.GetString(7));
                                    if (!descReader.GetSqlString(8).IsNull)
                                    {
                                        fpDescWS.Cell(descRow, 14).Value = getStatus(descReader.GetString(8));
                                        fpDescWS.Cell(descRow, 15).Value = getStatus(descReader.GetString(8));
                                    }
                                    fpDescWS.Cell(descRow, 16).Value = descReader.GetString(9);
                                    fpDescWS.Cell(descRow, 17).Value = descReader.GetString(10);
                                    fpDescWS.Cell(descRow, 18).Value = descReader.GetString(11);
                                    fpDescWS.Cell(descRow, 19).Value = descReader.GetString(12);
                                    fpDescWS.Cell(descRow, 20).Value = descReader.GetString(13);
                                    if (fpDescWS.Cell(descRow, 7).Value.Equals(2))
                                    {
                                        fpDescWS.Cell(descRow, 15).Value = getStatus("Red");
                                        fpDescWS.Cell(descRow, 17).Value = "< 60%";
                                        fpDescWS.Cell(descRow, 18).Value = "< 60%";
                                        fpDescWS.Cell(descRow, 19).Value = "< 60%";
                                        fpDescWS.Cell(descRow, 20).Value = "< 60%";
                                    }
                                    //else
                                    //{
                                        //fpDescWS.Cell(descRow, 15).Value = getStatus(descReader.GetString(7));
                                    //}                                    
                                    felms.Add(Tuple.Create(descReader.GetString(3), deserializedFPIds.FindIndex(x => x.Equals(descReader.GetInt32(0))) + 1, descReader.GetString(1)));
                                }
                                await descReader.CloseAsync();
                            }
                        }
                        else
                        {
                            string sheetname = "Force Package Description Con " + sheetCount.ToString();
                            fpDescWS = workbook.Worksheets.Add(sheetname);
                            descRow = 1;
                            sheetCount++;
                            fpDescWS.Cell(descRow, 1).Value = "FP_ID";
                            fpDescWS.Cell(descRow, 2).Value = "FPC_Permutation";
                            fpDescWS.Cell(descRow, 3).Value = "FPC_Group";
                            fpDescWS.Cell(descRow, 4).Value = "FPC_PkgNum";
                            fpDescWS.Cell(descRow, 5).Value = "FPC_PkgName";
                            fpDescWS.Cell(descRow, 6).Value = "FPC_PkgDesc";
                            fpDescWS.Cell(descRow, 7).Value = "FPC_Shared";
                            fpDescWS.Cell(descRow, 8).Value = "Shared_By_PkgNum";
                            fpDescWS.Cell(descRow, 9).Value = "Shared_By_PkgNam";
                            fpDescWS.Cell(descRow, 10).Value = "ElementID";
                            fpDescWS.Cell(descRow, 11).Value = "ElementName";
                            fpDescWS.Cell(descRow, 12).Value = "Weight";
                            fpDescWS.Cell(descRow, 13).Value = "OrgName";
                            fpDescWS.Cell(descRow, 14).Value = "Override_Readiness_Status";
                            fpDescWS.Cell(descRow, 15).Value = "FPC_Override_Readiness_Status";
                            fpDescWS.Cell(descRow, 16).Value = "Deployed_Status";
                            fpDescWS.Cell(descRow, 17).Value = "Pers_Status";
                            fpDescWS.Cell(descRow, 18).Value = "Equip_Status";
                            fpDescWS.Cell(descRow, 19).Value = "Training_Status";
                            fpDescWS.Cell(descRow, 20).Value = "Sustain_Status";
                        }
                    }
                    
                    SqlCommand totalWeightCmd = new(totalWeightQuery + strCombination + ");", connection);
                    SqlDataReader totalReader = await totalWeightCmd.ExecuteReaderAsync();
                    if (totalReader.HasRows)
                    {
                        while (await totalReader.ReadAsync())
                        {
                            try
                            {
                                decimal tempVal = totalReader.GetDecimal(0);
                                totalWS.Cell(row, col).Value = tempVal;
                            }
                            catch (Exception e)
                            {
                                totalWS.Cell(row, col).Value = 0;
                            };
                        }
                    }
                    await totalReader.CloseAsync();

                    SqlCommand otherWeightCmd = new(combiWeightQuery1 + strCombination + combiWeightQuery2, connection);
                    SqlDataReader otherReader = await otherWeightCmd.ExecuteReaderAsync();
                    if (otherReader.HasRows)
                    {
                        while (await otherReader.ReadAsync())
                        {
                            try
                            {
                                decimal tempVal = otherReader.GetDecimal(0);
                                otherWS.Cell(row, col).Value = tempVal;
                            }
                            catch (Exception e)
                            {
                                otherWS.Cell(row, col).Value = 0;
                            };
                        }
                    }
                    await otherReader.CloseAsync();
                    col++;
                    if (col > rowColSize)
                    {
                        row++;
                        col = 1;
                    }
                }
                await connection.CloseAsync();
            }

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);

            return File(stream.ToArray(), applicationTypeSpreadsheet, "ForcePackageWeight.xlsx");
        }

        public IActionResult CreateHighDemandReport(string? serializedForcePackageIds)
        {
            int row = 1;
            List<ForcePackage> fpList = forcePackageHelper.CreateFPListFromSerialized(serializedForcePackageIds);
            XLWorkbook workbook = new();
            IXLWorksheet? contentWS = workbook.Worksheets.Add("High Demand Low Density");
            SetWorksheetSizing(contentWS, "SECRET AUS/CAN/NZ/UK/ EYES ONLY");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.PageNumber);
            contentWS!.PageSetup.Footer.Right.AddText("/");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.NumberOfPages);
            contentWS!.PageSetup.ShowGridlines = true;

            var dummyForceElements = forcePackageHelper.GetDummyForceElementsQueryWIncludes()
                .Where(df => fpList.Contains(df.ForcePackage) && df.IsActiveInForcePackage && df.IsTiedToRealFelm).ToList()
                .GroupBy(df => df.ForceElementId).OrderByDescending(a => a.Count());

            contentWS.Column(1).Width = 80;
            contentWS.Column(2).Width = 40;
            contentWS.Column(3).Width = 80;

            contentWS.Cell(row, 1).Value = "Force Element";
            contentWS.Cell(row, 2).Value = "Demand Instances \n(Number of Force Packages found)";
            contentWS.Cell(row, 2).Style.Alignment.WrapText = true;
            contentWS.Cell(row, 3).Value = "Force Package Assignments";
            IXLRange headerRange = contentWS.Range(contentWS.Cell(row, 1).Address, contentWS.Cell(row, 3).Address);
            headerRange.Style.Fill.BackgroundColor = XLColor.Yellow;
            headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headerRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            headerRange.Style.Font.SetBold(true);
            row++;

            foreach (var dummyForceElement in dummyForceElements)
            {
                contentWS.Range(contentWS.Cell(row, 1).Address, contentWS.Cell(row, 3).Address).Style.Alignment.WrapText = true;
                contentWS.Cell(row, 1).Value = dummyForceElement.FirstOrDefault()!.ForceElement!.ElementName;
                contentWS.Cell(row, 2).Value = dummyForceElement.Count();
                string forcePackageNames = "";
                for (int i = 0; i < dummyForceElement.Count(); i++)
                {
                    forcePackageNames += dummyForceElement.ElementAt(i).ForcePackage.ForcePackageName;
                    if (i < dummyForceElement.Count() - 1)
                    {
                        forcePackageNames += ", ";
                    }
                }
                contentWS.Cell(row, 3).Value = forcePackageNames;
                row++;
            }

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);

            return File(stream.ToArray(), applicationTypeSpreadsheet, "HighDemandLowDensity.xlsx");
        }

        public IActionResult CreateAnnexBReport()
        {
            int row = 1;
            XLWorkbook workbook = new();
            IXLWorksheet? contentWS = workbook.Worksheets.Add("Annex B");
            SetWorksheetSizing(contentWS, "SECRET//REL TO CAN, FVEY");
            contentWS!.PageSetup.Footer.Right.AddText("B-");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.PageNumber);
            contentWS!.PageSetup.Footer.Right.AddText("/");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.NumberOfPages);
            IXLCell annexBCell = contentWS.Cell(row, 1);
            annexBCell.Value = "Annex B to the CDS Directive for CAF FP&R";
            annexBCell.Style.Font.Bold = true;
            contentWS.Range("A1:D1").Row(1).Merge();
            row += 2;

            IXLCell felmReqCell = contentWS.Cell(row++, 1);
            felmReqCell.Value = "Force Elements Required to Support CAF Core Missions";
            felmReqCell.Style.Font.Bold = true;
            contentWS.Range("A3:D3").Row(1).Merge();

            IXLRange detectDeterRange = contentWS.Range("G4:P4");
            IXLRange natoCommitRange = contentWS.Range("R4:V4");
            IXLRange firstHeaderRow = contentWS.Range("G4:V4");

            detectDeterRange.Row(1).Merge();
            natoCommitRange.Row(1).Merge();

            IXLCell detectDeterCell = contentWS.Cell(row, 7);
            detectDeterCell.Value = "Detect, Deter and Defend Canada; Respond to: Domestic Disasters, ALEA, and SAR";
            detectDeterCell.Style.Font.Bold = true;

            firstHeaderRow.Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
            firstHeaderRow.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            firstHeaderRow.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            detectDeterRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            detectDeterRange.Style.Border.OutsideBorderColor = XLColor.Black;

            IXLCell defendCell = contentWS.Cell(row, 17);
            defendCell.Value = "Defend North America";
            defendCell.Style.Font.Bold = true;
            defendCell.Style.Alignment.WrapText = true;
            defendCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            defendCell.Style.Border.OutsideBorderColor = XLColor.Black;

            natoCommitRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            natoCommitRange.Style.Border.OutsideBorderColor = XLColor.Black;

            IXLCell natoCommitCell = contentWS.Cell(row++, 18);
            natoCommitCell.Value = "Meet NATO commitments; Contribute to Int'l Peace and Stability; and Respond to Int'l Disasters";
            natoCommitCell.Style.Font.Bold = true;
            natoCommitCell.Style.Alignment.WrapText = true;

            for (int i = 1; i <= annexeBHeaderTitles.Length; i++)
            {
                contentWS.Cell(row, i).Value = annexeBHeaderTitles[i - 1];
                contentWS.Cell(row, i).Style.Alignment.WrapText = true;
                if (i == StatusCol || i == OpSuppElemCol || i == PresenceCol || i == RenaissanceCol)
                {
                    contentWS.Column(i).Width = 12;
                }
                else if (i == ForceGenCol)
                {
                    contentWS.Column(i).Width = 14;
                }
                else if (i == NtmCol)
                {
                    contentWS.Column(i).Width = 20;
                }
                else if (i == FelmNameCol)
                {
                    contentWS.Column(i).Width = 40;
                }
            }

            IXLRange secondHeaderRow = contentWS.Range(contentWS.Cell(row, 1).Address, contentWS.Cell(row++, 22).Address);
            secondHeaderRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            secondHeaderRow.Style.Font.Bold = true;
            secondHeaderRow.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            secondHeaderRow.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            secondHeaderRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            secondHeaderRow.Style.Border.OutsideBorderColor = XLColor.Black;
            secondHeaderRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            secondHeaderRow.Style.Border.InsideBorderColor = XLColor.Black;

            List<ForceElement>? forceElements = _context.ForceElements.Include(d => d.Organization)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Conplans)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Designation)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Echelon)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.NoticeToMove)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Operations)
                                                                      .Where(d => d.Archived == false && d.Aspirational == false && d.DataCards.Any())
                                                                      .OrderBy(f => f.ElementId)
                                                                      .ToList();
            if (!forceElements.Any())
            {
                return null;
            }

            contentWS = FillAnnexBData(forceElements, contentWS, row, false);
            row += forceElements.Count;

            contentWS.SheetView.FreezeColumns(4);

            contentWS.Cell(++row, 1).Value = "Placed under the care of SJS until such a time when a decision to FG is rendered.";
            contentWS.Range(contentWS.Cell(row, 1).Address, contentWS.Cell(row, 6).Address).Row(1).Merge();
            contentWS.Cell(row, 1).Style.Font.FontColor = XLColor.Red;
            contentWS.Cell(row, 1).Style.Font.Bold = true;
            row += 2;

            List<ForceElement>? apirationalForceElements = _context.ForceElements.Include(d => d.Organization)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Conplans)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Designation)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Echelon)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.NoticeToMove)
                                                                      .Include(d => d.DataCards)
                                                                            .ThenInclude(d => d.Operations)
                                                                      .Where(d => d.Aspirational && d.DataCards.Any())
                                                                      .OrderBy(f => f.ElementId)
                                                                      .ToList();
            if (apirationalForceElements.Any())
            {
                contentWS = FillAnnexBData(apirationalForceElements, contentWS, row, true);
                row += apirationalForceElements.Count;
            }

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);

            return File(stream.ToArray(), applicationTypeSpreadsheet, "AnnexB.xlsx");
        }

        private static IXLWorksheet FillAnnexBData(List<ForceElement> forceElements, IXLWorksheet worksheet, int row, bool aspirational)
        {
            int rowStart = row;
            int serial = 1;
            foreach (ForceElement? felm in forceElements)
            {
                DataCard? dataCard = felm.DataCards.FirstOrDefault();
                if (dataCard == null)
                {
                    continue;
                }
                if (!aspirational)
                {
                    worksheet.Cell(row, 1).Value = serial++;
                }
                worksheet.Cell(row, 2).Value = felm.ElementId;
                worksheet.Cell(row, 3).Value = felm.ElementName;
                if (aspirational)
                {
                    worksheet.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
                }
                worksheet.Cell(row, 3).Style.Alignment.WrapText = true;
                worksheet.Cell(row, 4).Value = felm.Aspirational ? "Aspirational" : "";
                worksheet.Cell(row, 5).Value = felm.Organization.OrganizationName;
                worksheet.Cell(row, 5).Style.Font.Bold = true;
                worksheet.Cell(row, 6).Value = dataCard.NoticeToMove?.NoticeToMoveName == "Other" ? dataCard.Ntmdetails : dataCard.NoticeToMove?.NoticeToMoveName;
                worksheet.Cell(row, 6).Style.Alignment.WrapText = true;
                worksheet.Cell(row, 7).Value = ""; // Op Supp Elem
                worksheet.Cell(row, 8).Value = dataCard.Conplans.Any(c => c.ConplanName == "CITADEL") ? "X" : "";
                worksheet.Cell(row, 9).Value = dataCard.Operations.Any(o => o.OperationName == "Op LIMPID") ? "X" : "";
                worksheet.Cell(row, 10).Value = dataCard.Operations.Any(o => o.OperationName == "Op LASER"
                    || o.OperationName == "Op LENTUS") || dataCard.Conplans.Any(c => c.ConplanName == "LOKI") ? "X" : "";
                worksheet.Cell(row, 11).Value = dataCard.Conplans.Any(c => c.ConplanName == "SOTERIA" || c.ConplanName == "SUBSAR") ? "X" : "";
                worksheet.Cell(row, 12).Value = dataCard.Operations.Any(o => o.OperationName == "Op SCYLLA") ? "X" : "";
                worksheet.Cell(row, 13).Value = dataCard.Conplans.Any(c => c.ConplanName == "RUBICON") ? "X" : "";
                worksheet.Cell(row, 14).Value = dataCard.Conplans.Any(c => c.ConplanName == "LADEN") ? "X" : "";
                worksheet.Cell(row, 15).Value = dataCard.Conplans.Any(c => c.ConplanName == "LEXICON") ? "X" : "";
                worksheet.Cell(row, 16).Value = dataCard.Conplans.Any(c => c.ConplanName == "SAR") ? "X" : "";
                worksheet.Cell(row, 17).Value = dataCard.Conplans.Any(c => c.ConplanName == "NORAD 3310") ? "X" : "";
                worksheet.Cell(row, 18).Value = ""; // eNRF/NRI
                worksheet.Cell(row, 19).Value = dataCard.Conplans.Any(c => c.ConplanName == "JUPITER") ? "X" : "";
                worksheet.Cell(row, 20).Value = dataCard.Conplans.Any(c => c.ConplanName == "PRESENCE") ? "X" : "";
                worksheet.Cell(row, 21).Value = dataCard.Conplans.Any(c => c.ConplanName == "RENAISSANCE") ? "X" : "";
                worksheet.Cell(row, 22).Value = dataCard.Conplans.Any(c => c.ConplanName == "ANGLE") ? "X" : "";

                IXLRange dataRange = worksheet.Range(worksheet.Cell(row, 1).Address, worksheet.Cell(row, 22).Address);
                dataRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(row, 3).Address, worksheet.Cell(row, 4).Address).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell(row++, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            }

            IXLRange range = worksheet.Range(worksheet.Cell(rowStart, 1).Address, worksheet.Cell(row - 1, 22).Address);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.Black;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = XLColor.Black;
            return worksheet;
        }

        // All methods below only used by eNRF/NRI reports

        /* Function: CreateNRIReport
         * Parameters: none
         * Return value: a .xlsx file
         * Description: XLWorkbook is used to generate a formatted spreadsheet with color-coded headers that displays Force Elements
         * which have been labelled as NRI. The spreadsheet is created to be used for NATO reporting.
         */

        public IActionResult CreateNRIReport()
        {
            int count = _context.DataCards.Where(d => d.Designation!.DesignationName == nriDeclared).Count();
            currentReportType = NRI;
            reportPOC = _context.ReportPocinformations.Single();

            InitializeExcelFile();
            CreateMainHeader();
            CreateDataGroupingHeaders();
            CreateColumnHeaders();
            SetDropdownValidation(count);

            contentWS!.PageSetup.Footer.Right.AddText("D-");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.PageNumber);
            contentWS!.PageSetup.Footer.Right.AddText("/");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.NumberOfPages);
            if (!FillSheetWithData()) return NotFound();

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);
            byte[] content = stream.ToArray();

            return File(content, applicationTypeSpreadsheet, "NRIReport.xlsx");
        }

        /* Function: CreateENRFReport
         * Parameters: none
         * Return value: a .xlsx file
         * Description: XLWorkbook is used to generate a formatted spreadsheet with color-coded headers that displays Force Elements
         * which have been labelled as ENRF. The spreadsheet is created to be used for NATO reporting.
         */

        public IActionResult CreateENRFReport()
        {
            int count = _context.DataCards.Where(d => d.Designation!.DesignationName == NRF || d.Designation.DesignationName == nrfDesignated
                || d.Designation.DesignationName == nrfEarmarked).Count();
            currentReportType = eNRF;
            reportPOC = _context.ReportPocinformations.Single();

            InitializeExcelFile();
            CreateMainHeader();
            CreateDataGroupingHeaders();
            CreateColumnHeaders();
            SetDropdownValidation(count);

            contentWS!.PageSetup.Footer.Right.AddText("C-");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.PageNumber);
            contentWS!.PageSetup.Footer.Right.AddText("/");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.NumberOfPages);

            if (!FillSheetWithData()) return NotFound();

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);
            byte[] content = stream.ToArray();

            return File(content, applicationTypeSpreadsheet, "eNRFReport.xlsx");
        }

        /* Function: CreateNFMReport
        * Parameters: none
        * Return value: a .xlsx file
        * Description: XLWorkbook is used to generate a formatted spreadsheet with color-coded headers that displays Force Elements
        * which have been labelled as NFM. The spreadsheet is created to be used for NATO reporting.
        */

        public IActionResult CreateNFMReport()
        {
            int count = _context.DataCards.Where(d => d.Designation!.DesignationName == NFM || d.Designation.DesignationName == nfmTier1
                || d.Designation.DesignationName == nfmTier2 || d.Designation.DesignationName == nfmTier3 || d.Designation.DesignationName == arfDesignated).Count();
            currentReportType = NFM;
            reportPOC = _context.ReportPocinformations.Single();

            InitializeExcelFile();
            CreateMainHeader();
            CreateDataGroupingHeaders();
            CreateColumnHeaders();
            SetDropdownValidation(count);

            contentWS!.PageSetup.Footer.Right.AddText("C-");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.PageNumber);
            contentWS!.PageSetup.Footer.Right.AddText("/");
            contentWS!.PageSetup.Footer.Right.AddText(XLHFPredefinedText.NumberOfPages);

            if (!FillSheetWithData()) return NotFound();

            using MemoryStream? stream = new();
            workbook.SaveAs(stream);
            byte[] content = stream.ToArray();

            return File(content, applicationTypeSpreadsheet, "NFMReport.xlsx");
        }

        /* Function: FillSheetWithData
        * Parameters: none
        * Return value: a boolean value to indicate success or failure
        * Description: this method fills the xlsheet with the relevant data for the report
        */

        private bool FillSheetWithData()
        {
            List<ForceElement>? forceElements = _context.ForceElements.Where(d => d.Archived == false).ToList();
            if (!forceElements.Any()) return false;
            foreach (ForceElement felm in forceElements)
            {
                DataCard? dataCard = null;
                IQueryable<DataCard>? dataCardQuery = null;
                if (currentReportType == NRI)
                {
                    dataCardQuery = _context.DataCards
                        .Where(d => d.ForceElementId == felm.Id && d.Designation!.DesignationName == nriDeclared);
                }
                else if (currentReportType == eNRF)
                {
                    dataCardQuery = _context.DataCards.Where(d => d.ForceElementId == felm.Id &&
                    (d.Designation!.DesignationName == NRF || d.Designation.DesignationName == nrfDesignated || d.Designation.DesignationName == nrfEarmarked));
                }
                else if (currentReportType == NFM)
                {
                    dataCardQuery = _context.DataCards.Where(d => d.ForceElementId == felm.Id &&
                    (d.Designation!.DesignationName == NFM || d.Designation.DesignationName == nfmTier1 || d.Designation.DesignationName == nfmTier2 || d.Designation.DesignationName == nfmTier3 || d.Designation.DesignationName == arfDesignated));
                }

                if (dataCardQuery != null)
                {
                    dataCard = dataCardQuery.Include(d => d.Capability)
                                            .Include(d => d.Category)
                                            .Include(d => d.CommandOverideStatus)
                                            .Include(d => d.Conplans)
                                            .Include(d => d.DeployedStatus)
                                            .Include(d => d.Designation)
                                            .Include(d => d.Echelon)
                                            .Include(d => d.EquipmentCombatVehicleStatus)
                                            .Include(d => d.EquipmentCommunicationsEquipmentStatus)
                                            .Include(d => d.EquipmentSpecialEquipmentStatus)
                                            .Include(d => d.EquipmentStatus)
                                            .Include(d => d.EquipmentSupportVehicleStatus)
                                            .Include(d => d.EquipmentWeaponsServiceRateStatus)
                                            .Include(d => d.ForceElement)
                                                   .ThenInclude(e => e!.Weighting)
                                            .Include(d => d.ForceElement)
                                                   .ThenInclude(e => e!.Organization)
                                            .Include(d => d.LastEditUserNavigation)
                                            .Include(d => d.Nato12SdosNavigation)
                                            .Include(d => d.Nato18SdosNavigation)
                                            .Include(d => d.NatoAfstrainingPercentageNavigation)
                                            .Include(d => d.NatoEvalCompletedNavigation)
                                            .Include(d => d.NatoCertProgramCoordNavigation)
                                            .Include(d => d.NatoCertCompletedNavigation)
                                            .Include(d => d.NatoFphyesNoBlankNavigation)
                                            .Include(d => d.NatoStratLiftCapacityNavigation)
                                            .Include(d => d.NatoNationalDeployNavigation)
                                            .Include(d => d.NatoNatSupplyPlanNavigation)
                                            .Include(d => d.NatoNatSupportElemNavigation)
                                            .Include(d => d.NoticeToMove)
                                            .Include(d => d.Operations)
                                            .Include(d => d.PersonnelStatus)
                                            .Include(d => d.Service)
                                            .Include(d => d.SrStatus)
                                            .Include(d => d.SustainmentAmmunitionStatus)
                                            .Include(d => d.SustainmentCombatRationsStatus)
                                            .Include(d => d.SustainmentPersonalEquipmentStatus)
                                            .Include(d => d.SustainmentPetrolStatus)
                                            .Include(d => d.SustainmentSparesStatus)
                                            .Include(d => d.SustainmentOtherStatus)
                                            .Include(d => d.SustainmentStatus)
                                            .Include(d => d.TrainingCollectiveTrainingStatus)
                                            .Include(d => d.TrainingIndividualTrainingStatus)
                                            .Include(d => d.TrainingStatus)
                                            .Include(d => d.TrainingSpecialtySkills)
                                            .Include(d => d.TrainingCrevalNavigation)
                                            .FirstOrDefault();
                }

                if (dataCard == null) continue;

                string ntmName = "";
                NoticeToMove? ntm = _context.NoticeToMoves.Where(d => d.Id == dataCard.NoticeToMoveId).FirstOrDefault();
                if (ntm != null && ntm.NoticeToMoveName.Any(char.IsDigit))
                {
                    // Keeps only numeric value if it exists
                    ntmName = ntm.NoticeToMoveName.Contains('-') ? ntm.NoticeToMoveName.Split('-')[1] : ntm.NoticeToMoveName;
                }

                currentExcelRow++;
                SetRequiredFieldsColor();
                for (int column = 1; column <= 64; column++)
                {
                    contentWS!.Cell(currentExcelRow, column).Value = column switch
                    {
                        2 => dataCard.NatoRequirementName,
                        3 => dataCard.NatoNationalName,
                        4 => CAN + felm.ElementId,
                        6 => CAN,
                        9 => dataCard.Subunit,
                        10 => dataCard.Unit,
                        11 => dataCard.Category?.CategoryName,
                        12 => dataCard.Designation?.DesignationName,
                        13 => dataCard.Service?.ServiceName,
                        14 => dataCard.Echelon?.EchelonName,
                        15 => dataCard.NatoCoordinates,
                        16 => dataCard.NatoLocation,
                        17 => dataCard.Capability?.CapabilityName,
                        18 => dataCard.NatoAssetsDeclared,
                        20 => dataCard.NatoMajorEquipmentComments,
                        21 => dataCard.NatoCavets,
                        24 => reportPOC?.Name,
                        25 => reportPOC?.Phone,
                        26 => reportPOC?.Email,
                        28 => reportPOC?.Name,
                        29 => reportPOC?.Phone,
                        30 => reportPOC?.Email,
                        32 => dataCard.PersonnelDesignatedStrength,
                        33 => dataCard.PersonnelActualStrength,
                        34 => ntmName,
                        35 => dataCard.NatoNoticeToEffect,
                        36 => dataCard.EquipmentStatus?.StatusDisplayValue,
                        37 => dataCard.NatoFphyesNoBlankNavigation?.Value,
                        38 => dataCard.NatoGeneralComments,
                        39 => dataCard.NatoAfstrainingPercentageNavigation?.StatusDisplayValue,
                        40 => dataCard.NatoNationalTrainingRemarks,
                        41 => dataCard.NatoEvalCompletedNavigation?.Value,
                        42 => dataCard.NatoPlannedEvalDate,
                        43 => dataCard.NatoCertProgramCoordNavigation?.Value,
                        44 => dataCard.NatoCertCompletedNavigation?.Value,
                        52 => dataCard.NatoStratLiftCapacityNavigation?.StratLiftCapacityName,
                        53 => dataCard.NatoStratLiftCapacityComments,
                        54 => dataCard.NatoNationalDeployNavigation?.NationalDeployName,
                        55 => dataCard.NatoNationalDeployComments,
                        56 => dataCard.Nato12SdosNavigation?.Value,
                        57 => dataCard.Nato18SdosNavigation?.Value,
                        58 => dataCard.NatoCurrentSdos,
                        59 => dataCard.NatoNatSupplyPlanNavigation?.Value,
                        60 => dataCard.NatoNatSupportElemNavigation?.Value,
                        63 => dataCard.SrStatus?.Id == PetsOverallGreenId ? "Green" : dataCard.SrStatus?.StatusDisplayColour,
                        64 => dataCard.NatoNationalAssesmentComments,
                        _ => "",
                    };

                    // Set background color if cell value is valid
                    if (column == 36)
                    {
                        if (dataCard.EquipmentStatus != null)
                        {
                            contentWS.Cell(currentExcelRow, column).Style.Fill.BackgroundColor =
                                XLColor.FromName(dataCard.EquipmentStatus.Id == PetsOverallGreenId ? "Green" : dataCard.EquipmentStatus.StatusDisplayColour);
                        }
                    }
                    else if (column == 39)
                    {
                        if (dataCard.NatoAfstrainingPercentageNavigation != null)
                        {
                            contentWS.Cell(currentExcelRow, column).Style.Fill.BackgroundColor =
                                XLColor.FromName(dataCard.NatoAfstrainingPercentageNavigation.StatusDisplayColour);
                        }
                    }
                    else if (column == 63)
                    {
                        if (dataCard.SrStatus != null)
                        {
                            contentWS.Cell(currentExcelRow, column).Style.Fill.BackgroundColor =
                                XLColor.FromName(dataCard.SrStatus.Id == PetsOverallGreenId ? "Green" : dataCard.SrStatus.StatusDisplayColour);
                        }
                    }
                    contentWS.Cell(currentExcelRow, column).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    contentWS.Cell(currentExcelRow, column).Style.Border.OutsideBorderColor = XLColor.LightGray;
                }
            }
            contentWS!.Columns(1, 64).AdjustToContents();
            return true;
        }

        private string getStatus(string colour)
        {
            switch (colour) {
                case "Lime":
                    return "Ready";
                    break;
                case "Yellow":
                    return "Ready with Limits";
                    break;
                case "Orange":
                    return "Combat Ineffective";
                    break;
                case "Red":
                    return "Not Ready";
                    break;
                default:
                    return "NA";
                    break;
            } 
        }

        private string CreateSortOrder(List<int> ids)
        {
            string sort = "";
            if (ids.Count != 0)
            {
                sort = ") order by case ";
                foreach (var item in ids)
                {
                    sort += "when dfe.ForcePackageId = " + item.ToString() + " then " + ids.FindIndex(n => n==item).ToString();
                }
                sort += " end, dfe.ElementID";
            }
            return sort;

        }


        /* Function: InitializeExcelFile
        * Parameters: none
        * Return value: void
        * Description: Creates the initial workbook and worksheets
        */

        private void InitializeExcelFile()
        {
            string worksheetName = "";
            if (currentReportType == NRI) worksheetName = nriWorksheetName;
            else if (currentReportType == eNRF) worksheetName = enrfWorksheetName;
            else if (currentReportType == NFM) worksheetName = nfmWorksheetName;

            if (worksheetName != "")
            {
                workbook = new XLWorkbook();
                contentWS = workbook.Worksheets.Add(worksheetName);
                SetWorksheetSizing(contentWS, "CAN SECRET REL NATO");
                validationWS = workbook.Worksheets.Add("Data Validation").Hide();
                currentExcelRow = 1;
            }
        }

        private static void SetWorksheetSizing(IXLWorksheet worksheet, string securityClassification)
        {
            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            worksheet.PageSetup.AdjustTo(65);
            worksheet.PageSetup.PaperSize = XLPaperSize.TabloidPaper;
            // Value in inches but excel shows in CM
            worksheet.PageSetup.Margins.SetTop(0.7519685);
            worksheet.PageSetup.Margins.SetBottom(0.7519685);
            worksheet.PageSetup.Margins.SetLeft(0.2519685);
            worksheet.PageSetup.Margins.SetRight(0.2519685);
            worksheet.PageSetup.Margins.SetFooter(0.2992126);
            worksheet.PageSetup.Margins.SetHeader(0.2992126);
            worksheet.PageSetup.CenterHorizontally = false;
            worksheet.PageSetup.CenterVertically = false;
            worksheet.PageSetup.Header.Left.AddText("CLASSIFICATION: ");
            worksheet.PageSetup.Header.Left.AddText(securityClassification).SetFontColor(XLColor.Red);
            worksheet.PageSetup.Footer.Left.AddText("CLASSIFICATION: ");
            worksheet.PageSetup.Footer.Left.AddText(securityClassification).SetFontColor(XLColor.Red);
        }

        /* Function: CreateMainHeader
         * Parameters: none
         * Return value: void
         * Description: Creates the main headers for the report in the excel file such as title and the rrt picture
         */

        private void CreateMainHeader()
        {
            string reportTitle = "";
            if (currentReportType == NRI) reportTitle = nriReportTitle;
            else if (currentReportType == eNRF) reportTitle = enrfReportTitle;
            else if (currentReportType == NFM) reportTitle = nfmReportTitle;

            if (contentWS != null && reportTitle != "")
            {
                contentWS.AddPicture(rrtUri).MoveTo(contentWS.Cell(1, 1));
                contentWS.Range(contentWS.Cell(currentExcelRow, 1).Address, contentWS.Cell(currentExcelRow + 2, 64).Address).Style.Fill.BackgroundColor = headerBgColor;
                currentExcelRow++;
                SetExcelCell(3, 18, reportTitle);
                currentExcelRow += 2;
                contentWS.Cell(currentExcelRow, 3).Value = releasableTo;
                currentExcelRow++;
            }
        }

        /* Function: CreateDataGroupingHeaders
         * Parameters: none
         * Return value: void
         * Description: Creates the headers for the different groups of data in the report. Ex Unit, POC, ...
         */

        private void CreateDataGroupingHeaders()
        {
            SetRangeBackground(unitHeaderFirstCell, unitHeaderFirstCell + 22, unitHeaderBgColor);
            SetExcelCell(unitHeaderFirstCell, dataFontSize, unitTitle);

            SetRangeBackground(unitPocFirstCell, unitPocFirstCell + 3, unitPocBgColor);
            SetExcelCell(unitPocFirstCell, dataFontSize, unitPocTitle);

            SetRangeBackground(reportingPocFirstCell, reportingPocFirstCell + 2, reportingPocBgColor);
            SetExcelCell(reportingPocFirstCell, dataFontSize, reportPocTitle);

            SetRangeBackground(readinessHeaderFirstCell, readinessHeaderFirstCell + 7, readinessHeaderBgColor);
            SetExcelCell(readinessHeaderFirstCell, dataFontSize, readinessTitle);

            SetRangeBackground(trainingHeaderFirstCell, trainingHeaderFirstCell + 5, trainingHeaderBgColor);
            SetExcelCell(trainingHeaderFirstCell, dataFontSize, trainingTitle);

            SetRangeBackground(brownHeaderFirstCell, brownHeaderFirstCell + 4, brownHeaderBgColor);
            SetExcelCell(brownHeaderFirstCell, dataFontSize, mestoTitle);

            SetRangeBackground(projectionHeaderFirstCell, projectionHeaderFirstCell + 5, projectionHeaderBgColor);
            SetExcelCell(projectionHeaderFirstCell, dataFontSize, projectionTitle);

            SetRangeBackground(sustainabilityHeaderFirstCell, sustainabilityHeaderFirstCell + 4, sustainabilityHeaderBgColor);
            SetExcelCell(sustainabilityHeaderFirstCell, dataFontSize, sustainabilityTitle);

            SetRangeBackground(cisHeaderFirstCell, cisHeaderFirstCell + 1, cisHeaderBgColor);
            SetExcelCell(cisHeaderFirstCell, dataFontSize, cisInteroperabilityTitle);

            SetRangeBackground(combatEffHeaderFirstCell, combatEffHeaderFirstCell + 1, combatEffHeaderBgColor);
            SetExcelCell(combatEffHeaderFirstCell, dataFontSize, combatEffTitle);
            currentExcelRow++;
        }

        /* Function: CreateColumnHeaders
         * Parameters: none
         * Return value: void
         * Description: Creates the individual headers for each column of data
         */

        private void CreateColumnHeaders()
        {
            SetRangeBackground(unitHeaderFirstCell, unitHeaderFirstCell + 29, unitDataBgColor);
            SetRangeBackground(readinessHeaderFirstCell, readinessHeaderFirstCell + 14, readinessDataBgColor);
            SetRangeBackground(brownHeaderFirstCell, brownHeaderFirstCell + 5, mestoDataBgColor);
            SetRangeBackground(projectionHeaderFirstCell, projectionHeaderFirstCell + 12, projDataBgColor);
            SetRangeBackground(combatEffHeaderFirstCell, combatEffHeaderFirstCell + 1, combatDataBgColor);
            for (int column = 1; column <= reportHeaderTitles.Length; column++)
            {
                contentWS!.Cell(currentExcelRow, column).Value = reportHeaderTitles[column - 1];
                contentWS.Cell(currentExcelRow, column).Style.Font.FontSize = dataFontSize;
                contentWS.Cell(currentExcelRow, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
        }

        /* Function: SetExcelCell
         * Parameters: int column, int fontSize, string text is the value of the cell
         * Return value: none
         * Description: Sets styling and value for identified cells
         */

        private void SetExcelCell(int column, int fontSize, string text)
        {
            if (contentWS != null)
            {
                contentWS.Cell(currentExcelRow, column).Value = text;
                contentWS.Cell(currentExcelRow, column).Style.Font.FontSize = fontSize;
                contentWS.Cell(currentExcelRow, column).Style.Font.FontColor = XLColor.White;
                contentWS.Cell(currentExcelRow, column).Style.Font.Bold = true;
            }
        }

        /* Function: SetRangeBackground
         * Parameters: int firstCol, int lastCol, XLColor bgColor
         * Return value: none
         * Description: Sets background color for range
         */

        private void SetRangeBackground(int firstCol, int lastCol, XLColor bgColor)
        {
            IXLRange range = contentWS!.Range(contentWS.Cell(currentExcelRow, firstCol).Address, contentWS.Cell(currentExcelRow, lastCol).Address);
            range.Style.Fill.BackgroundColor = bgColor;
        }

        /* Function: SetRequiredFieldsColor
         * Parameters: none
         * Return value: none
         * Description: Sets the background color of required fields to pink
         */

        private void SetRequiredFieldsColor()
        {
            foreach (int column in reportRequiredColumns)
            {
                contentWS!.Cell(currentExcelRow, column).Style.Fill.BackgroundColor = XLColor.PalePink;
            }
        }

        /* Function: InsertValidationValues
         * Parameters: col is the column in the worksheet to insert the data to,
         *  values is a list of strings that contains the values to insert,
         *  title is the a descriptor of the values placed in the first row
         * Return value: none
         * Description: inserts values in a seperate worksheet that will be used for data
         *  validation
         */

        private void InsertValidationValues(int col, List<string> values, string title)
        {
            validationWS!.Cell(1, col).Value = title;
            for (int i = 0; i < values.Count; i++)
            {
                validationWS.Cell(2 + i, col).Value = values.ElementAt(i);
            }
        }

        /* Function: SetDropdownValidation
         * Parameters: count is the number of total data entries
         * Return value: none
         * Description: This method get the data used for validation, places them in a seperate
         *  worksheet and then applies the validation to the relevant fields
         */

        private void SetDropdownValidation(int count)
        {
            List<PetsoverallStatus> petsoverallStatuses = _context.PetsoverallStatuses.Where(p => p.Archived == false).ToList();
            // Change Lime to Green
            petsoverallStatuses[petsoverallStatuses.FindIndex(s => s.Id == PetsOverallGreenId)].StatusDisplayColour = "Green";

            // Validation options
            List<string> categoryList = _context.Categories.Where(c => c.Archived == false).OrderBy(c => c.Ordered).Select(c => c.CategoryName).ToList();
            List<string> designationList = _context.Designations.Where(c => c.Archived == false).OrderBy(d => d.Ordered).Select(c => c.DesignationName).ToList();
            designationList.Insert(0, "ARF Designated");
            List<string> serviceList = _context.Services.Where(c => c.Archived == false).OrderBy(c => c.Ordered).Select(c => c.ServiceName).ToList();
            List<string> echelonList = _context.Echelons.Where(c => c.Archived == false).OrderBy(c => c.Ordered).Select(c => c.EchelonName).ToList();
            List<string> capabilityList = _context.Capabilities.Where(c => c.Archived == false).OrderBy(c => c.CapabilityName).Select(c => c.CapabilityName).ToList();
            List<string> equipmentList = petsoverallStatuses.Select(c => c.StatusDisplayValue!).ToList();
            List<string> fphList = _context.YesNoBlanks.Select(c => c.Value).ToList();
            List<string> afsList = _context.AfsTrainingPercentages.Where(c => c.Archived == false).Select(c => c.StatusDisplayValue).ToList();
            List<string> YNNAList = _context.YesNoNaBlanks.OrderBy(y => y.Order).Select(c => c.Value).ToList();
            List<string> stratLiftCapList = _context.NatoStratLiftCapacities.Select(c => c.StratLiftCapacityName).ToList();
            List<string> natDeployList = _context.NatoNationalDeploys.OrderBy(n => n.Ordered).Select(c => c.NationalDeployName).ToList();
            List<string> combEffectList = petsoverallStatuses.Select(c => c.StatusDisplayColour!).ToList();

            // Insert validation values into seperate worksheet
            InsertValidationValues(1, categoryList, "categoryOptions");
            InsertValidationValues(2, designationList, "designationOptions");
            InsertValidationValues(3, serviceList, "serviceOptions");
            InsertValidationValues(4, echelonList, "echelonOptions");
            InsertValidationValues(5, capabilityList, "capabilityOptions");
            InsertValidationValues(6, equipmentList, "equipmentOptions");
            InsertValidationValues(7, fphList, "fphOptions");
            InsertValidationValues(8, afsList, "afsOptions");
            InsertValidationValues(9, YNNAList, "YNNAOptions");
            InsertValidationValues(10, stratLiftCapList, "stratLiftCapOptions");
            InsertValidationValues(11, natDeployList, "natDeployOptions");
            InsertValidationValues(12, combEffectList, "combEffectOptions");

            // Ranges where validation values are stored
            IXLRange categoryRange = validationWS!.Range("A2:A" + (categoryList.Count + 1));
            IXLRange designationRange = validationWS.Range("B2:B" + (designationList.Count + 1));
            IXLRange serviceRange = validationWS.Range("C2:C" + (serviceList.Count + 1));
            IXLRange echelonRange = validationWS.Range("D2:D" + (echelonList.Count + 1));
            IXLRange capabilityRange = validationWS.Range("E2:E" + (capabilityList.Count + 1));
            IXLRange equipmentRange = validationWS.Range("F2:F" + (equipmentList.Count + 1));
            IXLRange fphRange = validationWS.Range("G2:G" + (fphList.Count + 1));
            IXLRange afsRange = validationWS.Range("H2:H" + (afsList.Count + 1));
            IXLRange YNNARange = validationWS.Range("I2:I" + (YNNAList.Count + 1));
            IXLRange stratLiftCapRange = validationWS.Range("J2:J" + (stratLiftCapList.Count + 1));
            IXLRange natDeployRange = validationWS.Range("K2:K" + (natDeployList.Count + 1));
            IXLRange combEffectRange = validationWS.Range("L2:L" + (combEffectList.Count + 1));

            // Set validation constraints for data fields
            for (int i = 7; i < count + 7; i++)
            {
                contentWS!.Cell(i, 11).CreateDataValidation().List(categoryRange, true);
                contentWS.Cell(i, 12).CreateDataValidation().List(designationRange, true);
                contentWS.Cell(i, 13).CreateDataValidation().List(serviceRange, true);
                contentWS.Cell(i, 14).CreateDataValidation().List(echelonRange, true);
                contentWS.Cell(i, 17).CreateDataValidation().List(capabilityRange, true);
                contentWS.Cell(i, 36).CreateDataValidation().List(equipmentRange, true);
                contentWS.Cell(i, 37).CreateDataValidation().List(fphRange, true);
                contentWS.Cell(i, 39).CreateDataValidation().List(afsRange, true);
                contentWS.Cell(i, 41).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 43).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 44).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 52).CreateDataValidation().List(stratLiftCapRange, true);
                contentWS.Cell(i, 54).CreateDataValidation().List(natDeployRange, true);
                contentWS.Cell(i, 56).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 57).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 59).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 60).CreateDataValidation().List(YNNARange, true);
                contentWS.Cell(i, 63).CreateDataValidation().List(combEffectRange, true);
            }
        }
    }
}