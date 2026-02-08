using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Common
{
    public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid(); // Dùng Guid để khó đoán ID hơn Int
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false; // Soft Delete (Xóa mềm)
}
}