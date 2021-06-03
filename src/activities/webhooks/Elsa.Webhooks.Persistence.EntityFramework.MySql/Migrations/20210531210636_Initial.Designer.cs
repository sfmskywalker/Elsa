﻿// <auto-generated />
using Elsa.Webhooks.Persistence.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Elsa.Webhooks.Persistence.EntityFramework.MySql.Migrations
{
    [DbContext(typeof(WebhookContext))]
    [Migration("20210531210636_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("Elsa.Webhooks.Abstractions.Models.WebhookDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DefinitionId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PayloadTypeName")
                        .HasColumnType("longtext");

                    b.Property<string>("TenantId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("WebhookDefinitions");
                });
#pragma warning restore 612, 618
        }
    }
}