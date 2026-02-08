using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Common;

namespace server.Core.Entities
{
    public class Photo : BaseEntity
{
    public Guid SenderId { get; set; }
    public User Sender { get; set; }

    public string ImageUrl { get; set; } // URL ảnh sau khi upload storage
    public string? Caption { get; set; } // Dòng chữ nhỏ kèm theo ảnh
    
    // Metadata kỹ thuật (để resize hoặc hiển thị đúng tỷ lệ)
    public int Width { get; set; }
    public int Height { get; set; }
}
}