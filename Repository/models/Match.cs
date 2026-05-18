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

        [Column("JobId")]
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual JobListings Job { get; set; }
        public int CandidateId { get; set; } // FK למועמד
        [ForeignKey("CandidateId")]
        public CandidateProfiles Candidate { get; set; }
        public double MatchScore { get; set; } // אחוז התאמה (למשל 95.5)
        public DateTime MatchDate { get; set; } // תאריך ריצת האלגוריתם
        public bool IsSelectedByAlgorithm { get; set; }
        public string Status { get; set; } = "pending"; // סטטוס המאץ' (pending, accepted, rejected)


    }
}


