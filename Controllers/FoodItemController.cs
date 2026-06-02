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
            IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        // Helper: build full ImageUrl tra ve client
        private string? BuildImageUrl(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            // Da la URL day du -> tra nguyen
            if (raw.StartsWith("http://") || raw.StartsWith("https://"))
                return raw;
            // Chi la filename -> prepend
            return $"{Request.Scheme}://{Request.Host}/Uploads/FoodItems/{raw}";
        }

        private object ToResponse(FoodItem f) => new
        {
            f.FoodId,
            f.CategoryId,
            f.Name,
            f.Price,
            ImageUrl = BuildImageUrl(f.ImageUrl),
            f.Description,
            f.TotalSold,
            f.StockQuantity,
            f.AvgRating,
            f.IsActive
        };

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var foods = context.FoodItems.ToList()
                .Select(f => ToResponse(f));
            return Ok(foods);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var food = context.FoodItems.Find(id);
            if (food == null) return NotFound();
            return Ok(ToResponse(food));
        }

        // =========================
        // CREATE — file HOAC imageUrl
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] FoodItem food,
            IFormFile? file)
        {
            if (food.CategoryId.HasValue)
            {
                bool categoryExists = context.Categories
                    .Any(c => c.CategoryId == food.CategoryId);
                if (!categoryExists)
                    return BadRequest(new { message = "Category does not exist" });
            }

            // Uu tien file upload
            if (file != null && file.Length > 0)
            {
                food.ImageUrl = await SaveImage(file);
            }
            // Khong co file -> giu nguyen food.ImageUrl (URL ngoai hoac null)

            context.FoodItems.Add(food);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Created successfully",
                food = ToResponse(food)
            });
        }

        // =========================
        // UPDATE — file HOAC imageUrl
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] FoodItem dto,
            IFormFile? file)
        {
            if (id != dto.FoodId) return BadRequest();

            var food = context.FoodItems.Find(id);
            if (food == null) return NotFound();

            if (dto.CategoryId.HasValue)
            {
                bool categoryExists = context.Categories
                    .Any(c => c.CategoryId == dto.CategoryId);
                if (!categoryExists)
                    return BadRequest(new { message = "Category does not exist" });
            }

            // Image: 3 truong hop
            if (file != null && file.Length > 0)
            {
                // 1. Co file upload moi -> xoa anh cu (neu la file local), luu file moi
                DeleteOldImageIfLocal(food.ImageUrl);
                food.ImageUrl = await SaveImage(file);
            }
            else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                // 2. Co imageUrl moi (URL hoac filename) -> xoa anh cu neu local
                if (food.ImageUrl != dto.ImageUrl)
                {
                    DeleteOldImageIfLocal(food.ImageUrl);
                }
                food.ImageUrl = dto.ImageUrl;
            }
            // 3. Khong gui gi -> giu nguyen anh cu

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
                food = ToResponse(food)
            });
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var food = context.FoodItems.Find(id);
            if (food == null) return NotFound();

            DeleteOldImageIfLocal(food.ImageUrl);

            context.FoodItems.Remove(food);
            context.SaveChanges();

            return NoContent();
        }

        // =========================
        // PRIVATE HELPERS
        // =========================
        private async Task<string> SaveImage(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString()
                              + Path.GetExtension(file.FileName);

            string uploadPath = Path.Combine(
                env.WebRootPath, "Uploads", "FoodItems");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        private void DeleteOldImageIfLocal(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            // Neu la URL ngoai -> khong xoa
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return;

            string uploadPath = Path.Combine(
                env.WebRootPath, "Uploads", "FoodItems");
            string oldPath = Path.Combine(uploadPath, imageUrl);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }
    }
}
