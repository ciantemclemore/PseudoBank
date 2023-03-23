﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PseudoBank.Payments.Runner.Context;

#nullable disable

namespace PseudoBank.Payments.Runner.Migrations
{
    [DbContext(typeof(AccountContext))]
    [Migration("20230219020748_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("AccountPaymentScheme", b =>
                {
                    b.Property<Guid>("AccountsId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AllowedPaymentSchemesId")
                        .HasColumnType("TEXT");

                    b.HasKey("AccountsId", "AllowedPaymentSchemesId");

                    b.HasIndex("AllowedPaymentSchemesId");

                    b.ToTable("AccountPaymentScheme");
                });

            modelBuilder.Entity("PseudoBank.Payments.Runner.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountStatusId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountStatusId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PseudoBank.Payments.Runner.Entities.AccountStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccountStatuses");
                });

            modelBuilder.Entity("PseudoBank.Payments.Runner.Entities.PaymentScheme", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("SchemeName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PaymentSchemes");
                });

            modelBuilder.Entity("AccountPaymentScheme", b =>
                {
                    b.HasOne("PseudoBank.Payments.Runner.Entities.Account", null)
                        .WithMany()
                        .HasForeignKey("AccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PseudoBank.Payments.Runner.Entities.PaymentScheme", null)
                        .WithMany()
                        .HasForeignKey("AllowedPaymentSchemesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PseudoBank.Payments.Runner.Entities.Account", b =>
                {
                    b.HasOne("PseudoBank.Payments.Runner.Entities.AccountStatus", "AccountStatus")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStatus");
                });

            modelBuilder.Entity("PseudoBank.Payments.Runner.Entities.AccountStatus", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}