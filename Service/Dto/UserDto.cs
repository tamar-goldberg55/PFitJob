using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{

    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; } // הופכים את ה-Enum לטקסט
        public bool IsEnable { get; set; }
    }
}
