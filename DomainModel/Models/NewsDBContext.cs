using DomainModel.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DomainModel.Models
{
    public class NewsDBContext:DbContext
    {
        public NewsDBContext(DbContextOptions<NewsDBContext> options) : base(options)
        {

        }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<News> News { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NewsCategoryConfigurations());
            modelBuilder.ApplyConfiguration(new NewsConfigurations());
            base.OnModelCreating(modelBuilder);
        }
    }
}
