using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Enums
{
    public enum JobApplicationStatus
    {
        Applied = 0,
        UnderReview = 1,
        Interview = 2,
        Accepted = 3,
        Rejected = 4,
        Cancelled = 5
    }
}
