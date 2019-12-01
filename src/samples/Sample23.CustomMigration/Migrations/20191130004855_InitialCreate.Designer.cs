﻿// <auto-generated />
using System;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sample23.CustomMigration.Migrations
{
    [DbContext(typeof(ElsaContext))]
    [Migration("20191130004855_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("elsa")
                .HasAnnotation("ProductVersion", "3.0.1");

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityDefinitionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivityId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Left")
                        .HasColumnType("INTEGER");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.Property<int>("Top")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WorkflowDefinitionVersionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionVersionId");

                    b.ToTable("ActivityDefinitions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityInstanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivityId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Output")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WorkflowInstanceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceId");

                    b.ToTable("ActivityInstances");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.BlockingActivityEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivityId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActivityType")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WorkflowInstanceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceId");

                    b.ToTable("BlockingActivities");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ConnectionDefinitionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DestinationActivityId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Outcome")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceActivityId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WorkflowDefinitionVersionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionVersionId");

                    b.ToTable("ConnectionDefinitions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DefinitionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLatest")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSingleton")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Variables")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VersionId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WorkflowDefinitionVersions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AbortedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CorrelationId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DefinitionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExecutionLog")
                        .HasColumnType("TEXT");

                    b.Property<string>("Fault")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("FaultedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("FinishedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Input")
                        .HasColumnType("TEXT");

                    b.Property<string>("InstanceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scopes")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("WorkflowInstances");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityDefinitionEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", "WorkflowDefinitionVersion")
                        .WithMany("Activities")
                        .HasForeignKey("WorkflowDefinitionVersionId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityInstanceEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", "WorkflowInstance")
                        .WithMany("Activities")
                        .HasForeignKey("WorkflowInstanceId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.BlockingActivityEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", "WorkflowInstance")
                        .WithMany("BlockingActivities")
                        .HasForeignKey("WorkflowInstanceId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ConnectionDefinitionEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", "WorkflowDefinitionVersion")
                        .WithMany("Connections")
                        .HasForeignKey("WorkflowDefinitionVersionId");
                });
#pragma warning restore 612, 618
        }
    }
}
