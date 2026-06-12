using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class CandidateProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; } // משאירים את ה-ID כדי לדעת למי הפרופיל שייך

        public string City { get; set; }
        public int MaxDistance { get; set; } // החזרנו את המרחק
        public decimal MinHourlyRate { get; set; }
        public bool Activity { get; set; } // האם הפרופיל פעיל

        public elevel Level { get; set; } // שליחת ה-Enum כטקסט (למשל "Easy")
        public int LevelValue { get; set; } // שליחת ה-Enum כמספר (למשל 0) - עוזר לחישובים ב-React

        public bool IsRemoteOnly { get; set; }
        public bool WithPeople { get; set; } // החזרנו את השדה (עם תיקון שמי קטן)
    }
}
