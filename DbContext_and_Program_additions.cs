// =============================================================
// ЧТО ВСТАВИТЬ В СУЩЕСТВУЮЩИЕ ФАЙЛЫ ПРОЕКТА
// =============================================================


// -------------------------------------------------------------
// 1. KnowledgeTestDbContext.cs
//    Добавить два DbSet рядом с существующими:
// -------------------------------------------------------------

public virtual DbSet<GroupPeriodStatus>        GroupPeriodStatuses      { get; set; }
public virtual DbSet<GroupSelectedDisciplines> GroupSelectedDisciplines  { get; set; }


// -------------------------------------------------------------
// 2. KnowledgeTestDbContext.cs → OnModelCreating
//    Добавить конфигурацию двух новых таблиц:
// -------------------------------------------------------------

modelBuilder.Entity<GroupPeriodStatus>(entity =>
{
    entity.ToTable("group_period_status");
    entity.HasKey(e => e.Id);

    entity.HasIndex(e => new { e.GroupId, e.SemesterId })
          .IsUnique()
          .HasDatabaseName("uq_gps_group_semester");

    entity.Property(e => e.UpdatedAt)
          .HasColumnName("updated_at")
          .ValueGeneratedOnAddOrUpdate();

    entity.HasOne(e => e.Group)
          .WithMany()
          .HasForeignKey(e => e.GroupId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Semester)
          .WithMany()
          .HasForeignKey(e => e.SemesterId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.UpdatedByUser)
          .WithMany()
          .HasForeignKey(e => e.UpdatedBy)
          .OnDelete(DeleteBehavior.SetNull);
});

modelBuilder.Entity<GroupSelectedDisciplines>(entity =>
{
    entity.ToTable("group_selected_disciplines");
    entity.HasKey(e => e.Id);

    entity.HasIndex(e => new { e.GroupId, e.SemesterId })
          .IsUnique()
          .HasDatabaseName("uq_gsd_group_semester");

    entity.Property(e => e.UpdatedAt)
          .HasColumnName("updated_at")
          .ValueGeneratedOnAddOrUpdate();

    entity.HasOne(e => e.Group)
          .WithMany()
          .HasForeignKey(e => e.GroupId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Semester)
          .WithMany()
          .HasForeignKey(e => e.SemesterId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Discipline1)
          .WithMany()
          .HasForeignKey(e => e.Discipline1Id)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.Discipline2)
          .WithMany()
          .HasForeignKey(e => e.Discipline2Id)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.Discipline1OwnerDepartment)
          .WithMany()
          .HasForeignKey(e => e.Discipline1OwnerDepartmentId)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.Discipline2OwnerDepartment)
          .WithMany()
          .HasForeignKey(e => e.Discipline2OwnerDepartmentId)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(e => e.SelectedByUser)
          .WithMany()
          .HasForeignKey(e => e.SelectedBy)
          .OnDelete(DeleteBehavior.SetNull);
});


// -------------------------------------------------------------
// 3. Program.cs
//    Добавить регистрацию рядом с остальными AddScoped:
// -------------------------------------------------------------

builder.Services.AddScoped<GroupPeriodStatusRepository>();
builder.Services.AddScoped<GroupPeriodStatusService>();
builder.Services.AddScoped<GroupSelectedDisciplinesRepository>();
builder.Services.AddScoped<GroupSelectedDisciplinesService>();
