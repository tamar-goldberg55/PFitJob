using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.models; // תוודאי שזה השם של ה-namespace של המודלים שלך
using CodeFirst;         // תוודאי שזה השם של ה-namespace של ה-DbContext שלך

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataBase _context; // אם ל-Context שלך קוראים אחרת, תשני כאן

        public CategoriesController(DataBase context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories()
        {
            // זה ניגש לדאטה בייס ומחזיר את כל הקטגוריות
            return await _context.Categories.ToListAsync();
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Categories>> PostCategory(Categories category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }
    }
}