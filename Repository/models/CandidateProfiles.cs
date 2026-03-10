using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
    public enum elevel
    {
        Easy,   // 0
        Medium, // 1
        Hard    // 2
    };
   
    public class CandidateProfiles
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } // FK למשתמש
        public User User { get; set; }
        public string City { get; set; } // עיר מגורים
        public int MaxDistance { get; set; } // מרחק מקסימלי בק"מ
        public decimal MinHourlyRate { get; set; } // שכר מינימום מבוקש
        public bool activity { get; set; }
        public elevel level { get; set; }
        public bool IsRemoteOnly { get; set; } // האם המועמד מעוניין רק בעבודה מרחוק?
        public bool Withpepole { get; set; }
    }
}
