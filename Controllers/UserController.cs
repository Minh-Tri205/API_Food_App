using API_Food_App.Dto;
using API_Food_App.Models;
using API_Food_App.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FoodAppContext context;
        private readonly UserService userService;
        private readonly IWebHostEnvironment env;

        public UserController(
            FoodAppContext context,
            UserService userService,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.userService = userService;
            this.env = env;
        }

        // =========================
        // Helper: project User -> response object (an passwordHash)
        // =========================
        private object ToResponse(User u) => new
        {
            u.UserId,
            u.FullName,
            u.Email,
            u.Phone,
            AvatarUrl = string.IsNullOrEmpty(u.AvatarUrl)
                ? null
                : $"{Request.Scheme}://{Request.Host}{u.AvatarUrl}",
            u.Role,
            u.IsActive,
            u.LoyaltyPoints,
            u.CreatedAt,
            u.UpdatedAt
        };

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    AvatarUrl = string.IsNullOrEmpty(u.AvatarUrl)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}{u.AvatarUrl}",
                    u.Role,
                    u.IsActive,
                    u.LoyaltyPoints,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .ToList();

            return Ok(users);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = context.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    AvatarUrl = string.IsNullOrEmpty(u.AvatarUrl)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}{u.AvatarUrl}",
                    u.Role,
                    u.IsActive,
                    u.LoyaltyPoints,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .FirstOrDefault();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(User user)
        {
            user.PasswordHash = userService.HashPassword(user.PasswordHash);

            user.IsActive ??= true;
            user.Role ??= "customer";
            user.LoyaltyPoints ??= 0;

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            context.Users.Add(user);
            context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.UserId },
                ToResponse(user)
            );
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email và mật khẩu không được để trống");

            var hashed = userService.HashPassword(dto.Password);

            var user = context.Users
                .Where(u => u.Email == dto.Email && u.PasswordHash == hashed)
                .FirstOrDefault();

            if (user == null)
                return Unauthorized("Email hoặc mật khẩu không đúng");

            if (user.IsActive == false)
                return Unauthorized("Tài khoản đã bị khoá");

            return Ok(ToResponse(user));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
    int id,
    [FromForm] UpdateUserDto dto,
    IFormFile? file)
        {
            var user = context.Users.Find(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
                user.PasswordHash = userService.HashPassword(dto.PasswordHash);

            // Upload avatar
            if (file != null && file.Length > 0)
            {
                string uploadPath = Path.Combine(
                    env.WebRootPath, "Uploads", "Users");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string extension = Path.GetExtension(file.FileName);

                string emailName =
                    (dto.Email ?? user.Email ?? $"user_{user.UserId}")
                    .Replace("@", "_")
                    .Replace(".", "_");

                string fileName = $"{emailName}_url{extension}";

                // Xoa avatar cu
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    string oldFile = Path.GetFileName(user.AvatarUrl);
                    string oldPath = Path.Combine(uploadPath, oldFile);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                user.AvatarUrl = $"/Uploads/Users/{fileName}";
            }

            if (!string.IsNullOrWhiteSpace(dto.DeviceToken))
                user.DeviceToken = dto.DeviceToken;

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.Role = dto.Role;

            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive;

            if (dto.LoyaltyPoints.HasValue)
                user.LoyaltyPoints = dto.LoyaltyPoints;

            if (!string.IsNullOrWhiteSpace(dto.PasswordResetToken))
                user.PasswordResetToken = dto.PasswordResetToken;

            if (dto.ResetTokenExpiry.HasValue)
                user.ResetTokenExpiry = dto.ResetTokenExpiry;

            user.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Updated successfully",
                user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                AvatarUrl = string.IsNullOrEmpty(user.AvatarUrl)
                    ? null
                    : $"{Request.Scheme}://{Request.Host}{user.AvatarUrl}",
                user.Role,
                user.IsActive,
                user.LoyaltyPoints
            });
        }


        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = context.Users.Find(id);
            if (user == null) return NotFound();

            // Xoa file avatar
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                string fileName = Path.GetFileName(user.AvatarUrl);
                string imagePath = Path.Combine(
                    env.WebRootPath, "Uploads", "Users", fileName);

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            context.Users.Remove(user);
            context.SaveChanges();

            return NoContent();
        }

        // =========================
        // TOGGLE STATUS
        // =========================
        [HttpPatch("toggle-status/{id}")]
        public IActionResult ToggleStatus(int id)
        {
            var result = userService.ToggleUserStatus(id);
            if (!result) return NotFound("User not found");
            return Ok("Toggled user status successfully");
        }

        // =========================
        // DEACTIVATE
        // =========================
        [HttpPatch("deactivate/{id}")]
        public IActionResult Deactivate(int id)
        {
            var result = userService.DeactivateUser(id);
            if (!result) return NotFound("User not found");
            return Ok("User deactivated");
        }

        // =========================
        // ACTIVATE
        // =========================
        [HttpPatch("activate/{id}")]
        public IActionResult Activate(int id)
        {
            var result = userService.ActivateUser(id);
            if (!result) return NotFound("User not found");
            return Ok("User activated");
        }

        // =========================
        // CHANGE ROLE
        // =========================
        [HttpPatch("change-role/{id}")]
        public IActionResult ChangeRole(int id, [FromBody] string role)
        {
            var result = userService.ChangeRole(id, role);
            if (!result) return BadRequest("Invalid role or user not found");
            return Ok("Role updated successfully");
        }
    }
}
