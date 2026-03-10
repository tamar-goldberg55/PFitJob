using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
    public enum UserRole///לא הבנו איך ז מאובטח 
    {
        Candidate = 1,  // מועמד
        Employer = 2,   // מעסיק
        Admin = 3       // מנהל
    }
    public class User
    {
        [Key]
        public int  Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }  
        public string PasswordHash { get; set; }
        public UserRole UserType { get; set; }
        public bool IsEnable { get; set; }
      
    }
}
