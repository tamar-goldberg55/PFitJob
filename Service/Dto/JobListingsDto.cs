using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class JobListingsDto
    {

        public int Id { get; set; }
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
