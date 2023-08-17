﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductGrpc.Data;

#nullable disable

namespace ProductGrpc.Migrations
{
    [DbContext(typeof(ProductsContext))]
    partial class ProductsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductGrpc.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CreatedTime = new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9427),
                            Description = "New Xiaomi Phone Mi10T",
                            Name = "Mi10T",
                            Price = 699f,
                            Status = 0
                        },
                        new
                        {
                            ProductId = 2,
                            CreatedTime = new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9434),
                            Description = "New Huawei Phone P40",
                            Name = "P40",
                            Price = 899f,
                            Status = 0
                        },
                        new
                        {
                            ProductId = 3,
                            CreatedTime = new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9436),
                            Description = "New Samsung Phone A50",
                            Name = "A50",
                            Price = 399f,
                            Status = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
