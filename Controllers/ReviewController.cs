using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly FoodAppContext context;

        public ReviewController(FoodAppContext context)
        {
            this.context = context;
        }

        // GET: api/review
        [HttpGet]
        public IActionResult GetAll()
        {
            var reviews = context.Reviews
                .Include(r => r.User)
                .Include(r => r.Food)
                .Include(r => r.Order)
                .ToList();

            return Ok(reviews);
        }

        // GET: api/review/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var review = context.Reviews
                .Include(r => r.User)
                .Include(r => r.Food)
                .Include(r => r.Order)
                .FirstOrDefault(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // POST: api/review
        [HttpPost]
        public IActionResult Create(Review review)
        {
            // Check Order exists
            bool orderExists = context.Orders
                .Any(o => o.OrderId == review.OrderId);

            if (!orderExists)
            {
                return BadRequest(new
                {
                    message = "Order does not exist"
                });
            }

            // Check User exists
            bool userExists = context.Users
                .Any(u => u.UserId == review.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User does not exist"
                });
            }

            // Check Food exists
            if (review.FoodId.HasValue)
            {
                bool foodExists = context.FoodItems
                    .Any(f => f.FoodId == review.FoodId);

                if (!foodExists)
                {
                    return BadRequest(new
                    {
                        message = "Food does not exist"
                    });
                }
            }

            // Rating validation
            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest(new
                {
                    message = "Rating must be between 1 and 5"
                });
            }

            review.CreatedAt = DateTime.Now;

            context.Reviews.Add(review);

            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = review.ReviewId },
                review
            );
        }

        // PUT: api/review/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Review dto)
        {
            if (id != dto.ReviewId)
            {
                return BadRequest();
            }

            var review = context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            // Check Order exists
            bool orderExists = context.Orders
                .Any(o => o.OrderId == dto.OrderId);

            if (!orderExists)
            {
                return BadRequest(new
                {
                    message = "Order does not exist"
                });
            }

            // Check User exists
            bool userExists = context.Users
                .Any(u => u.UserId == dto.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User does not exist"
                });
            }

            // Check Food exists
            if (dto.FoodId.HasValue)
            {
                bool foodExists = context.FoodItems
                    .Any(f => f.FoodId == dto.FoodId);

                if (!foodExists)
                {
                    return BadRequest(new
                    {
                        message = "Food does not exist"
                    });
                }
            }

            // Rating validation
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest(new
                {
                    message = "Rating must be between 1 and 5"
                });
            }

            review.OrderId = dto.OrderId;
            review.UserId = dto.UserId;
            review.FoodId = dto.FoodId;
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            context.SaveChanges();

            return Ok(review);
        }

        // PATCH: api/review/5
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, Review dto)
        {
            var review = context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            // Order
            if (dto.OrderId > 0)
            {
                bool orderExists = context.Orders
                    .Any(o => o.OrderId == dto.OrderId);

                if (!orderExists)
                {
                    return BadRequest(new
                    {
                        message = "Order does not exist"
                    });
                }

                review.OrderId = dto.OrderId;
            }

            // User
            if (dto.UserId > 0)
            {
                bool userExists = context.Users
                    .Any(u => u.UserId == dto.UserId);

                if (!userExists)
                {
                    return BadRequest(new
                    {
                        message = "User does not exist"
                    });
                }

                review.UserId = dto.UserId;
            }

            // Food
            if (dto.FoodId.HasValue)
            {
                bool foodExists = context.FoodItems
                    .Any(f => f.FoodId == dto.FoodId);

                if (!foodExists)
                {
                    return BadRequest(new
                    {
                        message = "Food does not exist"
                    });
                }

                review.FoodId = dto.FoodId;
            }

            // Rating
            if (dto.Rating > 0)
            {
                if (dto.Rating < 1 || dto.Rating > 5)
                {
                    return BadRequest(new
                    {
                        message = "Rating must be between 1 and 5"
                    });
                }

                review.Rating = dto.Rating;
            }

            // Comment
            if (!string.IsNullOrWhiteSpace(dto.Comment))
            {
                review.Comment = dto.Comment;
            }

            context.SaveChanges();

            return Ok(review);
        }

        // DELETE: api/review/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            context.Reviews.Remove(review);

            context.SaveChanges();

            return NoContent();
        }
    }
}