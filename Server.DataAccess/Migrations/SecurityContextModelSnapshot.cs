﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Server.DataAccess.Context;
using System;

namespace Server.DataAccess.Migrations
{
    [DbContext(typeof(SecurityContext))]
    partial class SecurityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-preview2-25794")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Server.DataAccess.Model.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Server.DataAccess.Model.CashAccount", b =>
                {
                    b.Property<int>("CashAccountId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<decimal>("Amount");

                    b.Property<int>("CurrencyId");

                    b.Property<bool>("IsJointCashAccount");

                    b.Property<string>("Name");

                    b.Property<int?>("UserId");

                    b.HasKey("CashAccountId");

                    b.HasIndex("AccountId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("CashAccount");
                });

            modelBuilder.Entity("Server.DataAccess.Model.Cashflow", b =>
                {
                    b.Property<int>("CashflowId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int?>("CashAccountId");

                    b.Property<int?>("CashflowCategoryId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<int?>("UserId");

                    b.HasKey("CashflowId");

                    b.HasIndex("CashAccountId");

                    b.HasIndex("CashflowCategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("Cashflow");
                });

            modelBuilder.Entity("Server.DataAccess.Model.CashflowCategory", b =>
                {
                    b.Property<int>("CashflowCategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("Name");

                    b.HasKey("CashflowCategoryId");

                    b.HasIndex("AccountId");

                    b.ToTable("CashflowCategory");
                });

            modelBuilder.Entity("Server.DataAccess.Model.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("CurrencyId");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("Server.DataAccess.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("Name");

                    b.HasKey("UserId");

                    b.HasIndex("AccountId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Server.DataAccess.Model.CashAccount", b =>
                {
                    b.HasOne("Server.DataAccess.Model.Account", "Account")
                        .WithMany("CashAccounts")
                        .HasForeignKey("AccountId");

                    b.HasOne("Server.DataAccess.Model.Currency", "Currency")
                        .WithMany("CashAccounts")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Server.DataAccess.Model.User", "User")
                        .WithMany("CashAccounts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Server.DataAccess.Model.Cashflow", b =>
                {
                    b.HasOne("Server.DataAccess.Model.CashAccount", "CashAccount")
                        .WithMany("Cashflows")
                        .HasForeignKey("CashAccountId");

                    b.HasOne("Server.DataAccess.Model.CashflowCategory", "CashflowCategory")
                        .WithMany("Cashflows")
                        .HasForeignKey("CashflowCategoryId");

                    b.HasOne("Server.DataAccess.Model.User", "User")
                        .WithMany("Cashflows")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Server.DataAccess.Model.CashflowCategory", b =>
                {
                    b.HasOne("Server.DataAccess.Model.Account", "Account")
                        .WithMany("CashflowCategories")
                        .HasForeignKey("AccountId");
                });

            modelBuilder.Entity("Server.DataAccess.Model.User", b =>
                {
                    b.HasOne("Server.DataAccess.Model.Account", "Account")
                        .WithMany("Users")
                        .HasForeignKey("AccountId");
                });
#pragma warning restore 612, 618
        }
    }
}
