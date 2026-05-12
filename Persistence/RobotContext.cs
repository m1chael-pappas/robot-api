using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotContext : DbContext
{
    public virtual DbSet<Map> Maps { get; set; } = null!;
    public virtual DbSet<RobotCommand> RobotCommands { get; set; } = null!;

    public RobotContext() { }

    public RobotContext(DbContextOptions<RobotContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>(entity =>
        {
            entity.ToTable("map");

            entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

            entity.Property(e => e.Columns).HasColumnName("columns");

            entity
                .Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");

            entity.Property(e => e.Description).HasMaxLength(800).HasColumnName("description");

            entity
                .Property(e => e.IsSquare)
                .HasColumnName("is_square")
                .HasComputedColumnSql("((rows > 0) AND (rows = columns))", true);

            entity
                .Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");

            entity.Property(e => e.Rows).HasColumnName("rows");
        });

        modelBuilder.Entity<RobotCommand>(entity =>
        {
            entity.ToTable("robot_command");

            entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

            entity
                .Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");

            entity.Property(e => e.Description).HasMaxLength(800).HasColumnName("description");

            entity.Property(e => e.IsMoveCommand).HasColumnName("is_move_command");

            entity
                .Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
        });
    }
}
