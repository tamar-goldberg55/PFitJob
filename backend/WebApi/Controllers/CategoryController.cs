using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
       
        private readonly ICategories _categoryService;

        // הזרקה של ה-Service (ולא של הדאטה-בייס ישירות!)
        public CategoryController(ICategories categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        [HttpGet]
        public Task<List<CategoriesDto>> GetAll()
        {
            // קריאה לסרביס שמחזיר רשימת DTOs
            return _categoryService.GetAll();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public Task<CategoriesDto> Get(int id)
        {
            return _categoryService.GetById(id);
        }

        // POST: api/Categories
        [HttpPost]
        public Task<CategoriesDto> Post([FromBody] CategoriesDto categoryDto)
        {
            // שליחת ה-DTO לסרביס ליצירה
            return _categoryService.AddItem(categoryDto);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public Task Put(int id, [FromBody] CategoriesDto categoryDto)
        {
            return _categoryService.UpdateItem(id, categoryDto);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _categoryService.DeleteItem(id);
        }
    }

}
