﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using ProaktifArizaTahmini.DAL;

#nullable disable

namespace ProaktifArizaTahmini.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240205113836_05.02.24")]
    partial class _050224
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.Disturbance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("CfgFileData")
                        .HasColumnType("CLOB")
                        .HasColumnName("Cfg_Dosyasi");

                    b.Property<string>("CfgFilePath")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Cfg_Dosya_Yolu");

                    b.Property<string>("ComtradeName")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Comtrade_Dosya_Ismi");

                    b.Property<byte[]>("DatFileData")
                        .IsRequired()
                        .HasColumnType("BLOB")
                        .HasColumnName("Dat_Dosyasi");

                    b.Property<string>("DatFilePath")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Dat_Dosya_Yolu");

                    b.Property<DateTime>("FaultTimeEnd")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Ariza_Saati_Bitis");

                    b.Property<DateTime>("FaultTimeStart")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Ariza_Saati_Baslangic");

                    b.Property<string>("FiderName")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Fider_Adi");

                    b.Property<string>("HucreNo")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Hucre_No");

                    b.Property<string>("IP")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("InstantDataPath")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Instant_Dosya_Yolu");

                    b.Property<DateTime>("PutTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Gönderilme_Tarihi");

                    b.Property<int>("RelayInformationId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("RmsDataPath")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Rms_Dosya_Yolu");

                    b.Property<string>("RoleModel")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Role_Model");

                    b.Property<bool>("Status")
                        .HasColumnType("NUMBER(1)");

                    b.Property<int>("TimeDifference")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Saat_Farki");

                    b.Property<string>("TmKvHucre")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Tm_KV_Hucre");

                    b.Property<string>("TmNo")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("TM_No");

                    b.Property<double>("TotalFaultTime")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("Ariza_Saati_Suresi");

                    b.Property<string>("kV")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("sFtpStatus")
                        .HasColumnType("NUMBER(1)")
                        .HasColumnName("SFTP_Durumu");

                    b.HasKey("ID");

                    b.HasIndex("RelayInformationId");

                    b.ToTable("Disturbances");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.HistoryOfChange", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("ChangedDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Degistirilme_Tarihi");

                    b.Property<string>("NewIP")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("New_IP");

                    b.Property<string>("OldIP")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Old_IP");

                    b.Property<int>("RelayInformationId")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("ID");

                    b.HasIndex("RelayInformationId");

                    b.ToTable("HistoryOfChanges");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.RelayInformation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("FiderName")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Fider_Adi");

                    b.Property<string>("HucreNo")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)")
                        .HasColumnName("Hucre_No");

                    b.Property<string>("IP")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("Port")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("RoleModel")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Role_Model");

                    b.Property<bool>("Status")
                        .HasColumnType("NUMBER(1)");

                    b.Property<int>("TimeDifference")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Saat_Farki");

                    b.Property<string>("TmKvHucre")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Tm_kV_Hucre");

                    b.Property<string>("TmNo")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)")
                        .HasColumnName("TM_No");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("kV")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("ID");

                    b.HasIndex("TmNo", "kV", "HucreNo")
                        .IsUnique();

                    b.ToTable("RelayInformations");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.ServiceLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Exception")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("LogLevel")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("ThreadID")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TIMESTAMP(7)");

                    b.HasKey("ID");

                    b.ToTable("ServiceLogs");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Adress")
                        .HasMaxLength(500)
                        .HasColumnType("NVARCHAR2(500)")
                        .HasColumnName("Adres");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Dogum_Tarihi");

                    b.Property<string>("Company")
                        .HasMaxLength(100)
                        .HasColumnType("NVARCHAR2(100)")
                        .HasColumnName("Şirket");

                    b.Property<string>("Departure")
                        .HasMaxLength(100)
                        .HasColumnType("NVARCHAR2(100)")
                        .HasColumnName("Departman");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("NVARCHAR2(200)");

                    b.Property<string>("Image")
                        .HasMaxLength(1000)
                        .HasColumnType("NVARCHAR2(1000)")
                        .HasColumnName("Resim");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Son_Giris_Tarihi");

                    b.Property<string>("Manager")
                        .HasMaxLength(500)
                        .HasColumnType("NVARCHAR2(500)")
                        .HasColumnName("Yönetici");

                    b.Property<string>("Mobile")
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)")
                        .HasColumnName("Cep_Telefonu");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("İsim");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)")
                        .HasColumnName("Telefon");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.Property<string>("Surname")
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("Soyisim");

                    b.Property<string>("TcIdentificationNumber")
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)")
                        .HasColumnName("TC_Kimlik");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("NVARCHAR2(100)")
                        .HasColumnName("Unvan");

                    b.Property<int?>("UserTypeId")
                        .IsRequired()
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("Kullanici_Adi");

                    b.HasKey("ID");

                    b.HasIndex("UserTypeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.UserLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Exception")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Hata");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("Tarih");

                    b.Property<string>("LogLevel")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Log_Seviyesi");

                    b.Property<string>("Message")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Aciklama");

                    b.Property<string>("MethodName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("Metot_Ismi");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.UserType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("Id");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("NVARCHAR2(500)");

                    b.Property<int>("UserTypeName")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("ID");

                    b.ToTable("UserTypes");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Description = "Domain",
                            UserTypeName = 1
                        },
                        new
                        {
                            ID = 2,
                            Description = "Misafir",
                            UserTypeName = 2
                        },
                        new
                        {
                            ID = 3,
                            Description = "Test",
                            UserTypeName = 3
                        },
                        new
                        {
                            ID = 4,
                            Description = "Kurum Dışı",
                            UserTypeName = 4
                        },
                        new
                        {
                            ID = 5,
                            Description = "Kurum İçi Domainsiz",
                            UserTypeName = 5
                        });
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.Disturbance", b =>
                {
                    b.HasOne("ProaktifArizaTahmini.CORE.Entities.RelayInformation", "RelayInformation")
                        .WithMany("Disturbances")
                        .HasForeignKey("RelayInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelayInformation");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.HistoryOfChange", b =>
                {
                    b.HasOne("ProaktifArizaTahmini.CORE.Entities.RelayInformation", "RelayInformation")
                        .WithMany("HistoryOfChanges")
                        .HasForeignKey("RelayInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelayInformation");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.User", b =>
                {
                    b.HasOne("ProaktifArizaTahmini.CORE.Entities.UserType", "UserType")
                        .WithMany("Users")
                        .HasForeignKey("UserTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserType");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.UserLog", b =>
                {
                    b.HasOne("ProaktifArizaTahmini.CORE.Entities.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.RelayInformation", b =>
                {
                    b.Navigation("Disturbances");

                    b.Navigation("HistoryOfChanges");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.User", b =>
                {
                    b.Navigation("UserLogs");
                });

            modelBuilder.Entity("ProaktifArizaTahmini.CORE.Entities.UserType", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
