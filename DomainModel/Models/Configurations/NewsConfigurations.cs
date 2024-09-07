using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Models.Configurations
{
    public class NewsConfigurations:IEntityTypeConfiguration<News>
    {

        public void Configure(EntityTypeBuilder<News> builder)
        {

        }
    }
}
