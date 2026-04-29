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
        [ForeignKey("Category")]
        public int ?CategoryId { get; set; } // המזהה של הקטגוריה (למשל: 1 עבור תכנות)
        public Categories ?Category { get; set; } // האובייקט המלא של הקטגוריה
        [ForeignKey("User")]
        public int UserId { get; set; } // FK למשתמש
        public User ?User { get; set; }
        [Required(ErrorMessage = "שדה עיר הוא חובה")]
        public string City { get; set; } // עיר מגורים
        public int ?MaxDistance { get; set; } // מרחק מקסימלי בק"מ
        public decimal ?MinHourlyRate { get; set; } // שכר מינימום מבוקש
        public bool activity { get; set; } = true;
        public elevel level { get; set; } = elevel.Easy;
        public bool IsRemoteOnly { get; set; }// האם המועמד מעוניין רק בעבודה מרחוק?
        public bool Withpepole { get; set; } 
    }
}
