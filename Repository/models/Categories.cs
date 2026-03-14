using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.models
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }
            public string Name { get; set; }

            // קשרים חזרה (בשביל ה-Entity Framework)
            public List<JobListings>? Jobs { get; set; }
           
        
    }
}
