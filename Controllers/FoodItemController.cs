//using API_Food_App.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace API_Food_App.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FoodItemController : Controller
//    {
//        private readonly FoodAppContext context;
//        public FoodItemController(FoodAppContext context)
//        {
//            this.context = context;
//        }

//        [HttpGet]
//        public IActionResult GetAll()
//        {
//            return Ok(context.FoodItems.ToList());
//        }

//        [HttpGet("{id}")]
//        public IActionResult GetById(int id)
//        {
//            var food = context.FoodItems.Find(id);

//            if (food == null)
//            {
//                return NotFound();
//            }

//            return Ok(food);
//        }

//        [HttpPost]
//        public IActionResult Create(FoodItem food)
//        {
//            if (food.CategoryId.HasValue)
//            {
//                bool categoryExists = context.Categories
//                    .Any(c => c.CategoryId == food.CategoryId);

//                if (!categoryExists)
//                {
//                    return BadRequest(new
//                    {
//                        message = "Category does not exist"
//                    });
//                }
//            }

//            context.FoodItems.Add(food);

//            context.SaveChanges();

//            return CreatedAtAction(
//                nameof(GetById),
//                new { id = food.FoodId },
//                food
//            );
//        }

//        [HttpPut("{id}")]
//        public IActionResult Update(int id, FoodItem dto)
//        {
//            if (id != dto.FoodId)
//            {
//                return BadRequest();
//            }

//            var food = context.FoodItems.Find(id);

//            if (food == null)
//            {
//                return NotFound();
//            }

//            // Check category exists
//            if (dto.CategoryId.HasValue)
//            {
//                bool categoryExists = context.Categories
//                    .Any(c => c.CategoryId == dto.CategoryId);

//                if (!categoryExists)
//                {
//                    return BadRequest(new
//                    {
//                        message = "Category does not exist"
//                    });
//                }
//            }

//            food.CategoryId = dto.CategoryId;
//            food.Name = dto.Name;
//            food.Price = dto.Price;
//            food.ImageUrl = dto.ImageUrl;
//            food.Description = dto.Description;
//            food.TotalSold = dto.TotalSold;
//            food.StockQuantity = dto.StockQuantity;
//            food.AvgRating = dto.AvgRating;
//            food.IsActive = dto.IsActive;

//            context.SaveChanges();

//            return Ok(food);
//        }


//        [HttpPatch("{id}")]
//        public IActionResult Patch(int id, FoodItem dto)
//        {
//            ModelState.Remove("Name");

//            var food = context.FoodItems.Find(id);

//            if (food == null)
//            {
//                return NotFound();
//            }

//            // Category
//            if (dto.CategoryId.HasValue)
//            {
//                bool categoryExists = context.Categories
//                    .Any(c => c.CategoryId == dto.CategoryId);

//                if (!categoryExists)
//                {
//                    return BadRequest(new
//                    {
//                        message = "Category does not exist"
//                    });
//                }

//                food.CategoryId = dto.CategoryId;
//            }

//            if (!string.IsNullOrWhiteSpace(dto.Name))
//            {
//                food.Name = dto.Name;
//            }

//            if (dto.Price > 0)
//            {
//                food.Price = dto.Price;
//            }

//            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
//            {
//                food.ImageUrl = dto.ImageUrl;
//            }

//            if (!string.IsNullOrWhiteSpace(dto.Description))
//            {
//                food.Description = dto.Description;
//            }

//            if (dto.TotalSold.HasValue)
//            {
//                food.TotalSold = dto.TotalSold;
//            }

//            if (dto.StockQuantity.HasValue)
//            {
//                food.StockQuantity = dto.StockQuantity;
//            }

//            if (dto.AvgRating.HasValue)
//            {
//                food.AvgRating = dto.AvgRating;
//            }

//            if (dto.IsActive.HasValue)
//            {
//                food.IsActive = dto.IsActive;
//            }

//            context.SaveChanges();

//            return Ok(food);
//        }

//        [HttpDelete("{id}")]
//        public IActionResult Delete(int id)
//        {
//            var food = context.FoodItems.Find(id);

//            if (food == null)
//            {
//                return NotFound();
//            }

//            context.FoodItems.Remove(food);

//            context.SaveChanges();

//            return NoContent();
//        }
//    }
//}

using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodItemController : ControllerBase
    {
        private readonly FoodAppContext context;
        private readonly IWebHostEnvironment env;

        public FoodItemController(
            FoodAppContext context,
            IWebHostEnvironment env
        )
        {
            this.context = context;
            this.env = env;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var foods = context.FoodItems.ToList();

            var result = foods.Select(food => new
            {
                food.FoodId,
                food.CategoryId,
                food.Name,
                food.Price,

                ImageUrl = string.IsNullOrEmpty(food.ImageUrl)
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/Uploads/FoodItems/{food.ImageUrl}",

                food.Description,
                food.TotalSold,
                food.StockQuantity,
                food.AvgRating,
                food.IsActive
            });

            return Ok(result);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                food.FoodId,
                food.CategoryId,
                food.Name,
                food.Price,

                ImageUrl = string.IsNullOrEmpty(food.ImageUrl)
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/Uploads/FoodItems/{food.ImageUrl}",

                food.Description,
                food.TotalSold,
                food.StockQuantity,
                food.AvgRating,
                food.IsActive
            });
        }

        // =========================
        // CREATE FOOD
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] FoodItem food,
            IFormFile? file
        )
        {
            // Check category
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

            // Upload image
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(file.FileName);

                string uploadPath = Path.Combine(
                    env.WebRootPath,
                    "Uploads",
                    "FoodItems"
                );

                // Create folder if not exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                food.ImageUrl = fileName;
            }

            context.FoodItems.Add(food);

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Created successfully",
                food
            });
        }

        // =========================
        // UPDATE FOOD
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] FoodItem dto,
            IFormFile? file
        )
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

            // Check category
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

            // Upload new image
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(file.FileName);

                string uploadPath = Path.Combine(
                    env.WebRootPath,
                    "Uploads",
                    "FoodItems"
                );

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // delete old image
                if (!string.IsNullOrEmpty(food.ImageUrl))
                {
                    string oldPath = Path.Combine(
                        uploadPath,
                        food.ImageUrl
                    );

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                food.ImageUrl = fileName;
            }

            // update data
            food.CategoryId = dto.CategoryId;
            food.Name = dto.Name;
            food.Price = dto.Price;
            food.Description = dto.Description;
            food.TotalSold = dto.TotalSold;
            food.StockQuantity = dto.StockQuantity;
            food.AvgRating = dto.AvgRating;
            food.IsActive = dto.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Updated successfully",
                food
            });
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var food = context.FoodItems.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            // delete image
            if (!string.IsNullOrEmpty(food.ImageUrl))
            {
                string imagePath = Path.Combine(
                    env.WebRootPath,
                    "Uploads",
                    "FoodItems",
                    food.ImageUrl
                );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            context.FoodItems.Remove(food);

            context.SaveChanges();

            return NoContent();
        }
    }
}