#nullable disable

using Microsoft.EntityFrameworkCore;

namespace SMARTV3.Models
{
    public partial class SMARTV3DbContext : DbContext
    {
        public SMARTV3DbContext(DbContextOptions<SMARTV3DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AfsTrainingPercentage> AfsTrainingPercentages { get; set; } = null!;
        public virtual DbSet<AfsTrainingPercentageArchiveComment> AfsTrainingPercentageArchiveComments { get; set; } = null!;
        public virtual DbSet<ArchiveComment> ArchiveComments { get; set; } = null!;
        public virtual DbSet<Capability> Capabilities { get; set; } = null!;
        public virtual DbSet<CapabilityArchiveComment> CapabilityArchiveComments { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryArchiveComment> CategoryArchiveComments { get; set; } = null!;
        public virtual DbSet<ChangeLog> ChangeLogs { get; set; } = null!;
        public virtual DbSet<CommandOverideStatus> CommandOverideStatuses { get; set; } = null!;
        public virtual DbSet<Conplan> Conplans { get; set; } = null!;
        public virtual DbSet<ConplanArchiveComment> ConplanArchiveComments { get; set; } = null!;
        public virtual DbSet<Creval> Crevals { get; set; } = null!;
        public virtual DbSet<DataCard> DataCards { get; set; } = null!;
        public virtual DbSet<DataCardConplanHistory> DataCardConplanHistories { get; set; } = null!;
        public virtual DbSet<DataCardHistory> DataCardHistories { get; set; } = null!;
        public virtual DbSet<DataCardOperationsHistory> DataCardOperationsHistories { get; set; } = null!;
        public virtual DbSet<DatacardKpi> DatacardKpis { get; set; } = null!;
        public virtual DbSet<DeployedStatus> DeployedStatuses { get; set; } = null!;
        public virtual DbSet<Designation> Designations { get; set; } = null!;
        public virtual DbSet<DesignationArchiveComment> DesignationArchiveComments { get; set; } = null!;
        public virtual DbSet<DummyDataCard> DummyDataCards { get; set; } = null!;
        public virtual DbSet<DummyForceElement> DummyForceElements { get; set; } = null!;
        public virtual DbSet<Echelon> Echelons { get; set; } = null!;
        public virtual DbSet<EchelonArchiveComment> EchelonArchiveComments { get; set; } = null!;
        public virtual DbSet<ForceElement> ForceElements { get; set; } = null!;
        public virtual DbSet<ForceElementArchiveComment> ForceElementArchiveComments { get; set; } = null!;
        public virtual DbSet<ForcePackage> ForcePackages { get; set; } = null!;
        public virtual DbSet<ForcePackageKpi> ForcePackageKpis { get; set; } = null!;
        public virtual DbSet<ForcePackagePurpose> ForcePackagePurposes { get; set; } = null!;
        public virtual DbSet<FpcompareModel> FpcompareModels { get; set; } = null!;
        public virtual DbSet<HistoryMigration> HistoryMigrations { get; set; } = null!;
        public virtual DbSet<NatoNationalDeploy> NatoNationalDeploys { get; set; } = null!;
        public virtual DbSet<NatoStratLiftCapacity> NatoStratLiftCapacities { get; set; } = null!;
        public virtual DbSet<NoticeToMove> NoticeToMoves { get; set; } = null!;
        public virtual DbSet<Operation> Operations { get; set; } = null!;
        public virtual DbSet<OperationArchiveComment> OperationArchiveComments { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<OrganizationArchiveComment> OrganizationArchiveComments { get; set; } = null!;
        public virtual DbSet<PetsoverallStatus> PetsoverallStatuses { get; set; } = null!;
        public virtual DbSet<PetspercentStatus> PetspercentStatuses { get; set; } = null!;
        public virtual DbSet<ReportPocinformation> ReportPocinformations { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceArchiveComment> ServiceArchiveComments { get; set; } = null!;
        public virtual DbSet<SpecialtySkill> SpecialtySkills { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Weighting> Weightings { get; set; } = null!;
        public virtual DbSet<YesNoBlank> YesNoBlanks { get; set; } = null!;
        public virtual DbSet<YesNoNaBlank> YesNoNaBlanks { get; set; } = null!;
        public virtual DbSet<CapibilityMet> CapibilityMets { get; set; } = null!;
        public virtual DbSet<FelmMetl> FelmMetls { get; set; } = null!;
        public virtual DbSet<Metl> Metls { get; set; } = null!;
        public virtual DbSet<OutputForceElement> OutputForceElements { get; set; } = null!;
        public virtual DbSet<OutputTask> OutputTasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=JFC-LTM-VMW2K16\\JIIFC;Initial Catalog=SMARTV3;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AfsTrainingPercentage>(entity =>
            {
                entity.ToTable("AfsTrainingPercentage");

                entity.Property(e => e.StatusDisplayColour)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDisplayValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AfsTrainingPercentageArchiveComment>(entity =>
            {
                entity.HasOne(d => d.AfsTrainingPercentage)
                    .WithMany(p => p.AfsTrainingPercentageArchiveComments)
                    .HasForeignKey(d => d.AfsTrainingPercentageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AfsTrainingPercentageArchive_AfsTrainingPercentages");

                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.AfsTrainingPercentageArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AfsTrainingPercentageArchive_ArchiveComments");
            });

            modelBuilder.Entity<ArchiveComment>(entity =>
            {
                entity.Property(e => e.ChangeDate).HasColumnType("datetime");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.HasOne(d => d.ChangeUserNavigation)
                    .WithMany(p => p.ArchiveComments)
                    .HasForeignKey(d => d.ChangeUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArchiveComments_Users");
            });

            modelBuilder.Entity<Capability>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Primary Key for Capabilities");

                entity.Property(e => e.Archived).HasComment("If the Capability is archived and should no longer show in the UI");

                entity.Property(e => e.NatoCapability).HasComment("If the Capability is from NATO UI");

                entity.Property(e => e.CapabilityDesc)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasComment("Capability Description");

                entity.Property(e => e.CapabilityName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("English Capability Name");

                entity.Property(e => e.CapabilityNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("French Capability Name");

                entity.Property(e => e.Ordered).HasComment("Order Capibilities in Drop Downs");
            });

            modelBuilder.Entity<CapabilityArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.CapabilityArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CapabilityArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.CapabilityArchiveComments)
                    .HasForeignKey(d => d.CapabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CapabilityArchiveComments_Capabilities");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Primary Key for Categories");

                entity.Property(e => e.Archived).HasComment("If the Categories is archived and should no longer show in the UI");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("English Category Name");

                entity.Property(e => e.CategoryNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("French Category Name");

                entity.Property(e => e.Ordered).HasComment("Order in Drop Downs");
            });

            modelBuilder.Entity<CategoryArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.CategoryArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryArchiveComments)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryArchiveComments_Categories");
            });

            modelBuilder.Entity<ChangeLog>(entity =>
            {
                entity.ToTable("ChangeLog");

                entity.Property(e => e.ChangedDate).HasColumnType("date");

                entity.Property(e => e.CommandOverrideAuthority)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoCavets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoGeneralComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoMajorEquipmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalAssesmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalDeployComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoStratLiftCapacityComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PersonnelComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SustainmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TrainingComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.CommandOverideStatus)
                    .WithMany(p => p.ChangeLogs)
                    .HasForeignKey(d => d.CommandOverideStatusId)
                    .HasConstraintName("FK_ChangeLog_CommandOverideStatuses");

                entity.HasOne(d => d.DeployedStatus)
                    .WithMany(p => p.ChangeLogs)
                    .HasForeignKey(d => d.DeployedStatusId)
                    .HasConstraintName("FK_ChangeLog_DeployedStatuses");

                entity.HasOne(d => d.EquipmentStatus)
                    .WithMany(p => p.ChangeLogEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentStatusId)
                    .HasConstraintName("FK_ChangeLog_PETSOverallStatuses3");

                entity.HasOne(d => d.ForceElement)
                    .WithMany(p => p.ChangeLogs)
                    .HasForeignKey(d => d.ForceElementId)
                    .HasConstraintName("FK_ChangeLog_ForceElements");

                entity.HasOne(d => d.LastEditUserNavigation)
                    .WithMany(p => p.ChangeLogs)
                    .HasForeignKey(d => d.LastEditUser)
                    .HasConstraintName("FK_ChangeLog_Users");

                entity.HasOne(d => d.PersonnelStatus)
                    .WithMany(p => p.ChangeLogPersonnelStatuses)
                    .HasForeignKey(d => d.PersonnelStatusId)
                    .HasConstraintName("FK_ChangeLog_PETSOverallStatuses1");

                entity.HasOne(d => d.SrStatus)
                    .WithMany(p => p.ChangeLogSrStatuses)
                    .HasForeignKey(d => d.SrStatusId)
                    .HasConstraintName("FK_ChangeLog_PETSOverallStatuses");

                entity.HasOne(d => d.SustainmentStatus)
                    .WithMany(p => p.ChangeLogSustainmentStatuses)
                    .HasForeignKey(d => d.SustainmentStatusId)
                    .HasConstraintName("FK_ChangeLog_PETSOverallStatuses4");

                entity.HasOne(d => d.TrainingStatus)
                    .WithMany(p => p.ChangeLogTrainingStatuses)
                    .HasForeignKey(d => d.TrainingStatusId)
                    .HasConstraintName("FK_ChangeLog_PETSOverallStatuses2");
            });

            modelBuilder.Entity<CommandOverideStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.StatusDisplayvalue)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDisplayvalueFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StausDisplayColour)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Conplan>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Primary Key for Conplan Table");

                entity.Property(e => e.Archived).HasComment("If the Conplan is archived and should no longer show in the UI");

                entity.Property(e => e.ConplanName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("English Conplan Name");

                entity.Property(e => e.ConplanNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("French Conplan Name");

                entity.Property(e => e.Ordered).HasComment("Order Conplans in Drop Downs");
            });

            modelBuilder.Entity<ConplanArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.ConplanArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConplanArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Conplan)
                    .WithMany(p => p.ConplanArchiveComments)
                    .HasForeignKey(d => d.ConplanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConplanArchiveComments_Conplans");
            });

            modelBuilder.Entity<Creval>(entity =>
            {
                entity.ToTable("Creval");

                entity.Property(e => e.CrevalName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CrevalNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DataCard>(entity =>
            {
                entity.ToTable("DataCard");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Brigade)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideAuthority)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyCommnets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DataCardComplete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Division)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentComments).IsUnicode(false);

                entity.Property(e => e.LastEditDate).HasColumnType("date");

                entity.Property(e => e.NatoAfstraining).HasColumnName("NatoAFSTraining");

                entity.Property(e => e.NatoAfstrainingPercentage)
                    .HasColumnName("NatoAFSTrainingPercentage")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoCavets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoCoordinates)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoFph).HasColumnName("NatoFPH");

                entity.Property(e => e.NatoFphyesNoBlank)
                    .HasColumnName("NatoFPHYesNoBlank")
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.NatoGeneralComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoLocation)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoMajorEquipmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNatSupplyPlan).HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoNatSupportElem).HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoNationalAssesmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalDeployComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalTrainingRemarks).IsUnicode(false);

                entity.Property(e => e.NatoPlannedEvalDate).HasColumnType("date");

                entity.Property(e => e.NatoRequirementName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NatoStratLiftCapacityComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Ntmdetails)
                    .IsUnicode(false)
                    .HasColumnName("NTMDetails");

                entity.Property(e => e.PersonnelAps).HasColumnName("PersonnelAPS");

                entity.Property(e => e.PersonnelComments).IsUnicode(false);

                entity.Property(e => e.PersonnelIt).HasColumnName("PersonnelIT");

                entity.Property(e => e.PersonnelLob).HasColumnName("PersonnelLOB");

                entity.Property(e => e.Rds).HasColumnName("RDS");

                entity.Property(e => e.ReadinessFromDate).HasColumnType("date");

                entity.Property(e => e.ReadinessToDate).HasColumnType("date");

                entity.Property(e => e.Subunit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.SustainmentComments).IsUnicode(false);

                entity.Property(e => e.SustainmentPpereadinessFactor).HasColumnName("SustainmentPPEReadinessFactor");

                entity.Property(e => e.TrainingComments).IsUnicode(false);

                entity.Property(e => e.TrainingCreval).HasColumnName("TrainingCREVAL");

                entity.Property(e => e.TrainingCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingCREVALDate");

                entity.Property(e => e.TrainingCtreadinessFactor).HasColumnName("TrainingCTReadinessFactor");

                entity.Property(e => e.TrainingItreadinessFactor).HasColumnName("TrainingITReadinessFactor");

                entity.Property(e => e.TrainingProjectedCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingProjectedCREVALDate");

                entity.Property(e => e.Unit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Validitydate).HasColumnType("date");

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.CapabilityId)
                    .HasConstraintName("FK_DataCard_Capabilities");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_DataCard_Categories");

                entity.HasOne(d => d.CommandOverideStatus)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.CommandOverideStatusId)
                    .HasConstraintName("FK_DataCard_CommandOverideStatuses");

                entity.HasOne(d => d.DeployedStatus)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.DeployedStatusId)
                    .HasConstraintName("FK_DataCard_DeployedStatuses");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_DataCard_Designations");

                entity.HasOne(d => d.Echelon)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.EchelonId)
                    .HasConstraintName("FK_DataCard_Echelons");

                entity.HasOne(d => d.EquipmentCombatVehicleStatus)
                    .WithMany(p => p.DataCardEquipmentCombatVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentCombatVehicleStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses5");

                entity.HasOne(d => d.EquipmentCommunicationsEquipmentStatus)
                    .WithMany(p => p.DataCardEquipmentCommunicationsEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentCommunicationsEquipmentStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses10");

                entity.HasOne(d => d.EquipmentSpecialEquipmentStatus)
                    .WithMany(p => p.DataCardEquipmentSpecialEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentSpecialEquipmentStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses11");

                entity.HasOne(d => d.EquipmentStatus)
                    .WithMany(p => p.DataCardEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses2");

                entity.HasOne(d => d.EquipmentSupportVehicleStatus)
                    .WithMany(p => p.DataCardEquipmentSupportVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentSupportVehicleStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses6");

                entity.HasOne(d => d.EquipmentWeaponsServiceRateStatus)
                    .WithMany(p => p.DataCardEquipmentWeaponsServiceRateStatuses)
                    .HasForeignKey(d => d.EquipmentWeaponsServiceRateStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses9");

                entity.HasOne(d => d.ForceElement)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.ForceElementId)
                    .HasConstraintName("FK_DataCard_ForceElements");

                entity.HasOne(d => d.LastEditUserNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.LastEditUser)
                    .HasConstraintName("FK_DataCard_Users");

                entity.HasOne(d => d.Nato12SdosNavigation)
                    .WithMany(p => p.DataCardNato12SdosNavigations)
                    .HasForeignKey(d => d.Nato12Sdos)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank2");

                entity.HasOne(d => d.Nato18SdosNavigation)
                    .WithMany(p => p.DataCardNato18SdosNavigations)
                    .HasForeignKey(d => d.Nato18Sdos)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank3");

                entity.HasOne(d => d.NatoAfstrainingPercentageNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.NatoAfstrainingPercentage)
                    .HasConstraintName("FK_DataCard_AfsTrainingPercentage");

                entity.HasOne(d => d.NatoCertCompletedNavigation)
                    .WithMany(p => p.DataCardNatoCertCompletedNavigations)
                    .HasForeignKey(d => d.NatoCertCompleted)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank6");

                entity.HasOne(d => d.NatoCertProgramCoordNavigation)
                    .WithMany(p => p.DataCardNatoCertProgramCoordNavigations)
                    .HasForeignKey(d => d.NatoCertProgramCoord)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank5");

                entity.HasOne(d => d.NatoEvalCompletedNavigation)
                    .WithMany(p => p.DataCardNatoEvalCompletedNavigations)
                    .HasForeignKey(d => d.NatoEvalCompleted)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank4");

                entity.HasOne(d => d.NatoFphyesNoBlankNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.NatoFphyesNoBlank)
                    .HasConstraintName("FK_DataCard_YesNoBlank");

                entity.HasOne(d => d.NatoNatSupplyPlanNavigation)
                    .WithMany(p => p.DataCardNatoNatSupplyPlanNavigations)
                    .HasForeignKey(d => d.NatoNatSupplyPlan)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank");

                entity.HasOne(d => d.NatoNatSupportElemNavigation)
                    .WithMany(p => p.DataCardNatoNatSupportElemNavigations)
                    .HasForeignKey(d => d.NatoNatSupportElem)
                    .HasConstraintName("FK_DataCard_YesNoNaBlank1");

                entity.HasOne(d => d.NatoNationalDeployNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.NatoNationalDeployId)
                    .HasConstraintName("FK_DataCard_NatoNationalDeploy");

                entity.HasOne(d => d.NatoStratLiftCapacityNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.NatoStratLiftCapacityId)
                    .HasConstraintName("FK_DataCard_NatoStratLiftCapacity");

                entity.HasOne(d => d.NoticeToMove)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.NoticeToMoveId)
                    .HasConstraintName("FK_DataCard_NoticeToMove");

                entity.HasOne(d => d.PersonnelStatus)
                    .WithMany(p => p.DataCardPersonnelStatuses)
                    .HasForeignKey(d => d.PersonnelStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses1");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_DataCard_Services");

                entity.HasOne(d => d.SrStatus)
                    .WithMany(p => p.DataCardSrStatuses)
                    .HasForeignKey(d => d.SrStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses");

                entity.HasOne(d => d.SustainmentAmmunitionStatus)
                    .WithMany(p => p.DataCardSustainmentAmmunitionStatuses)
                    .HasForeignKey(d => d.SustainmentAmmunitionStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses7");

                entity.HasOne(d => d.SustainmentCombatRationsStatus)
                    .WithMany(p => p.DataCardSustainmentCombatRationsStatuses)
                    .HasForeignKey(d => d.SustainmentCombatRationsStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses14");

                entity.HasOne(d => d.SustainmentOtherStatus)
                    .WithMany(p => p.DataCardSustainmentOtherStatuses)
                    .HasForeignKey(d => d.SustainmentOtherStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses17");

                entity.HasOne(d => d.SustainmentPersonalEquipmentStatus)
                    .WithMany(p => p.DataCardSustainmentPersonalEquipmentStatuses)
                    .HasForeignKey(d => d.SustainmentPersonalEquipmentStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses15");

                entity.HasOne(d => d.SustainmentPetrolStatus)
                    .WithMany(p => p.DataCardSustainmentPetrolStatuses)
                    .HasForeignKey(d => d.SustainmentPetrolStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses16");

                entity.HasOne(d => d.SustainmentSparesStatus)
                    .WithMany(p => p.DataCardSustainmentSparesStatuses)
                    .HasForeignKey(d => d.SustainmentSparesStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses8");

                entity.HasOne(d => d.SustainmentStatus)
                    .WithMany(p => p.DataCardSustainmentStatuses)
                    .HasForeignKey(d => d.SustainmentStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses4");

                entity.HasOne(d => d.TrainingCollectiveTrainingStatus)
                    .WithMany(p => p.DataCardTrainingCollectiveTrainingStatuses)
                    .HasForeignKey(d => d.TrainingCollectiveTrainingStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses12");

                entity.HasOne(d => d.TrainingCrevalNavigation)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.TrainingCreval)
                    .HasConstraintName("FK_DataCard_Creval");

                entity.HasOne(d => d.TrainingIndividualTrainingStatus)
                    .WithMany(p => p.DataCardTrainingIndividualTrainingStatuses)
                    .HasForeignKey(d => d.TrainingIndividualTrainingStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses13");

                entity.HasOne(d => d.TrainingSpecialtySkills)
                    .WithMany(p => p.DataCards)
                    .HasForeignKey(d => d.TrainingSpecialtySkillsId)
                    .HasConstraintName("FK_DataCard_SpecialtySkills");

                entity.HasOne(d => d.TrainingStatus)
                    .WithMany(p => p.DataCardTrainingStatuses)
                    .HasForeignKey(d => d.TrainingStatusId)
                    .HasConstraintName("FK_DataCard_PETSOverallStatuses3");

                entity.HasMany(d => d.Conplans)
                    .WithMany(p => p.DataCards)
                    .UsingEntity<Dictionary<string, object>>(
                        "DataCardConplan",
                        l => l.HasOne<Conplan>().WithMany().HasForeignKey("ConplanId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_DataCardConplan_Conplans1"),
                        r => r.HasOne<DataCard>().WithMany().HasForeignKey("DataCardId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_DataCardConplan_DataCard"),
                        j =>
                        {
                            j.HasKey("DataCardId", "ConplanId").HasName("PK_felm_data_card_conplan");

                            j.ToTable("DataCardConplan");
                        });

                entity.HasMany(d => d.Operations)
                    .WithMany(p => p.DataCards)
                    .UsingEntity<Dictionary<string, object>>(
                        "DataCardOperation",
                        l => l.HasOne<Operation>().WithMany().HasForeignKey("OperationId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_DataCardOperations_Operations"),
                        r => r.HasOne<DataCard>().WithMany().HasForeignKey("DataCardId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_DataCardOperations_DataCard"),
                        j =>
                        {
                            j.HasKey("DataCardId", "OperationId").HasName("PK_felm_data_card_operation_1");

                            j.ToTable("DataCardOperations");
                        });
            });

            modelBuilder.Entity<DataCardConplanHistory>(entity =>
            {
                entity.HasKey(e => new { e.DataCardId, e.ConplanId, e.HistoryYear, e.HistoryMonth })
                    .HasName("PK_felm_data_card_conplan_hist");

                entity.ToTable("DataCardConplanHistory");

                entity.HasOne(d => d.Conplan)
                    .WithMany(p => p.DataCardConplanHistories)
                    .HasForeignKey(d => d.ConplanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DataCardConplanHistory_Conplans");
            });

            modelBuilder.Entity<DataCardHistory>(entity =>
            {
                entity.ToTable("DataCardHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Brigade)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideAuthority)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyCommnets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Division)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentComments).IsUnicode(false);

                entity.Property(e => e.LastEditDate).HasColumnType("date");

                entity.Property(e => e.NatoAfstraining).HasColumnName("NatoAFSTraining");

                entity.Property(e => e.NatoAfstrainingPercentage)
                    .HasColumnName("NatoAFSTrainingPercentage")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoCavets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoCoordinates)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoFph).HasColumnName("NatoFPH");

                entity.Property(e => e.NatoFphyesNoBlank)
                    .HasColumnName("NatoFPHYesNoBlank")
                    .HasDefaultValueSql("((3))");

                entity.Property(e => e.NatoGeneralComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoLocation)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoMajorEquipmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNatSupplyPlan).HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoNatSupportElem).HasDefaultValueSql("((1))");

                entity.Property(e => e.NatoNationalAssesmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalDeployComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalTrainingRemarks).IsUnicode(false);

                entity.Property(e => e.NatoPlannedEvalDate).HasColumnType("date");

                entity.Property(e => e.NatoRequirementName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NatoStratLiftCapacityComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Ntmdetails)
                    .IsUnicode(false)
                    .HasColumnName("NTMDetails");

                entity.Property(e => e.PersonnelAps).HasColumnName("PersonnelAPS");

                entity.Property(e => e.PersonnelComments).IsUnicode(false);

                entity.Property(e => e.PersonnelIt).HasColumnName("PersonnelIT");

                entity.Property(e => e.PersonnelLob).HasColumnName("PersonnelLOB");

                entity.Property(e => e.Rds).HasColumnName("RDS");

                entity.Property(e => e.ReadinessFromDate).HasColumnType("date");

                entity.Property(e => e.ReadinessToDate).HasColumnType("date");

                entity.Property(e => e.Subunit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.SustainmentComments).IsUnicode(false);

                entity.Property(e => e.SustainmentPpereadinessFactor).HasColumnName("SustainmentPPEReadinessFactor");

                entity.Property(e => e.TrainingComments).IsUnicode(false);

                entity.Property(e => e.TrainingCreval).HasColumnName("TrainingCREVAL");

                entity.Property(e => e.TrainingCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingCREVALDate");

                entity.Property(e => e.TrainingCtreadinessFactor).HasColumnName("TrainingCTReadinessFactor");

                entity.Property(e => e.TrainingItreadinessFactor).HasColumnName("TrainingITReadinessFactor");

                entity.Property(e => e.TrainingProjectedCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingProjectedCREVALDate");

                entity.Property(e => e.Unit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Validitydate).HasColumnType("date");

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.CapabilityId)
                    .HasConstraintName("FK_DataCardHistory_Capabilities");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_DataCardHistory_Categories");

                entity.HasOne(d => d.CommandOverideStatus)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.CommandOverideStatusId)
                    .HasConstraintName("FK_DataCardHistory_CommandOverideStatuses");

                entity.HasOne(d => d.DeployedStatus)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.DeployedStatusId)
                    .HasConstraintName("FK_DataCardHistory_DeployedStatuses");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_DataCardHistory_Designations");

                entity.HasOne(d => d.Echelon)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.EchelonId)
                    .HasConstraintName("FK_DataCardHistory_Echelons");

                entity.HasOne(d => d.EquipmentCombatVehicleStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentCombatVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentCombatVehicleStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses");

                entity.HasOne(d => d.EquipmentCommunicationsEquipmentStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentCommunicationsEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentCommunicationsEquipmentStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses3");

                entity.HasOne(d => d.EquipmentSpecialEquipmentStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentSpecialEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentSpecialEquipmentStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses4");

                entity.HasOne(d => d.EquipmentStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSOverallStatuses2");

                entity.HasOne(d => d.EquipmentSupportVehicleStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentSupportVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentSupportVehicleStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses1");

                entity.HasOne(d => d.EquipmentWeaponsServiceRateStatus)
                    .WithMany(p => p.DataCardHistoryEquipmentWeaponsServiceRateStatuses)
                    .HasForeignKey(d => d.EquipmentWeaponsServiceRateStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses13");

                entity.HasOne(d => d.ForceElement)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.ForceElementId)
                    .HasConstraintName("FK_DataCardHistory_ForceElements");

                entity.HasOne(d => d.LastEditUserNavigation)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.LastEditUser)
                    .HasConstraintName("FK_DataCardHistory_Users");

                entity.HasOne(d => d.NatoStratLiftCapacityNavigation)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.NatoStratLiftCapacityId)
                    .HasConstraintName("FK_DataCardHistory_NatoStratLiftCapacity");

                entity.HasOne(d => d.NoticeToMove)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.NoticeToMoveId)
                    .HasConstraintName("FK_DataCardHistory_NoticeToMove");

                entity.HasOne(d => d.PersonnelStatus)
                    .WithMany(p => p.DataCardHistoryPersonnelStatuses)
                    .HasForeignKey(d => d.PersonnelStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSOverallStatuses1");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.DataCardHistories)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_DataCardHistory_Services");

                entity.HasOne(d => d.SrStatus)
                    .WithMany(p => p.DataCardHistorySrStatuses)
                    .HasForeignKey(d => d.SrStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSOverallStatuses");

                entity.HasOne(d => d.SustainmentAmmunitionStatus)
                    .WithMany(p => p.DataCardHistorySustainmentAmmunitionStatuses)
                    .HasForeignKey(d => d.SustainmentAmmunitionStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses10");

                entity.HasOne(d => d.SustainmentCombatRationsStatus)
                    .WithMany(p => p.DataCardHistorySustainmentCombatRationsStatuses)
                    .HasForeignKey(d => d.SustainmentCombatRationsStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses7");

                entity.HasOne(d => d.SustainmentOtherStatus)
                    .WithMany(p => p.DataCardHistorySustainmentOtherStatuses)
                    .HasForeignKey(d => d.SustainmentOtherStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses12");

                entity.HasOne(d => d.SustainmentPersonalEquipmentStatus)
                    .WithMany(p => p.DataCardHistorySustainmentPersonalEquipmentStatuses)
                    .HasForeignKey(d => d.SustainmentPersonalEquipmentStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses8");

                entity.HasOne(d => d.SustainmentPetrolStatus)
                    .WithMany(p => p.DataCardHistorySustainmentPetrolStatuses)
                    .HasForeignKey(d => d.SustainmentPetrolStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses9");

                entity.HasOne(d => d.SustainmentSparesStatus)
                    .WithMany(p => p.DataCardHistorySustainmentSparesStatuses)
                    .HasForeignKey(d => d.SustainmentSparesStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses11");

                entity.HasOne(d => d.SustainmentStatus)
                    .WithMany(p => p.DataCardHistorySustainmentStatuses)
                    .HasForeignKey(d => d.SustainmentStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSOverallStatuses4");

                entity.HasOne(d => d.TrainingCollectiveTrainingStatus)
                    .WithMany(p => p.DataCardHistoryTrainingCollectiveTrainingStatuses)
                    .HasForeignKey(d => d.TrainingCollectiveTrainingStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses5");

                entity.HasOne(d => d.TrainingIndividualTrainingStatus)
                    .WithMany(p => p.DataCardHistoryTrainingIndividualTrainingStatuses)
                    .HasForeignKey(d => d.TrainingIndividualTrainingStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSPercentStatuses6");

                entity.HasOne(d => d.TrainingStatus)
                    .WithMany(p => p.DataCardHistoryTrainingStatuses)
                    .HasForeignKey(d => d.TrainingStatusId)
                    .HasConstraintName("FK_DataCardHistory_PETSOverallStatuses3");
            });

            modelBuilder.Entity<DataCardOperationsHistory>(entity =>
            {
                entity.HasKey(e => new { e.DataCardId, e.OperationId, e.HistoryYear, e.HistoryMonth })
                    .HasName("PK_felm_data_card_operation_hist");

                entity.ToTable("DataCardOperationsHistory");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.DataCardOperationsHistories)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DataCardOperationsHistory_Operations");
            });

            modelBuilder.Entity<DatacardKpi>(entity =>
            {
                entity.ToTable("DatacardKpi");

                entity.HasOne(d => d.Datacard)
                    .WithMany(p => p.DatacardKpis)
                    .HasForeignKey(d => d.DatacardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DatacardKpi_DataCard");

                entity.HasOne(d => d.OverallStatusAbove)
                    .WithMany(p => p.DatacardKpiOverallStatusAboves)
                    .HasForeignKey(d => d.OverallStatusAboveId)
                    .HasConstraintName("FK_DatacardKpi_PETSOverallStatuses1");

                entity.HasOne(d => d.OverallStatusBelow)
                    .WithMany(p => p.DatacardKpiOverallStatusBelows)
                    .HasForeignKey(d => d.OverallStatusBelowId)
                    .HasConstraintName("FK_DatacardKpi_PETSOverallStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DatacardKpis)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DatacardKpi_Users");
            });

            modelBuilder.Entity<DeployedStatus>(entity =>
            {
                entity.Property(e => e.StatusDisplayValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDisplayValueFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StausDisplayColour)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Designation>(entity =>
            {
                entity.Property(e => e.DesignationName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DesignationNameFre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DesignationArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.DesignationArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DesignationArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.DesignationArchiveComments)
                    .HasForeignKey(d => d.DesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DesignationArchiveComments_Designations");
            });

            modelBuilder.Entity<DummyDataCard>(entity =>
            {
                entity.ToTable("DummyDataCard");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Brigade)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideAuthority)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CommandOverrideComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyCommnets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Division)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentComments).IsUnicode(false);

                entity.Property(e => e.LastEditDate).HasColumnType("date");

                entity.Property(e => e.NatoAfstraining).HasColumnName("NatoAFSTraining");

                entity.Property(e => e.NatoCavets)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoCoordinates)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoFph).HasColumnName("NatoFPH");

                entity.Property(e => e.NatoGeneralComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoLocation)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.NatoMajorEquipmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalAssesmentComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoNationalDeployComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NatoStratLiftCapacityComments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PersonnelAps).HasColumnName("PersonnelAPS");

                entity.Property(e => e.PersonnelComments).IsUnicode(false);

                entity.Property(e => e.PersonnelIt).HasColumnName("PersonnelIT");

                entity.Property(e => e.PersonnelLob).HasColumnName("PersonnelLOB");

                entity.Property(e => e.Rds).HasColumnName("RDS");

                entity.Property(e => e.ReadinessFromDate).HasColumnType("date");

                entity.Property(e => e.ReadinessToDate).HasColumnType("date");

                entity.Property(e => e.Subunit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.SustainmentComments).IsUnicode(false);

                entity.Property(e => e.SustainmentPpereadinessFactor).HasColumnName("SustainmentPPEReadinessFactor");

                entity.Property(e => e.TrainingComments).IsUnicode(false);

                entity.Property(e => e.TrainingCreval).HasColumnName("TrainingCREVAL");

                entity.Property(e => e.TrainingCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingCREVALDate");

                entity.Property(e => e.TrainingCtreadinessFactor).HasColumnName("TrainingCTReadinessFactor");

                entity.Property(e => e.TrainingItreadinessFactor).HasColumnName("TrainingITReadinessFactor");

                entity.Property(e => e.TrainingProjectedCrevaldate)
                    .HasColumnType("date")
                    .HasColumnName("TrainingProjectedCREVALDate");

                entity.Property(e => e.Unit)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Validitydate).HasColumnType("date");

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.CapabilityId)
                    .HasConstraintName("FK_DummyDataCard_Capabilities");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_DummyDataCard_Categories");

                entity.HasOne(d => d.CommandOverideStatus)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.CommandOverideStatusId)
                    .HasConstraintName("FK_DummyDataCard_CommandOverideStatuses");

                entity.HasOne(d => d.DeployedStatus)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.DeployedStatusId)
                    .HasConstraintName("FK_DummyDataCard_DeployedStatuses");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_DummyDataCard_Designations");

                entity.HasOne(d => d.DummyForceElement)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.DummyForceElementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DummyDataCard_DummyForceElements");

                entity.HasOne(d => d.Echelon)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.EchelonId)
                    .HasConstraintName("FK_DummyDataCard_Echelons");

                entity.HasOne(d => d.EquipmentCombatVehicleStatus)
                    .WithMany(p => p.DummyDataCardEquipmentCombatVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentCombatVehicleStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses5");

                entity.HasOne(d => d.EquipmentCommunicationsEquipmentStatus)
                    .WithMany(p => p.DummyDataCardEquipmentCommunicationsEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentCommunicationsEquipmentStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses6");

                entity.HasOne(d => d.EquipmentSpecialEquipmentStatus)
                    .WithMany(p => p.DummyDataCardEquipmentSpecialEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentSpecialEquipmentStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses7");

                entity.HasOne(d => d.EquipmentStatus)
                    .WithMany(p => p.DummyDataCardEquipmentStatuses)
                    .HasForeignKey(d => d.EquipmentStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses1");

                entity.HasOne(d => d.EquipmentSupportVehicleStatus)
                    .WithMany(p => p.DummyDataCardEquipmentSupportVehicleStatuses)
                    .HasForeignKey(d => d.EquipmentSupportVehicleStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses8");

                entity.HasOne(d => d.EquipmentWeaponsServiceRateStatus)
                    .WithMany(p => p.DummyDataCardEquipmentWeaponsServiceRateStatuses)
                    .HasForeignKey(d => d.EquipmentWeaponsServiceRateStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses9");

                entity.HasOne(d => d.LastEditUserNavigation)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.LastEditUser)
                    .HasConstraintName("FK_DummyDataCard_Users");

                entity.HasOne(d => d.NatoNationalDeployNavigation)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.NatoNationalDeployId)
                    .HasConstraintName("FK_DummyDataCard_NatoNationalDeploy");

                entity.HasOne(d => d.NatoStratLiftCapacityNavigation)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.NatoStratLiftCapacityId)
                    .HasConstraintName("FK_DummyDataCard_NatoStratLiftCapacity");

                entity.HasOne(d => d.NoticeToMove)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.NoticeToMoveId)
                    .HasConstraintName("FK_DummyDataCard_NoticeToMove");

                entity.HasOne(d => d.PersonnelStatus)
                    .WithMany(p => p.DummyDataCardPersonnelStatuses)
                    .HasForeignKey(d => d.PersonnelStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_DummyDataCard_Services");

                entity.HasOne(d => d.SrStatus)
                    .WithMany(p => p.DummyDataCardSrStatuses)
                    .HasForeignKey(d => d.SrStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses4");

                entity.HasOne(d => d.SustainmentAmmunitionStatus)
                    .WithMany(p => p.DummyDataCardSustainmentAmmunitionStatuses)
                    .HasForeignKey(d => d.SustainmentAmmunitionStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses10");

                entity.HasOne(d => d.SustainmentCombatRationsStatus)
                    .WithMany(p => p.DummyDataCardSustainmentCombatRationsStatuses)
                    .HasForeignKey(d => d.SustainmentCombatRationsStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses11");

                entity.HasOne(d => d.SustainmentOtherStatus)
                    .WithMany(p => p.DummyDataCardSustainmentOtherStatuses)
                    .HasForeignKey(d => d.SustainmentOtherStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses12");

                entity.HasOne(d => d.SustainmentPersonalEquipmentStatus)
                    .WithMany(p => p.DummyDataCardSustainmentPersonalEquipmentStatuses)
                    .HasForeignKey(d => d.SustainmentPersonalEquipmentStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses13");

                entity.HasOne(d => d.SustainmentPetrolStatus)
                    .WithMany(p => p.DummyDataCardSustainmentPetrolStatuses)
                    .HasForeignKey(d => d.SustainmentPetrolStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses14");

                entity.HasOne(d => d.SustainmentSparesStatus)
                    .WithMany(p => p.DummyDataCardSustainmentSparesStatuses)
                    .HasForeignKey(d => d.SustainmentSparesStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses15");

                entity.HasOne(d => d.SustainmentStatus)
                    .WithMany(p => p.DummyDataCardSustainmentStatuses)
                    .HasForeignKey(d => d.SustainmentStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses3");

                entity.HasOne(d => d.TrainingCollectiveTrainingStatus)
                    .WithMany(p => p.DummyDataCardTrainingCollectiveTrainingStatuses)
                    .HasForeignKey(d => d.TrainingCollectiveTrainingStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses16");

                entity.HasOne(d => d.TrainingCrevalNavigation)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.TrainingCreval)
                    .HasConstraintName("FK_DummyDataCard_Creval");

                entity.HasOne(d => d.TrainingIndividualTrainingStatus)
                    .WithMany(p => p.DummyDataCardTrainingIndividualTrainingStatuses)
                    .HasForeignKey(d => d.TrainingIndividualTrainingStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses17");

                entity.HasOne(d => d.TrainingSpecialtySkills)
                    .WithMany(p => p.DummyDataCards)
                    .HasForeignKey(d => d.TrainingSpecialtySkillsId)
                    .HasConstraintName("FK_DummyDataCard_SpecialtySkills");

                entity.HasOne(d => d.TrainingStatus)
                    .WithMany(p => p.DummyDataCardTrainingStatuses)
                    .HasForeignKey(d => d.TrainingStatusId)
                    .HasConstraintName("FK_DummyDataCard_PETSOverallStatuses2");
            });

            modelBuilder.Entity<DummyForceElement>(entity =>
            {
                entity.Property(e => e.ElementId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ElementID");

                entity.Property(e => e.ElementName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ElementNameFre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ForceElement)
                    .WithMany(p => p.DummyForceElements)
                    .HasForeignKey(d => d.ForceElementId)
                    .HasConstraintName("FK_DummyForceElements_ForceElements");

                entity.HasOne(d => d.ForcePackage)
                    .WithMany(p => p.DummyForceElements)
                    .HasForeignKey(d => d.ForcePackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DummyForceElements_ForcePackages");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.DummyForceElements)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DummyForceElements_Organizations");

                entity.HasOne(d => d.Weighting)
                    .WithMany(p => p.DummyForceElements)
                    .HasForeignKey(d => d.WeightingId)
                    .HasConstraintName("FK_DummyForceElements_Weighting");
            });

            modelBuilder.Entity<Echelon>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EchelonName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EchelonNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EchelonArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.EchelonArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EchelonArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Echelon)
                    .WithMany(p => p.EchelonArchiveComments)
                    .HasForeignKey(d => d.EchelonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EchelonArchiveComments_Echelons");
            });

            modelBuilder.Entity<ForceElement>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Primary Key");

                entity.Property(e => e.ElementId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ElementID");

                entity.Property(e => e.ElementName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ElementNameFre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Shortname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortnameFR)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ForceElements)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForceElements_Organizations");

                entity.HasOne(d => d.Weighting)
                    .WithMany(p => p.ForceElements)
                    .HasForeignKey(d => d.WeightingId)
                    .HasConstraintName("FK_ForceElements_Weighting");
            });

            modelBuilder.Entity<ForceElementArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.ForceElementArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForceElementArchiveComments_ArchiveComments");

                entity.HasOne(d => d.ForceElement)
                    .WithMany(p => p.ForceElementArchiveComments)
                    .HasForeignKey(d => d.ForceElementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForceElementArchiveComments_ForceElements");
            });

            modelBuilder.Entity<ForcePackage>(entity =>
            {
                entity.Property(e => e.DateLastFetchedLiveData).HasColumnType("date");

                entity.Property(e => e.ForcePackageDescription).IsUnicode(false);

                entity.Property(e => e.ForcePackageName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastEditDate).HasColumnType("date");

                entity.HasOne(d => d.ForcePackagePurposeNavigation)
                    .WithMany(p => p.ForcePackages)
                    .HasForeignKey(d => d.ForcePackagePurpose)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForcePackages_ForcePackagePurposes");

                entity.HasOne(d => d.LastEditUserNavigation)
                    .WithMany(p => p.ForcePackageLastEditUserNavigations)
                    .HasForeignKey(d => d.LastEditUser)
                    .HasConstraintName("FK_ForcePackages_Users1");

                entity.HasOne(d => d.PackageOwner)
                    .WithMany(p => p.ForcePackagePackageOwners)
                    .HasForeignKey(d => d.PackageOwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForcePackages_Users");

                entity.HasMany(d => d.Conplans)
                    .WithMany(p => p.ForcePackages)
                    .UsingEntity<Dictionary<string, object>>(
                        "ForcePackageConplan",
                        l => l.HasOne<Conplan>().WithMany().HasForeignKey("ConplanId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageConplans_Conplans"),
                        r => r.HasOne<ForcePackage>().WithMany().HasForeignKey("ForcePackageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageConplans_ForcePackages"),
                        j =>
                        {
                            j.HasKey("ForcePackageId", "ConplanId").HasName("PK__ForcePac__84F2D3CB646F690E");

                            j.ToTable("ForcePackageConplans");
                        });

                entity.HasMany(d => d.Operations)
                    .WithMany(p => p.ForcePackages)
                    .UsingEntity<Dictionary<string, object>>(
                        "ForcePackageOperation",
                        l => l.HasOne<Operation>().WithMany().HasForeignKey("OperationId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageOperations_Operations"),
                        r => r.HasOne<ForcePackage>().WithMany().HasForeignKey("ForcePackageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageOperations_ForcePackages"),
                        j =>
                        {
                            j.HasKey("ForcePackageId", "OperationId").HasName("PK__ForcePac__0F4B1E719210A046");

                            j.ToTable("ForcePackageOperations");
                        });

                entity.HasMany(d => d.Users)
                    .WithMany(p => p.ForcePackages)
                    .UsingEntity<Dictionary<string, object>>(
                        "ForcePackageUser",
                        l => l.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageUsers_Users"),
                        r => r.HasOne<ForcePackage>().WithMany().HasForeignKey("ForcePackageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ForcePackageUsers_ForcePackages"),
                        j =>
                        {
                            j.HasKey("ForcePackageId", "UserId").HasName("PK__ForcePac__847CCD7159DD9AAF");

                            j.ToTable("ForcePackageUsers");
                        });
            });

            modelBuilder.Entity<ForcePackageKpi>(entity =>
            {
                entity.ToTable("ForcePackageKpi");

                entity.HasOne(d => d.ForcePackage)
                    .WithMany(p => p.ForcePackageKpis)
                    .HasForeignKey(d => d.ForcePackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForcePackageKpi_ForcePackages");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForcePackageKpis)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForcePackageKpi_Users");
            });

            modelBuilder.Entity<ForcePackagePurpose>(entity =>
            {
                entity.Property(e => e.NameEn)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NameFr)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Order).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<FpcompareModel>(entity =>
            {
                entity.ToTable("FPCompareModels");

                entity.Property(e => e.SerializedForcePackageIds).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FpcompareModels)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FPCompareModels_Users");
            });

            modelBuilder.Entity<HistoryMigration>(entity =>
            {
                entity.HasKey(e => new { e.HistoryYear, e.HistoryMonth });
            });

            modelBuilder.Entity<NatoNationalDeploy>(entity =>
            {
                entity.ToTable("NatoNationalDeploy");

                entity.Property(e => e.NationalDeployName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NationalDeployNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NatoStratLiftCapacity>(entity =>
            {
                entity.ToTable("NatoStratLiftCapacity");

                entity.Property(e => e.StratLiftCapacityName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StratLiftCapacityNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NoticeToMove>(entity =>
            {
                entity.ToTable("NoticeToMove");

                entity.Property(e => e.NoticeToMoveName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NoticeToMoveNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.Property(e => e.OperationName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OperationNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OperationArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.OperationArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationsArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.OperationArchiveComments)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationsArchiveComments_Operations");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrganizationNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrganizationArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.OrganizationArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationArchiveComments)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationArchiveComments_Organizations");
            });

            modelBuilder.Entity<PetsoverallStatus>(entity =>
            {
                entity.ToTable("PETSOverallStatuses");

                entity.Property(e => e.StatusDisplayColour)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDisplayValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PetspercentStatus>(entity =>
            {
                entity.ToTable("PETSPercentStatuses");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.StatusDisplayColour)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDisplayValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportPocinformation>(entity =>
            {
                entity.ToTable("ReportPOCInformation");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(80);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleDescription)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoleNameLong)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.ServiceName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceNameFre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceArchiveComment>(entity =>
            {
                entity.HasOne(d => d.ArchiveComment)
                    .WithMany(p => p.ServiceArchiveComments)
                    .HasForeignKey(d => d.ArchiveCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceArchiveComments_ArchiveComments");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceArchiveComments)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceArchiveComments_Services");
            });

            modelBuilder.Entity<SpecialtySkill>(entity =>
            {
                entity.Property(e => e.SpecialtySkillName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpecialtySkillNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(75)
                    .IsUnicode(false);
                
                entity.Property(e => e.POC)
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Organizations");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_user_roles_roles"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_user_roles_users"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK_user_roles");

                            j.ToTable("UserRoles");
                        });
            });

            modelBuilder.Entity<Weighting>(entity =>
            {
                entity.ToTable("Weighting");

                entity.Property(e => e.WeightValue).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<YesNoBlank>(entity =>
            {
                entity.ToTable("YesNoBlank");

                entity.Property(e => e.Order).HasDefaultValueSql("((1))");

                entity.Property(e => e.Value)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ValueFre)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<YesNoNaBlank>(entity =>
            {
                entity.ToTable("YesNoNaBlank");

                entity.Property(e => e.Value)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ValueFre)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CapibilityMet>(entity =>
            {

                entity.ToTable("CapibilityMET");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MetId).HasColumnName("MET_Id");

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("date")
                    .HasColumnName("validFrom");

                entity.Property(e => e.ValidTo)
                    .HasColumnType("date")
                    .HasColumnName("validTo");

                entity.HasOne(d => d.Met)
                    .WithMany()
                    .HasForeignKey(d => d.MetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CapibilityMET_METL");
            });

            modelBuilder.Entity<FelmMetl>(entity =>
            {
                entity.ToTable("FelmMETL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FelmId).HasColumnName("felmId");

                entity.Property(e => e.MetId).HasColumnName("metId");

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("date")
                    .HasColumnName("validFrom");

                entity.Property(e => e.ValidTo)
                    .HasColumnType("date")
                    .HasColumnName("validTo");

                entity.HasOne(d => d.Met)
                    .WithMany(p => p.FelmMetls)
                    .HasForeignKey(d => d.MetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FelmMETL_METL");
            });

            modelBuilder.Entity<Metl>(entity =>
            {
                entity.HasKey(e => e.MetId);

                entity.ToTable("METL");

                entity.Property(e => e.MetId).HasColumnName("metId");

                entity.Property(e => e.Archived).HasColumnName("archived");

                entity.Property(e => e.MetCode)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasColumnName("metCode");

                entity.Property(e => e.MetDesc)
                    .IsUnicode(false)
                    .HasColumnName("metDesc");

                entity.Property(e => e.MetName)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("metName");
            });

            modelBuilder.Entity<OutputForceElement>(entity =>
            {
                entity.ToTable("OutputForceElement");

                entity.Property(e => e.AssignmentEnd).HasColumnType("date");

                entity.Property(e => e.AssignmentStart).HasColumnType("date");

                entity.HasOne(d => d.OutputTask)
                    .WithMany(p => p.OutputForceElements)
                    .HasForeignKey(d => d.OutputTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OutputForceElement_OutputTask");
            });

            modelBuilder.Entity<OutputTask>(entity =>
            {
                entity.ToTable("OutputTask");

                entity.Property(e => e.NtmId).HasColumnName("NTM_Id");

                entity.Property(e => e.OutputDesc)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.OutputEnd).HasColumnType("date");

                entity.Property(e => e.OutputName)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.OutputStart).HasColumnType("date");

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.OutputTasks)
                    .HasForeignKey(d => d.CapabilityId)
                    .HasConstraintName("FK_OutputTask_Capabilities");

                entity.HasOne(d => d.Ntm)
                    .WithMany(p => p.OutputTasks)
                    .HasForeignKey(d => d.NtmId)
                    .HasConstraintName("FK_OutputTask_NoticeToMove");
            });
        }
    }
}