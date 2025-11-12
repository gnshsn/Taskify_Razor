using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Taskify.Models
{
    public enum TaskPriority { Low, Medium, High }
    public enum TaskStatus { Pending, InProgress, Completed }
    public class TodoItems
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // Foreign keys
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Ownership
        public string? OwnerId { get; set; }
        public IdentityUser? Owner { get; set; }
    }
}
