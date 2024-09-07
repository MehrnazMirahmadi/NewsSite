using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Models.Configurations
{
    public class NewsCategoryConfigurations : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            builder.HasMany(x => x.Children)
                .WithOne(x => x.Parent)
                .HasForeignKey(x => x.ParentID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.News)
                .WithOne(x => x.NewsCategory)
                .HasForeignKey(x => x.NewsCategoryID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.SmallDescription).HasMaxLength(400);
            builder.Property(x => x.CategoryName).HasMaxLength(100);
            builder.Property(x => x.Slug).HasMaxLength(100);


        }
    }
}
