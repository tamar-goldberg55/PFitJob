using AutoMapper;
using Repository.Interfaces;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CategoryService:ICategories
    {
        private readonly IRepository<Categories> _repository;
        private readonly IMapper mapper;
        public CategoryService(IRepository<Categories>repository,IMapper map)
        {
            _repository = repository;   
            mapper = map;   
        }
        public async Task<CategoriesDto> AddItem(CategoriesDto item)
        {
            return mapper.Map<Categories, CategoriesDto>(
           await _repository.AddItem(mapper.Map<CategoriesDto, Categories>(item)));
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public Task<List<CategoriesDto>> GetAll()
        {
            return mapper.Map<Task<List<Categories>>, Task<List<CategoriesDto>>>(_repository.GetAll());
        }

        public  async Task<CategoriesDto> GetById(int id)
        {
            return mapper.Map<Categories, CategoriesDto>(await _repository.GetById(id));
        }

        public Task UpdateItem(int id, CategoriesDto item)
        {
            throw new NotImplementedException();
        }
    }
}
