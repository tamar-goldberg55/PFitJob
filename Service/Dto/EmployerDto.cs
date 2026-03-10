using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
  public class EmployerDto {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CompanyName { get; set; }
    public bool Status { get; set; }
}
}
