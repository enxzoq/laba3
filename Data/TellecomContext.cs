using System;
using System.Collections.Generic;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Data;

public partial class TelecomContext : DbContext
{
    public TelecomContext()
    {
    }

    public TelecomContext(DbContextOptions<TelecomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ServiceContract> ServiceContracts { get; set; }

    public virtual DbSet<ServiceStatistic> ServiceStatistics { get; set; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }

    public virtual DbSet<TariffPlan> TariffPlans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db8317.public.databaseasp.net; Database=db8317; User Id=db8317; Password=Eq7#x=B95Cm@; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC27B0FF43F9");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Education)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Position)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServiceContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__ServiceC__C90D34094A4218AC");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubscriberId).HasColumnName("SubscriberID");
            entity.Property(e => e.TariffPlanName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.ServiceContracts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__ServiceCo__Emplo__403A8C7D");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.ServiceContracts)
                .HasForeignKey(d => d.SubscriberId)
                .HasConstraintName("FK__ServiceCo__Subsc__3F466844");

            entity.HasOne(d => d.TariffPlanNameNavigation).WithMany(p => p.ServiceContracts)
                .HasForeignKey(d => d.TariffPlanName)
                .HasConstraintName("FK__ServiceCo__Tarif__3E52440B");
        });

        modelBuilder.Entity<ServiceStatistic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceS__3214EC27D4CC13A9");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Mmscount).HasColumnName("MMSCount");
            entity.Property(e => e.Smscount).HasColumnName("SMSCount");

            entity.HasOne(d => d.Contract).WithMany(p => p.ServiceStatistics)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__ServiceSt__Contr__4316F928");
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrib__3214EC27DEDE42DC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PassportData)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TariffPlan>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__TariffPl__737584F78979891C");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BillingType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DataTransferCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InternationalCallCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LocalCallCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LongDistanceCallCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Mmscost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MMSCost");
            entity.Property(e => e.Smscost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("SMSCost");
            entity.Property(e => e.SubscriptionFee).HasColumnType("decimal(10, 2)");
        });

        

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
