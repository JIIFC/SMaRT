using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

public static class Constants
{
    #region Datacard Filtering

    public static readonly List<KeyValuePair<string, string>> StatusOptions = new() { new KeyValuePair<string, string>("Completed", "True"), new KeyValuePair<string, string>("Incomplete", "False") };

    public static readonly List<KeyValuePair<string, string>> StatusOptionsFre = new() { new KeyValuePair<string, string>("Completed Fr", "True"), new KeyValuePair<string, string>("Incomplete Fr", "False") };

    #endregion Datacard Filtering

    #region Pagination

    public static readonly string[] PaginationOptions = { "5", "10", "25", "50" };

    #endregion Pagination

    #region User

    public const string RankTitleLabel = "Rank/Title";
    public const string RoleLabel = "Role";

    public const string DevLdapUrl = "jiifc.mil.ca";
    public const string ProdLdapUrl = "dnd.cmil.ca";

    #endregion User

    #region Roles

    public const string Admin = "Admin";
    public const string SuperUser = "SuperUser";
    public const string ReportingUser = "ReportingUser";
    public const string FDUser = "FDUser";
    public const string FGPlanner = "FGPlanner";
    public const string Modeler = "Modeler";
    public const string ReadOnlyUser = "ReadOnlyUser";

    #endregion Roles

    #region amCharts
    public const string licensekey = "CH200327186998171";
    #endregion amCharts

    #region NRI/eNRF Report

    public const string applicationTypeSpreadsheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string eNRF = "eNRF";
    public const string NRF = "NRF";
    public const string NRI = "NRI";
    public const string CAN = "CAN";
    public const string NFM = "NFM";

    public const string nriDeclared = "NRI Declared";
    public const string nrfEarmarked = "NRF Earmarked";
    public const string nrfDesignated = "NRF Designated";
    public const string nfmTier1 = "NFM Tier 1";
    public const string nfmTier2 = "NFM Tier 2";
    public const string nfmTier3 = "NFM Tier 3";
    public const string arfDesignated = "ARF Designated";


    private const string report = " Report";

    public const string nriWorksheetName = NRI + report;
    public const string enrfWorksheetName = eNRF + report;
    public const string nfmWorksheetName = NFM + report;

    public const string nriReportTitle = "Readiness" + report;
    public const string enrfReportTitle = eNRF + nriReportTitle;
    public const string nfmReportTitle = NFM + nriReportTitle;

    public static readonly int[] reportRequiredColumns = { 3, 4, 6, 11, 13, 14, 15, 17, 22, 23, 24, 25, 26, 28, 30, 31, 32, 33, 34, 36, 39, 41, 42, 43, 44, 52, 54, 56, 57, 58, 59, 60, 63, 64 };

    public static readonly string[] reportHeaderTitles = {
        "Serial",
        "Requirement Name",
        "National Name",
        "Force ID",
        "UIC",
        "Nationality",
        "Second Nationality",
        "Third Nationality",
        "English Name",
        "National Parent Unit",
        "Functional Category",
        "Designation",
        "Service",
        "Echelon",
        "Peacetime Location",
        "Peacetime Location Name",
        "Capability Code",
        "Assets Declared",
        "Excluded Statements",
        "Major Equipment",
        "Caveats",
        "Vaild From",
        "Valid To",
        "POC",
        "POC Phone",
        "POC Email",
        "Duty Officer Phone",
        "Reporting POC",
        "Reporting POC Phone",
        "Reporting POC Email",
        "Reporting Date",
        "Manning Declared",
        "Manning Current",
        "NTM",
        "NTE",
        "Equipment Level",
        "Force Profile & Holdings",
        "Remarks",
        "AFS Training Completed",
        "National Training Remarks",
        "Evaluation Completed",
        "Planned Evaluation Date",
        "Certification Program Coordinated",
        "Certification Completed",
        "Manning",
        "Equipment",
        "Supply",
        "Training",
        "Ordnance",
        "Current Location",
        "Current Location Name",
        "Strategic Lift Capacity",
        "Strategic Lift Capacity Remarks",
        "National Deployment Plan",
        "National Deployment Plan Remarks",
        "12 SDOS",
        "18 SDOS",
        "Current SDOS",
        "National Supply Plan",
        "National Support Elements",
        "FMN Spiral",
        "Validation Exercise/Event",
        "Combat Effectiveness",
        "National Assessment"
    };

    public const string rrtUri = @".\wwwroot\images\RRT.png";
    public const string releasableTo = "Releasable to:";

    public static XLColor headerBgColor = XLColor.MediumSeaGreen;
    public static XLColor unitHeaderBgColor = XLColor.ForestGreen;
    public static XLColor unitPocBgColor = XLColor.OliveDrab;
    public static XLColor reportingPocBgColor = XLColor.DarkOliveGreen;
    public static XLColor readinessHeaderBgColor = XLColor.CornflowerBlue;
    public static XLColor trainingHeaderBgColor = XLColor.RoyalBlue;
    public static XLColor brownHeaderBgColor = XLColor.BrownTraditional;
    public static XLColor projectionHeaderBgColor = XLColor.CornflowerBlue;
    public static XLColor sustainabilityHeaderBgColor = XLColor.DarkMidnightBlue;
    public static XLColor cisHeaderBgColor = XLColor.Chocolate;
    public static XLColor combatEffHeaderBgColor = XLColor.DarkGoldenrod;

    public static XLColor unitDataBgColor = XLColor.Honeydew;
    public static XLColor readinessDataBgColor = XLColor.LightCyan;
    public static XLColor mestoDataBgColor = XLColor.Khaki;
    public static XLColor projDataBgColor = XLColor.AliceBlue;
    public static XLColor combatDataBgColor = XLColor.BlanchedAlmond;

    public const string unitTitle = "UNIT";
    public const string unitPocTitle = "UNIT POINT OF CONTACT";
    public const string reportPocTitle = "REPORTING POINT OF CONTACT";
    public const string readinessTitle = "READINESS";
    public const string trainingTitle = "TRAINING";
    public const string mestoTitle = "MESTO";
    public const string projectionTitle = "PROJECTION";
    public const string sustainabilityTitle = "SUSTAINABILITY";
    public const string cisInteroperabilityTitle = "CIS INTEROPERABILITY";
    public const string combatEffTitle = "COMBAT EFFECTIVENESS ASSESSMENT";

    public const int unitHeaderFirstCell = 1;
    public const int unitPocFirstCell = 24;
    public const int reportingPocFirstCell = 28;
    public const int readinessHeaderFirstCell = 31;
    public const int trainingHeaderFirstCell = 39;
    public const int brownHeaderFirstCell = 45;
    public const int projectionHeaderFirstCell = 50;
    public const int sustainabilityHeaderFirstCell = 56;
    public const int cisHeaderFirstCell = 61;
    public const int combatEffHeaderFirstCell = 63;

    #endregion NRI/eNRF Report

    #region Annexe B report

    public const int FelmNameCol = 3;
    public const int StatusCol = 4;
    public const int ForceGenCol = 5;
    public const int NtmCol = 6;
    public const int OpSuppElemCol = 7;
    public const int PresenceCol = 20;
    public const int RenaissanceCol = 21;

    public static readonly string[] annexeBHeaderTitles = {
        "Ser.",
        "FElm ID",
        "Force Element",
        "Status (Official / Aspirational)",
        "FG",
        "NTM",
        "Operational Supporting Elements",
        "CITADEL",
        "LIMPID",
        "SOODO LASER LENTUS LOKI",
        "SORTERIA SUBSAR",
        "SCYLLA",
        "RUBICON",
        "LADEN",
        "LEXICON",
        "SAR",
        "NORAD 3310 CANUS CAP/CDP",
        "eNRF/NRI (NFM) + mission spt",
        "JUPITER",
        "PRESENCE",
        "RENAISSANCE",
        "ANGLE"
    };

    #endregion Annexe B report

    #region Weight Report

    public const string totalWeightQuery = @"select sum(w.WeightValue)
                                           from DummyForceElements dfe
                                           join DummyDataCard ddc
                                           on ddc.DummyForceElementId = dfe.Id
                                           join Weighting w
                                           on w.Id = dfe.WeightingId
                                           where dfe.IsTiedToRealFelm = 1 and dfe.ForcePackageId in (";

    public const string combiWeightQuery1 = @"select sum(total.WeightValue) from(
                                            select dfe.ForceElementId, w.WeightValue, count(1) as cou
                                            from DummyForceElements dfe
                                            join DummyDataCard ddc
                                            on ddc.DummyForceElementId = dfe.Id
                                            join Weighting w
                                            on w.Id = dfe.WeightingId
                                            where dfe.ForcePackageId in (";

    public const string combiWeightQuery2 = @") and dfe.IsTiedToRealFelm = 1
                                            and (ddc.SrStatusId = 1 or ddc.SrStatusId = 2)
                                            and (ddc.CommandOverideStatusId is null or ddc.CommandOverideStatusId = 1 or ddc.CommandOverideStatusId = 2)
                                            group by dfe.ForceElementId, w.WeightValue
                                            ) as total;";

    public const string descWeightQuery = @"select dfe.ForcePackageId
                                            , fp.ForcePackageName
                                            , fp.ForcePackageDescription
                                            , dfe.ElementID
                                            , dfe.ElementName
                                            , w.WeightValue
                                            , o.OrganizationName
                                            , overstatus.StatusDisplayColour
                                            , cmdoverstatus.StatusDisplayColour
                                            , isnull(deploy.StatusDisplayValue,'Not Employed')
                                            , pstatus.StatusDisplayValue
                                            , estatus.StatusDisplayValue
                                            , tstatus.StatusDisplayValue
                                            , sstatus.StatusDisplayValue
                                            , (select count(1) from DummyForceElements dummy 
                                            where dummy.ElementID = dfe.ElementID and dummy.ForcePackageId in (";
    
    public const string descWeightMiddleQuery = @")) as times	
                                            from DummyForceElements dfe
                                            join Weighting w 
                                            on w.Id = dfe.WeightingId
                                            join ForcePackages fp
                                            on fp.Id = dfe.ForcePackageId
                                            join Organizations o
                                            on o.Id = dfe.OrganizationId
                                            join DummyDataCard ddc
                                            on ddc.DummyForceElementId = dfe.Id
                                            join PETSOverallStatuses overstatus
                                            on overstatus.Id = ddc.SrStatusId
                                            join PETSOverallStatuses pstatus
                                            on pstatus.Id = ddc.PersonnelStatusId
                                            join PETSOverallStatuses estatus
                                            on estatus.Id = ddc.EquipmentStatusId
                                            join PETSOverallStatuses tstatus
                                            on tstatus.Id = ddc.TrainingStatusId
                                            join PETSOverallStatuses sstatus
                                            on sstatus.Id = ddc.SustainmentStatusId
                                            left join PETSOverallStatuses cmdoverstatus
                                            on cmdoverstatus.Id = ddc.CommandOverideStatusId
                                            left join DeployedStatuses deploy
                                            on ddc.DeployedStatusId = deploy.Id
                                            where dfe.IsTiedToRealFelm = 1 
                                            and dfe.ForcePackageId in (";

    public const string descWeightEndQuery = @")
                                            order by dfe.ForcePackageId, dfe.ElementID";

    #endregion Weight Report
}