using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly FoodAppContext context;
        public CategoryController(FoodAppContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(context.Categories.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            context.Categories.Remove(category);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, Category dto)
        {
            // When you test this endpoint, make sure to send Name as not null
            // if you don't want to update it, otherwise it will be set to empty string in the database.
            ModelState.Remove("Name");

            var category = context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                category.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                category.ImageUrl = dto.ImageUrl;
            }

            if (dto.IsActive.HasValue)
            {
                category.IsActive = dto.IsActive.Value;
            }

            context.SaveChanges();

            return Ok(category);
        }
    }
}
