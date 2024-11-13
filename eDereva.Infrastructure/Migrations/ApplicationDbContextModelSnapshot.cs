﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eDereva.Infrastructure.Data;

#nullable disable

namespace eDereva.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UsersNIN")
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("RolesId", "UsersNIN");

                    b.HasIndex("UsersNIN");

                    b.ToTable("RoleUser");
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

                    b.Property<bool>("CanDeleteQuestionBanks")
                        .HasColumnType("bit");

                    b.Property<bool>("CanDeleteTests")
                        .HasColumnType("bit");

                    b.Property<bool>("CanDeleteUsers")
                        .HasColumnType("bit");

                    b.Property<bool>("CanDeleteVenues")
                        .HasColumnType("bit");

                    b.Property<bool>("CanEditQuestionBanks")
                        .HasColumnType("bit");

                    b.Property<bool>("CanEditTests")
                        .HasColumnType("bit");

                    b.Property<bool>("CanEditUsers")
                        .HasColumnType("bit");

                    b.Property<bool>("CanEditVenues")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManageQuestionBanks")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManageTests")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManageUsers")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManageVenues")
                        .HasColumnType("bit");

                    b.Property<bool>("CanViewQuestionBanks")
                        .HasColumnType("bit");

                    b.Property<bool>("CanViewSoftDeletedData")
                        .HasColumnType("bit");

                    b.Property<bool>("CanViewTests")
                        .HasColumnType("bit");

                    b.Property<bool>("CanViewUsers")
                        .HasColumnType("bit");

                    b.Property<bool>("CanViewVenues")
                        .HasColumnType("bit");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("eDereva.Core.Entities.User", b =>
                {
                    b.Property<string>("NIN")
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

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

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

                    b.HasKey("NIN");

                    b.HasIndex("FirstName", "MiddleName", "LastName")
                        .IsUnique()
                        .HasFilter("[MiddleName] IS NOT NULL");

                    b.ToTable("Users");
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
                        .HasForeignKey("UsersNIN")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("eDereva.Core.Entities.Permission", b =>
                {
                    b.HasOne("eDereva.Core.Entities.Role", null)
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("eDereva.Core.Entities.Role", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
