using Repository.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class MatchDto
    {
        public int Id { get; set; }

        [ForeignKey("JobListings")]
        public int JobId { get; set; } // FK למשרה
        public JobListingsDto? Job { get; set; }
        [ForeignKey("CandidateProfiles")]
        public int CandidateId { get; set; } // FK למועמד
        public CandidateProfiles? Candidate { get; set; }
        public double MatchScore { get; set; } // אחוז התאמה (למשל 95.5)
        public DateTime MatchDate { get; set; } // תאריך ריצת האלגוריתם
        public string Status { get; set; } = "pending"; // סטטוס המאץ' (pending, accepted, rejected)
        public bool IsSelectedByAlgorithm { get; set; }
    }
}
