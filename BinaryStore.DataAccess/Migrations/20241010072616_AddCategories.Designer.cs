﻿// <auto-generated />
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BinaryDecimalStore.Migrations
{
    [DbContext(typeof(BinaryStoreDbContext))]
    [Migration("20241010072616_AddCategories")]
    partial class AddCategories
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BinaryDecimalStore.Models.Categorey", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Categoreies");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Description = "a Modern Phones From Apple",
                            DisplayOrder = 1,
                            name = "iphone"
                        },
                        new
                        {
                            ID = 2,
                            Description = "a Modern HeadPhones From Apple",
                            DisplayOrder = 3,
                            name = "AirPods"
                        },
                        new
                        {
                            ID = 3,
                            Description = "a Modern Laptop From Apple",
                            DisplayOrder = 2,
                            name = "MacBook"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
