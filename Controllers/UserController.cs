using API_Food_App.Models;
using API_Food_App.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly FoodAppContext context;

        private readonly UserService userService;

        public UserController(
            FoodAppContext context,
            UserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(context.Users.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            user.PasswordHash = userService.HashPassword(user.PasswordHash);

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            context.Users.Add(user);
            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.UserId },
                user
            );
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            context.Users.Remove(user);
            context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, User dto)
        {
            ModelState.Remove("FullName");
            ModelState.Remove("PasswordHash");

            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            // FullName
            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.FullName = dto.FullName;
            }

            // Email
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user.Email = dto.Email;
            }

            // Phone
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                user.Phone = dto.Phone;
            }

            // PasswordHash
            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                user.PasswordHash =
                    userService.HashPassword(dto.PasswordHash);
            }

            // AvatarUrl
            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
            {
                user.AvatarUrl = dto.AvatarUrl;
            }

            // DeviceToken
            if (!string.IsNullOrWhiteSpace(dto.DeviceToken))
            {
                user.DeviceToken = dto.DeviceToken;
            }

            // Role
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = dto.Role;
            }

            // IsActive
            if (dto.IsActive.HasValue)
            {
                user.IsActive = dto.IsActive.Value;
            }

            // LoyaltyPoints
            if (dto.LoyaltyPoints.HasValue)
            {
                user.LoyaltyPoints = dto.LoyaltyPoints.Value;
            }

            // PasswordResetToken
            if (!string.IsNullOrWhiteSpace(dto.PasswordResetToken))
            {
                user.PasswordResetToken = dto.PasswordResetToken;
            }

            // ResetTokenExpiry
            if (dto.ResetTokenExpiry.HasValue)
            {
                user.ResetTokenExpiry = dto.ResetTokenExpiry.Value;
            }

            user.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return Ok(user);
        }

        [HttpPatch("toggle-status/{id}")]
        public IActionResult ToggleStatus(int id)
        {
            var result = userService.ToggleUserStatus(id);

            if (!result)
                return NotFound("User not found");

            return Ok("Toggled user status successfully");
        }

        [HttpPatch("deactivate/{id}")]
        public IActionResult Deactivate(int id)
        {
            var result = userService.DeactivateUser(id);

            if (!result)
                return NotFound("User not found");

            return Ok("User deactivated");
        }

        [HttpPatch("activate/{id}")]
        public IActionResult Activate(int id)
        {
            var result = userService.ActivateUser(id);

            if (!result)
                return NotFound("User not found");

            return Ok("User activated");
        }

        [HttpPatch("change-role/{id}")]
        public IActionResult ChangeRole(int id, [FromBody] string role)
        {
            var result = userService.ChangeRole(id, role);

            if (!result)
                return BadRequest("Invalid role or user not found");

            return Ok("Role updated successfully");
        }
    }
}