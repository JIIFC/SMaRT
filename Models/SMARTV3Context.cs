using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SMARTV3.Models
{
    public partial class SMARTV3Context : DbContext
    {
        public SMARTV3Context()
        {
        }

        public SMARTV3Context(DbContextOptions<SMARTV3Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Capability> Capabilities { get; set; } = null!;
        public virtual DbSet<ForcePackage> ForcePackages { get; set; } = null!;
        public virtual DbSet<NoticeToMove> NoticeToMoves { get; set; } = null!;
        public virtual DbSet<OutputForceElement> OutputForceElements { get; set; } = null!;
        public virtual DbSet<OutputTask> OutputTasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:SMARTConnectionString");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Capability>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Primary Key for Capabilities");

                entity.Property(e => e.Archived).HasComment("If the Capability is archived and should no longer show in the UI");

                entity.Property(e => e.CapabilityDesc).IsUnicode(false);

                entity.Property(e => e.CapabilityName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("English Capability Name");

                entity.Property(e => e.CapabilityNameFre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("French Capability Name");

                entity.Property(e => e.NatoCapability)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ordered).HasComment("Order Capibilities in Drop Downs");
            });

            modelBuilder.Entity<ForcePackage>(entity =>
            {
                entity.Property(e => e.DateLastFetchedLiveData).HasColumnType("date");

                entity.Property(e => e.ForcePackageDescription).IsUnicode(false);

                entity.Property(e => e.ForcePackageName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastEditDate).HasColumnType("date");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
