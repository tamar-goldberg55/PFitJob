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

        //public Task<List<CategoriesDto>> GetAll()
        //{
        //    return mapper.Map<Task<List<Categories>>, Task<List<CategoriesDto>>>(_repository.GetAll());
        //}
        public async Task<List<CategoriesDto>> GetAll()
        {
            // 1. קודם כל מקבלים את הרשימה מה-Repository (מחכים שה-Task יסתיים)
            var categories = await _repository.GetAll();

            // 2. עכשיו כשיש לנו רשימה ביד, הופכים אותה ל-DTO
            return mapper.Map<List<CategoriesDto>>(categories);
        }
        public  async Task<CategoriesDto> GetById(int id)
        {
            return mapper.Map<Categories, CategoriesDto>(await _repository.GetById(id));
        }

        public async Task UpdateItem(int id, CategoriesDto item)
        {
            var categoryEntity = mapper.Map<CategoriesDto, Categories>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, categoryEntity);
        }
    }
}
