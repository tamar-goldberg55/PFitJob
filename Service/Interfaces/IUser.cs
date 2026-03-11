using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUser:IService<UserDto>
    {
        // פונקציית רישום - מקבלת DTO ומחזירה את המשתמש שנוצר (או Token)
        Task<string> RegisterAsync(UserDto userDto, string password);

        // פונקציית התחברות - מקבלת פרטי כניסה ומחזירה את פרטי המשתמש
        Task<string> LoginAsync(string email, string password);

    }
}
