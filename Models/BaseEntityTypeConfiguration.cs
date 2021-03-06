﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimeTheme.Service.Models
{
    public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property<DateTime>("InsertDateTime")
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow)
                .ValueGeneratedOnAdd();
            builder.Property<DateTime>("UpdateDateTime")
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow)
                .ValueGeneratedOnAdd();
        }
    }
}