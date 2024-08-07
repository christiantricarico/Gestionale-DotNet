﻿// <auto-generated />
using System;
using Gdn.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Gdn.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240709144542_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Gdn.Domain.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ProductCategoryId")
                        .HasColumnType("int");

                    b.Property<decimal>("Stock")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<int?>("TaxRateId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProductCategoryId");

                    b.HasIndex("TaxRateId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Gdn.Domain.Models.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Gdn.Domain.Models.TaxRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal>("Rate")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<int?>("TaxRateNatureId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("TaxRateNatureId");

                    b.ToTable("TaxRates");
                });

            modelBuilder.Entity("Gdn.Domain.Models.TaxRateNature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("TaxRateNatures");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "N1",
                            IsDeleted = false,
                            Name = "N1 : Escluso art.15"
                        },
                        new
                        {
                            Id = 2,
                            Code = "N2",
                            IsDeleted = false,
                            Name = "N2 : Non soggette"
                        },
                        new
                        {
                            Id = 3,
                            Code = "N2.1",
                            IsDeleted = false,
                            Name = "N2.1 : Non soggette artt. Da 7 a 7-septies"
                        },
                        new
                        {
                            Id = 4,
                            Code = "N2.2",
                            IsDeleted = false,
                            Name = "N2.2 : Non soggette - altri casi"
                        },
                        new
                        {
                            Id = 5,
                            Code = "N3",
                            IsDeleted = false,
                            Name = "N2.2 : Non soggette - altri casi"
                        },
                        new
                        {
                            Id = 6,
                            Code = "N3.1",
                            IsDeleted = false,
                            Name = "N3.1 : Non imponibili - esportazioni"
                        },
                        new
                        {
                            Id = 7,
                            Code = "N3.2",
                            IsDeleted = false,
                            Name = "N3.2 : Non imponibili - cessioni intracomunitarie"
                        },
                        new
                        {
                            Id = 8,
                            Code = "N3.3",
                            IsDeleted = false,
                            Name = "N3.3 : Non imponibili - cessioni verso San Marino"
                        },
                        new
                        {
                            Id = 9,
                            Code = "N3.4",
                            IsDeleted = false,
                            Name = "N3.4 : Non imponibili - op. assimilate alle esportazioni"
                        },
                        new
                        {
                            Id = 10,
                            Code = "N3.5",
                            IsDeleted = false,
                            Name = "N3.5 : Non imponibili - a seguito di dichiarazioni d?intento"
                        },
                        new
                        {
                            Id = 11,
                            Code = "N3.6",
                            IsDeleted = false,
                            Name = "N3.6 : Non imponibili - altre op. no plafond"
                        },
                        new
                        {
                            Id = 12,
                            Code = "N4",
                            IsDeleted = false,
                            Name = "N4 : Esenti"
                        },
                        new
                        {
                            Id = 13,
                            Code = "N5",
                            IsDeleted = false,
                            Name = "N5 : Regime del margine / IVA non esposta"
                        },
                        new
                        {
                            Id = 14,
                            Code = "N6",
                            IsDeleted = false,
                            Name = "N6 : Inversione contabile"
                        },
                        new
                        {
                            Id = 15,
                            Code = "N6.1",
                            IsDeleted = false,
                            Name = "N6.1 : Inversione contabile - cessione di rottami"
                        },
                        new
                        {
                            Id = 16,
                            Code = "N6.2",
                            IsDeleted = false,
                            Name = "N6.2 : Inversione contabile - cessione di oro e argento puro"
                        },
                        new
                        {
                            Id = 17,
                            Code = "N6.3",
                            IsDeleted = false,
                            Name = "N6.3 : Inversione contabile - subappalto nel settore edile"
                        },
                        new
                        {
                            Id = 18,
                            Code = "N6.4",
                            IsDeleted = false,
                            Name = "N6.4 : Inversione contabile - cessione di fabbricati"
                        },
                        new
                        {
                            Id = 19,
                            Code = "N6.5",
                            IsDeleted = false,
                            Name = "N6.5 : Inversione contabile - cessione di telefoni cellulari"
                        },
                        new
                        {
                            Id = 20,
                            Code = "N6.6",
                            IsDeleted = false,
                            Name = "N6.6 : Inversione contabile - cessione di prodotti elettronici"
                        },
                        new
                        {
                            Id = 21,
                            Code = "N6.7",
                            IsDeleted = false,
                            Name = "N6.7 : Inversione contabile - prestazioni comparto edile"
                        },
                        new
                        {
                            Id = 22,
                            Code = "N6.8",
                            IsDeleted = false,
                            Name = "N6.8 : Inversione contabile - op. settore energetico"
                        },
                        new
                        {
                            Id = 23,
                            Code = "N6.9",
                            IsDeleted = false,
                            Name = "N6.9 : Inversione contabile - altri casi"
                        },
                        new
                        {
                            Id = 24,
                            Code = "N7",
                            IsDeleted = false,
                            Name = "N7 : IVA assolta in altro stato UE"
                        });
                });

            modelBuilder.Entity("Gdn.Domain.Models.Product", b =>
                {
                    b.HasOne("Gdn.Domain.Models.ProductCategory", "ProductCategory")
                        .WithMany()
                        .HasForeignKey("ProductCategoryId");

                    b.HasOne("Gdn.Domain.Models.TaxRate", "TaxRate")
                        .WithMany()
                        .HasForeignKey("TaxRateId");

                    b.Navigation("ProductCategory");

                    b.Navigation("TaxRate");
                });

            modelBuilder.Entity("Gdn.Domain.Models.ProductCategory", b =>
                {
                    b.HasOne("Gdn.Domain.Models.ProductCategory", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Gdn.Domain.Models.TaxRate", b =>
                {
                    b.HasOne("Gdn.Domain.Models.TaxRateNature", "TaxRateNature")
                        .WithMany()
                        .HasForeignKey("TaxRateNatureId");

                    b.Navigation("TaxRateNature");
                });
#pragma warning restore 612, 618
        }
    }
}
