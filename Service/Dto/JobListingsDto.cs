using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.models;

namespace Service.Dto
{
    public class JobListingsDto
    {

        //public int Id { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public string Location { get; set; } // מיקום המשימה
        //public decimal Payment { get; set; } // שכר למשימה
        //public DateTime RequiredDate { get; set; } // מתי זה קורה
        //public bool IsCatch { get; set; }
        //public bool IsRemote { get; set; } // האם המשרה היא מהבית?
        //public bool IsJobWithPepole { get; set; }
        public int Id { get; set; }

        // חובה להוסיף את שני אלו כדי שהמידע מה-Swagger יתקבל
        public int EmployerId { get; set; }
        public int CategoryId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Payment { get; set; }
        public DateTime RequiredDate { get; set; }
        public bool IsCatch { get; set; }
        public bool IsRemote { get; set; }
        public bool IsJobWithPepole { get; set; }
        public elevel leveJob { get; set; } = Repository.models.elevel.Easy; // ערך ברירת מחדל


    }
}
