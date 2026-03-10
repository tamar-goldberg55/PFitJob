using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
    public class Employer
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } // FK למשתמש
        public User User { get; set; }

        public string CompanyName { get; set; }

        // קשר גומלין: 1 לרבים (מעסיק אחד מחזיק רשימת משימות)
        public List<JobListings> MyJobs { get; set; } = new List<JobListings>();
        public bool status { get; set; }
        
    }
}
