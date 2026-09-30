using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string RecruiterId { get; set; } = string.Empty;

        public DateTime? ClosedAt { get; set; }

        public string? ClosedBy { get; set; }
    }
}
