using System;
using System.Collections.Generic;
using CatalogWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogWebApi.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Endowment> Endowments { get; set; }

    public virtual DbSet<EndowmentType> EndowmentTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Quotation> Quotations { get; set; }

    public virtual DbSet<QuotationItem> QuotationItems { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.faovmwzwrmqedyiisecx;Password=87H2opQyhBAItYr1;Server=aws-0-us-west-2.pooler.supabase.com;Port=6543;Database=postgres");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("unaccent");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("endowment_color_pkey");

            entity.ToTable("color");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('endowment_color_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Endowment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("endowment_pkey");

            entity.ToTable("endowment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndowmentCategoryId).HasColumnName("endowment_category_id");
            entity.Property(e => e.EndowmentTypeId).HasColumnName("endowment_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.EndowmentCategory).WithMany(p => p.Endowments)
                .HasForeignKey(d => d.EndowmentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("endowment_category_id");

            entity.HasOne(d => d.EndowmentType).WithMany(p => p.Endowments)
                .HasForeignKey(d => d.EndowmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("endowment_type_id");

            entity.HasMany(d => d.Images).WithMany(p => p.Endowments)
                .UsingEntity<Dictionary<string, object>>(
                    "EndowmentImage",
                    r => r.HasOne<Image>().WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("endowment_image_image_id_fkey"),
                    l => l.HasOne<Endowment>().WithMany()
                        .HasForeignKey("EndowmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("endowment_image_endowment_id_fkey"),
                    j =>
                    {
                        j.HasKey("EndowmentId", "ImageId").HasName("endowment_image_pkey");
                        j.ToTable("endowment_image");
                        j.IndexerProperty<int>("EndowmentId").HasColumnName("endowment_id");
                        j.IndexerProperty<int>("ImageId").HasColumnName("image_id");
                    });
        });

        modelBuilder.Entity<EndowmentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("endowment_type_pkey");

            entity.ToTable("endowment_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("image_pkey");

            entity.ToTable("image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .HasColumnName("name");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotations_pkey");

            entity.ToTable("quotations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientAddress)
                .HasMaxLength(50)
                .HasColumnName("client_address");
            entity.Property(e => e.ClientCompany)
                .HasMaxLength(50)
                .HasColumnName("client_company");
            entity.Property(e => e.ClientEmail)
                .HasMaxLength(70)
                .HasColumnName("client_email");
            entity.Property(e => e.ClientName)
                .HasMaxLength(20)
                .HasColumnName("client_name");
            entity.Property(e => e.ClientPhone)
                .HasMaxLength(30)
                .HasColumnName("client_phone");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotation_items_pkey");

            entity.ToTable("quotation_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ColorId).HasColumnName("color_id");
            entity.Property(e => e.EndowmentId).HasColumnName("endowment_id");
            entity.Property(e => e.ImageItemName).HasColumnName("image_item_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.QuotationId).HasColumnName("quotation_id");
            entity.Property(e => e.SizeId).HasColumnName("size_id");

            entity.HasOne(d => d.Color).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("quotation_items_color_id_fkey");

            entity.HasOne(d => d.Endowment).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.EndowmentId)
                .HasConstraintName("quotation_items_endowment_id_fkey");

            entity.HasOne(d => d.Quotation).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.QuotationId)
                .HasConstraintName("quotation_items_quotation_id_fkey");

            entity.HasOne(d => d.Size).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("quotation_items_size_id_fkey");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("endowment_size_pkey");

            entity.ToTable("size");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('endowment_size_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });
        modelBuilder.HasSequence("endowment_brand_seq").IncrementsBy(50);
        modelBuilder.HasSequence("endowment_category_seq").IncrementsBy(50);
        modelBuilder.HasSequence("endowment_color_seq").IncrementsBy(50);
        modelBuilder.HasSequence("endowment_seq").IncrementsBy(50);
        modelBuilder.HasSequence("endowment_size_seq").IncrementsBy(50);
        modelBuilder.HasSequence("endowment_type_seq").IncrementsBy(50);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
