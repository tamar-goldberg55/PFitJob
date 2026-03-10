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
            await context.Categories.ToList().Remove(await GetById(id));
            context.save();
        }

        public Task<List<Categories>> GetAll()
        {
            return context.Categories.ToList();
        }

        public Task<Categories> GetById(int id)
        {
            return context.Categories.ToList().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItem(int id, Categories item)
        {
            var category = GetById(id);
            category.Name = item.Name;
            _context.save();
        }
    }
}
