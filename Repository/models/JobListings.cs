using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
   
    public class JobListings
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employer")]
        public int EmployerId { get; set; } // FK למעסיק (מי פרסם)
        public Employer Employer { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; } // FK לקטגוריה (סוג העבודה)
        public Categories Category { get; set; }
        public elevel leveJob { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; } // מיקום המשימה
        public decimal Payment { get; set; } // שכר למשימה
        public DateTime RequiredDate { get; set; } // מתי זה קורה
        public bool IsCatch { get; set; }
        public bool IsRemote { get; set; } // האם המשרה היא מהבית?
        public bool IsJobWithPepole { get; set; }
        



    }
}
