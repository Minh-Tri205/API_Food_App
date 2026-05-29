using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly FoodAppContext context;

        public CartController(FoodAppContext context)
        {
            this.context = context;
        }

        // GET: api/cart
        [HttpGet]
        public IActionResult GetAll()
        {
            var carts = context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .ToList();

            return Ok(carts);
        }

        // GET: api/cart/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cart = context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        // POST: api/cart
        [HttpPost]
        public IActionResult Create(Cart cart)
        {
            // Check User exists
            if (cart.UserId.HasValue)
            {
                bool userExists = context.Users
                    .Any(u => u.UserId == cart.UserId);

                if (!userExists)
                {
                    return BadRequest(new
                    {
                        message = "User does not exist"
                    });
                }

                // Check user already has cart
                bool cartExists = context.Carts
                    .Any(c => c.UserId == cart.UserId);

                if (cartExists)
                {
                    return BadRequest(new
                    {
                        message = "User already has a cart"
                    });
                }
            }

            context.Carts.Add(cart);

            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = cart.CartId },
                cart
            );
        }

        // PUT: api/cart/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Cart dto)
        {
            if (id != dto.CartId)
            {
                return BadRequest();
            }

            var cart = context.Carts.Find(id);

            if (cart == null)
            {
                return NotFound();
            }

            // Check User exists
            if (dto.UserId.HasValue)
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

                // Check duplicate cart
                bool cartExists = context.Carts.Any(c =>
                    c.UserId == dto.UserId &&
                    c.CartId != id);

                if (cartExists)
                {
                    return BadRequest(new
                    {
                        message = "User already has a cart"
                    });
                }
            }

            cart.UserId = dto.UserId;

            context.SaveChanges();

            return Ok(cart);
        }

        // PATCH: api/cart/5
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, Cart dto)
        {
            var cart = context.Carts.Find(id);

            if (cart == null)
            {
                return NotFound();
            }

            if (dto.UserId.HasValue)
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

                // Check duplicate cart
                bool cartExists = context.Carts.Any(c =>
                    c.UserId == dto.UserId &&
                    c.CartId != id);

                if (cartExists)
                {
                    return BadRequest(new
                    {
                        message = "User already has a cart"
                    });
                }

                cart.UserId = dto.UserId;
            }

            context.SaveChanges();

            return Ok(cart);
        }

        // DELETE: api/cart/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cart = context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound();
            }

            // Delete cart items first
            if (cart.CartItems.Any())
            {
                context.CartItems.RemoveRange(cart.CartItems);
            }

            context.Carts.Remove(cart);

            context.SaveChanges();

            return NoContent();
        }
    }
}