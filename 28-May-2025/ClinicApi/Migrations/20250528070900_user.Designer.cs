﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SecondWebApi.Contexts;

#nullable disable

namespace SecondWebApi.Migrations
{
    [DbContext(typeof(ClinicContext))]
    [Migration("20250528070900_user")]
    partial class user
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SecondWebApi.Models.Appointment", b =>
                {
                    b.Property<string>("AppointmnetNumber")
                        .HasColumnType("text");

                    b.Property<DateTime>("AppointmnetDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DoctorId")
                        .HasColumnType("integer");

                    b.Property<int>("PatientId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AppointmnetNumber")
                        .HasName("PK_Appointment_Number");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("appointmnets");
                });

            modelBuilder.Entity("SecondWebApi.Models.Doctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("YearsOfExperience")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("doctors");
                });

            modelBuilder.Entity("SecondWebApi.Models.DoctorSpeciality", b =>
                {
                    b.Property<int>("SerialNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SerialNumber"));

                    b.Property<int>("DoctorId")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialityId")
                        .HasColumnType("integer");

                    b.HasKey("SerialNumber");

                    b.HasIndex("DoctorId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("doctorSpecialities");
                });

            modelBuilder.Entity("SecondWebApi.Models.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("patients");
                });

            modelBuilder.Entity("SecondWebApi.Models.Speciality", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("specialities");
                });

            modelBuilder.Entity("SecondWebApi.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<int>("FollwerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("FollwerId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SecondWebApi.Models.Appointment", b =>
                {
                    b.HasOne("SecondWebApi.Models.Doctor", "Doctor")
                        .WithMany("Appointmnets")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Appoinment_Doctor");

                    b.HasOne("SecondWebApi.Models.Patient", "Patient")
                        .WithMany("Appointmnets")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Appoinment_Patient");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("SecondWebApi.Models.DoctorSpeciality", b =>
                {
                    b.HasOne("SecondWebApi.Models.Doctor", "Doctor")
                        .WithMany("DoctorSpecialities")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Speciality_Doctor");

                    b.HasOne("SecondWebApi.Models.Speciality", "Speciality")
                        .WithMany("DoctorSpecialities")
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Speciality_Spec");

                    b.Navigation("Doctor");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("SecondWebApi.Models.User", b =>
                {
                    b.HasOne("SecondWebApi.Models.User", "UserFollower")
                        .WithMany("Followers")
                        .HasForeignKey("FollwerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Followers");

                    b.Navigation("UserFollower");
                });

            modelBuilder.Entity("SecondWebApi.Models.Doctor", b =>
                {
                    b.Navigation("Appointmnets");

                    b.Navigation("DoctorSpecialities");
                });

            modelBuilder.Entity("SecondWebApi.Models.Patient", b =>
                {
                    b.Navigation("Appointmnets");
                });

            modelBuilder.Entity("SecondWebApi.Models.Speciality", b =>
                {
                    b.Navigation("DoctorSpecialities");
                });

            modelBuilder.Entity("SecondWebApi.Models.User", b =>
                {
                    b.Navigation("Followers");
                });
#pragma warning restore 612, 618
        }
    }
}
