﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WaveMaster_Backend.Models;

#nullable disable

namespace WaveMaster_Backend.Migrations
{
    [DbContext(typeof(WaveMasterDbContext))]
    [Migration("20231121121332_UpdatedID")]
    partial class UpdatedID
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WaveMaster_Backend.Models.PlotData", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("time")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("voltage")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("id");

                    b.ToTable("plotDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
