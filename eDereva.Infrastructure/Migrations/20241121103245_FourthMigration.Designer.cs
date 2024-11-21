﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eDereva.Infrastructure.Data;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241121103245_FourthMigration")]
    partial class FourthMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UsersNin")
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("RolesId", "UsersNin");

                    b.HasIndex("UsersNin");

                    b.ToTable("RoleUser");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Answer", b =>
                {
                    b.Property<Guid>("ChoiceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Choice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Choices");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Contingency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContingencyExplanation")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ContingencyType")
                        .HasColumnType("int");

                    b.Property<Guid?>("VenueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VenueId");

                    b.ToTable("Contingencies");
                });

            modelBuilder.Entity("eDereva.Core.Entities.District", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid?>("RegionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("Districts");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Otp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Otps");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Flags")
                        .HasColumnType("int");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId")
                        .IsUnique();

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.PrimitiveCollection<string>("ImageUrls")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("QuestionBankId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scenario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionBankId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.QuestionBank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("QuestionBanks");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Region", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<Guid?>("ContingencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("VenueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContingencyId");

                    b.HasIndex("VenueId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.User", b =>
                {
                    b.Property<string>("Nin")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.HasKey("Nin");

                    b.HasIndex("FirstName", "MiddleName", "LastName")
                        .IsUnique()
                        .HasFilter("[MiddleName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Venue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<Guid>("DistrictId")
                        .HasColumnType("uniqueidentifier");

                    b.PrimitiveCollection<string>("ImageUrls")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DistrictId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("eDereva.Core.Entities.VenueExemption", b =>
                {
                    b.Property<Guid>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExemptionDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasBeenApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("HasBeenExempted")
                        .HasColumnType("bit");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("VenueId");

                    b.ToTable("VenueExemptions");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eDereva.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersNin")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("eDereva.Core.Entities.Choice", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Question", "Question")
                        .WithMany("Choices")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Contingency", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Venue", null)
                        .WithMany("Contingencies")
                        .HasForeignKey("VenueId");
                });

            modelBuilder.Entity("eDereva.Core.Entities.District", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Region", "Region")
                        .WithMany("Districts")
                        .HasForeignKey("RegionId");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Permission", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Role", "Role")
                        .WithOne("Permission")
                        .HasForeignKey("eDereva.Core.Entities.Permission", "RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Question", b =>
                {
                    b.HasOne("eDereva.Core.Entities.QuestionBank", "QuestionBank")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionBankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionBank");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Session", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Contingency", "Contingency")
                        .WithMany("AffectedSessions")
                        .HasForeignKey("ContingencyId");

                    b.HasOne("eDereva.Core.Entities.Venue", "Venue")
                        .WithMany("Sessions")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contingency");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Venue", b =>
                {
                    b.HasOne("eDereva.Core.Entities.District", "District")
                        .WithMany("Venues")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("District");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Contingency", b =>
                {
                    b.Navigation("AffectedSessions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.District", b =>
                {
                    b.Navigation("Venues");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Question", b =>
                {
                    b.Navigation("Choices");
                });

            modelBuilder.Entity("eDereva.Core.Entities.QuestionBank", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Region", b =>
                {
                    b.Navigation("Districts");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Role", b =>
                {
                    b.Navigation("Permission");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Venue", b =>
                {
                    b.Navigation("Contingencies");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
