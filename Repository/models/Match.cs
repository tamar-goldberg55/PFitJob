using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("JobListings")]
        public int JobId { get; set; } // FK למשרה
        public JobListings Job { get; set; }
        [ForeignKey("CandidateProfiles")]
        public int CandidateId { get; set; } // FK למועמד
        public CandidateProfiles Candidate { get; set; }
        public double MatchScore { get; set; } // אחוז התאמה (למשל 95.5)
        public DateTime MatchDate { get; set; } // תאריך ריצת האלגוריתם

    }
}
