using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; } // הסיסמה בטקסט גלוי מהלקוח
        public UserRole UserType { get; set; } // מועמד או מעסיק
    }
}
