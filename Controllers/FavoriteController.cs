using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : Controller
    {
        private readonly FoodAppContext context;

        public FavoriteController(FoodAppContext context)
        {
            this.context = context;
        }

        // GET: api/favorite
        [HttpGet]
        public IActionResult GetAll()
        {
            var favorites = context.Favorites
                .Include(f => f.User)
                .Include(f => f.Food)
                .ToList();

            return Ok(favorites);
        }

        // GET: api/favorite/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var favorite = context.Favorites
                .Include(f => f.User)
                .Include(f => f.Food)
                .FirstOrDefault(f => f.FavoriteId == id);

            if (favorite == null)
            {
                return NotFound();
            }

            return Ok(favorite);
        }

        // POST: api/favorite
        [HttpPost]
        public IActionResult Create(Favorite favorite)
        {
            // Check User exists
            bool userExists = context.Users
                .Any(u => u.UserId == favorite.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User does not exist"
                });
            }

            // Check Food exists
            bool foodExists = context.FoodItems
                .Any(f => f.FoodId == favorite.FoodId);

            if (!foodExists)
            {
                return BadRequest(new
                {
                    message = "Food does not exist"
                });
            }

            // Check duplicate favorite
            bool alreadyExists = context.Favorites.Any(f =>
                f.UserId == favorite.UserId &&
                f.FoodId == favorite.FoodId);

            if (alreadyExists)
            {
                return BadRequest(new
                {
                    message = "Food already added to favorites"
                });
            }

            favorite.CreatedAt = DateTime.Now;

            context.Favorites.Add(favorite);

            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = favorite.FavoriteId },
                favorite
            );
        }

        // PUT: api/favorite/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Favorite dto)
        {
            if (id != dto.FavoriteId)
            {
                return BadRequest();
            }

            var favorite = context.Favorites.Find(id);

            if (favorite == null)
            {
                return NotFound();
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
            bool foodExists = context.FoodItems
                .Any(f => f.FoodId == dto.FoodId);

            if (!foodExists)
            {
                return BadRequest(new
                {
                    message = "Food does not exist"
                });
            }

            // Check duplicate favorite
            bool alreadyExists = context.Favorites.Any(f =>
                f.UserId == dto.UserId &&
                f.FoodId == dto.FoodId &&
                f.FavoriteId != id);

            if (alreadyExists)
            {
                return BadRequest(new
                {
                    message = "Food already added to favorites"
                });
            }

            favorite.UserId = dto.UserId;
            favorite.FoodId = dto.FoodId;

            context.SaveChanges();

            return Ok(favorite);
        }

        // PATCH: api/favorite/5
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, Favorite dto)
        {
            var favorite = context.Favorites.Find(id);

            if (favorite == null)
            {
                return NotFound();
            }

            // Update User
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

                favorite.UserId = dto.UserId;
            }

            // Update Food
            if (dto.FoodId > 0)
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

                favorite.FoodId = dto.FoodId;
            }

            // Check duplicate after update
            bool alreadyExists = context.Favorites.Any(f =>
                f.UserId == favorite.UserId &&
                f.FoodId == favorite.FoodId &&
                f.FavoriteId != id);

            if (alreadyExists)
            {
                return BadRequest(new
                {
                    message = "Food already added to favorites"
                });
            }

            context.SaveChanges();

            return Ok(favorite);
        }

        // DELETE: api/favorite/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var favorite = context.Favorites.Find(id);

            if (favorite == null)
            {
                return NotFound();
            }

            context.Favorites.Remove(favorite);

            context.SaveChanges();

            return NoContent();
        }
    }
}