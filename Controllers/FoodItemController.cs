using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodItemController : Controller
    {
        private readonly FoodAppContext context;
        public FoodItemController(FoodAppContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(context.FoodItems.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            return Ok(food);
        }

        [HttpPost]
        public IActionResult Create(FoodItem food)
        {
            if (food.CategoryId.HasValue)
            {
                bool categoryExists = context.Categories
                    .Any(c => c.CategoryId == food.CategoryId);

                if (!categoryExists)
                {
                    return BadRequest(new
                    {
                        message = "Category does not exist"
                    });
                }
            }

            context.FoodItems.Add(food);

            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = food.FoodId },
                food
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, FoodItem dto)
        {
            if (id != dto.FoodId)
            {
                return BadRequest();
            }

            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            // Check category exists
            if (dto.CategoryId.HasValue)
            {
                bool categoryExists = context.Categories
                    .Any(c => c.CategoryId == dto.CategoryId);

                if (!categoryExists)
                {
                    return BadRequest(new
                    {
                        message = "Category does not exist"
                    });
                }
            }

            food.CategoryId = dto.CategoryId;
            food.Name = dto.Name;
            food.Price = dto.Price;
            food.ImageUrl = dto.ImageUrl;
            food.Description = dto.Description;
            food.TotalSold = dto.TotalSold;
            food.StockQuantity = dto.StockQuantity;
            food.AvgRating = dto.AvgRating;
            food.IsActive = dto.IsActive;

            context.SaveChanges();

            return Ok(food);
        }


        [HttpPatch("{id}")]
        public IActionResult Patch(int id, FoodItem dto)
        {
            ModelState.Remove("Name");

            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            // Category
            if (dto.CategoryId.HasValue)
            {
                bool categoryExists = context.Categories
                    .Any(c => c.CategoryId == dto.CategoryId);

                if (!categoryExists)
                {
                    return BadRequest(new
                    {
                        message = "Category does not exist"
                    });
                }

                food.CategoryId = dto.CategoryId;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                food.Name = dto.Name;
            }

            if (dto.Price > 0)
            {
                food.Price = dto.Price;
            }

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                food.ImageUrl = dto.ImageUrl;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                food.Description = dto.Description;
            }

            if (dto.TotalSold.HasValue)
            {
                food.TotalSold = dto.TotalSold;
            }

            if (dto.StockQuantity.HasValue)
            {
                food.StockQuantity = dto.StockQuantity;
            }

            if (dto.AvgRating.HasValue)
            {
                food.AvgRating = dto.AvgRating;
            }

            if (dto.IsActive.HasValue)
            {
                food.IsActive = dto.IsActive;
            }

            context.SaveChanges();

            return Ok(food);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            context.FoodItems.Remove(food);

            context.SaveChanges();

            return NoContent();
        }
    }
}
