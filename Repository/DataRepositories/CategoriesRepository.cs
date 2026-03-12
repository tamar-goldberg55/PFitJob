using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DataRepositories
{
    public class CategoriesRepository : IRepository<Categories>
    {
        private readonly IContext context;
        public CategoriesRepository()
        {
            this.context = context;
        }
        public async Task<Categories> AddItem(Categories item)
        {
            await context.Categories.AddAsync(item);
            context.save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            //await context.Categories.ToListAsync().Remove(await GetById(id));
            //context.save();
            // 1. קודם כל מקבלים את האובייקט בצורה אסינכרונית
            var item = await GetById(id);

            if (item != null)
            {
                // 2. מסירים אותו ישירות מה-Categories (בלי ToListAsync!)
                context.Categories.Remove(item);

                // 3. קוראים לפונקציית ה-save שלך (בלי await כי היא void)
                context.save();
            }

        }

        public  Task<List<Categories>> GetAll()
        {
            return context.Categories.ToListAsync();
        }

        public async Task<Categories> GetById(int id)
        {
           /* return await context.Categories.ToListAsync().FirstOrDefaultAsync(x => x.Id == id)*/;
            return await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItem(int id, Categories item)//האם צריכה לעדכן גם את הרשימה ?
        {
            var category = await GetById(id);
            category.Name = item.Name;
            context.save();
        }
    }
}
