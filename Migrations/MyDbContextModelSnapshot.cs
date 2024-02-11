﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace PillPallAPI.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0");

            modelBuilder.Entity("Container", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Containers");
                });

            modelBuilder.Entity("ContainerMedMap", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContainerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MedId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ContainerMedMaps");
                });

            modelBuilder.Entity("DispenseLog", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MedId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumPills")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DispenseLogs");
                });

            modelBuilder.Entity("Medication", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExpirationDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("MaxDosesPerDay")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MaxPillsPerDose")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NumPills")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PIN")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PharmacyAddr1")
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyAddr2")
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyCity")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PharmacyPhone")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PharmacyState")
                        .HasColumnType("TEXT");

                    b.Property<string>("PharmacyZip")
                        .HasColumnType("TEXT");

                    b.Property<string>("PrescriberName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Medications");
                });

            modelBuilder.Entity("Schedule", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PIN")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("ScheduleMed", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MedId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumPills")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ScheduleMeds");
                });

            modelBuilder.Entity("ScheduleTime", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ScheduleTimes");
                });

            modelBuilder.Entity("Time", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Times");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PIN")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
