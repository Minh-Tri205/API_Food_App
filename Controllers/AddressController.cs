using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly FoodAppContext context;

        public AddressController(FoodAppContext context)
        {
            this.context = context;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var addresses = context.Addresses
                .Select(a => new
                {
                    a.AddressId,
                    a.UserId,
                    a.Label,
                    a.FullAddress,
                    a.RecipientName,
                    a.RecipientPhone,
                    a.IsDefault
                })
                .ToList();

            return Ok(addresses);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var address = context.Addresses
                .Where(a => a.AddressId == id)
                .Select(a => new
                {
                    a.AddressId,
                    a.UserId,
                    a.Label,
                    a.FullAddress,
                    a.RecipientName,
                    a.RecipientPhone,
                    a.IsDefault
                })
                .FirstOrDefault();

            if (address == null)
            {
                return NotFound(new
                {
                    message = "Address not found"
                });
            }

            return Ok(address);
        }

        // =========================
        // GET ADDRESS BY USER
        // =========================
        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(int userId)
        {
            var addresses = context.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.AddressId,
                    a.UserId,
                    a.Label,
                    a.FullAddress,
                    a.RecipientName,
                    a.RecipientPhone,
                    a.IsDefault
                })
                .ToList();

            return Ok(addresses);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(Address address)
        {
            bool userExists = await context.Users
                .AnyAsync(u => u.UserId == address.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User does not exist"
                });
            }

            // Nếu địa chỉ này là mặc định
            if (address.IsDefault == true)
            {
                var oldDefaults = context.Addresses
                    .Where(a => a.UserId == address.UserId
                             && a.IsDefault == true);

                foreach (var item in oldDefaults)
                {
                    item.IsDefault = false;
                }
            }

            context.Addresses.Add(address);

            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = address.AddressId },
                address
            );
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            Address dto)
        {
            if (id != dto.AddressId)
            {
                return BadRequest();
            }

            var address = await context.Addresses
                .FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            bool userExists = await context.Users
                .AnyAsync(u => u.UserId == dto.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User does not exist"
                });
            }

            if (dto.IsDefault == true)
            {
                var oldDefaults = context.Addresses
                    .Where(a => a.UserId == dto.UserId
                             && a.AddressId != id
                             && a.IsDefault == true);

                foreach (var item in oldDefaults)
                {
                    item.IsDefault = false;
                }
            }

            address.UserId = dto.UserId;
            address.Label = dto.Label;
            address.FullAddress = dto.FullAddress;
            address.RecipientName = dto.RecipientName;
            address.RecipientPhone = dto.RecipientPhone;
            address.IsDefault = dto.IsDefault;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Updated successfully",
                address
            });
        }

        // =========================
        // PATCH
        // =========================
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(
            int id,
            Address dto)
        {
            var address = await context.Addresses
                .FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            if (dto.UserId > 0)
            {
                bool userExists = await context.Users
                    .AnyAsync(u => u.UserId == dto.UserId);

                if (!userExists)
                {
                    return BadRequest(new
                    {
                        message = "User does not exist"
                    });
                }

                address.UserId = dto.UserId;
            }

            if (!string.IsNullOrWhiteSpace(dto.Label))
            {
                address.Label = dto.Label;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullAddress))
            {
                address.FullAddress = dto.FullAddress;
            }

            if (!string.IsNullOrWhiteSpace(dto.RecipientName))
            {
                address.RecipientName = dto.RecipientName;
            }

            if (!string.IsNullOrWhiteSpace(dto.RecipientPhone))
            {
                address.RecipientPhone = dto.RecipientPhone;
            }

            if (dto.IsDefault.HasValue)
            {
                if (dto.IsDefault == true)
                {
                    var oldDefaults = context.Addresses
                        .Where(a => a.UserId == address.UserId
                                 && a.AddressId != id
                                 && a.IsDefault == true);

                    foreach (var item in oldDefaults)
                    {
                        item.IsDefault = false;
                    }
                }

                address.IsDefault = dto.IsDefault;
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Updated successfully",
                address
            });
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var address = await context.Addresses
                .FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            context.Addresses.Remove(address);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}