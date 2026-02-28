using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using server.Core.Entities;

namespace server.Data.configurations
{
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            // Đánh Index cho SenderId để query Feed nhanh hơn
            builder.HasIndex(p => p.SenderId);

            // Đánh Index cho CreatedAt vì chúng ta thường xuyên sắp xếp theo thời gian
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}