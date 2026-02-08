using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using server.Core.Entities;

namespace server.Data.configurations
{
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            // Set khóa chính
            builder.HasKey(ud => ud.Id);

            // Token là bắt buộc và tối đa 500 ký tự (ví dụ)
            builder.Property(ud => ud.FcmToken)
                .IsRequired()
                .HasMaxLength(500);

            // Quan hệ: 1 User có N Devices
            // Khi xóa User -> Xóa luôn các Devices của họ (Cascade)
            builder.HasOne(ud => ud.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}