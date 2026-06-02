using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherUsageController : ControllerBase
    {
        private readonly FoodAppContext context;

        public VoucherUsageController(FoodAppContext context)
        {
            this.context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var usages = context.VoucherUsages
                .Include(x => x.User)
                .Include(x => x.Voucher)
                .ToList();

            return Ok(usages);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usage = context.VoucherUsages
                .Include(x => x.User)
                .Include(x => x.Voucher)
                .FirstOrDefault(x => x.UsageId == id);

            if (usage == null)
                return NotFound();

            return Ok(usage);
        }

        // GET BY USER
        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(int userId)
        {
            var usages = context.VoucherUsages
                .Where(x => x.UserId == userId)
                .Include(x => x.Voucher)
                .ToList();

            return Ok(usages);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            VoucherUsage usage)
        {
            bool userExists = await context.Users
                .AnyAsync(u => u.UserId == usage.UserId);

            if (!userExists)
            {
                return BadRequest(new
                {
                    message = "User not found"
                });
            }

            bool voucherExists = await context.Vouchers
                .AnyAsync(v => v.VoucherId == usage.VoucherId);

            if (!voucherExists)
            {
                return BadRequest(new
                {
                    message = "Voucher not found"
                });
            }

            usage.UsedAt = DateTime.Now;

            context.VoucherUsages.Add(usage);

            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = usage.UsageId },
                usage
            );
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            VoucherUsage dto)
        {
            if (id != dto.UsageId)
                return BadRequest();

            var usage = await context.VoucherUsages
                .FindAsync(id);

            if (usage == null)
                return NotFound();

            bool userExists = await context.Users
                .AnyAsync(x => x.UserId == dto.UserId);

            bool voucherExists = await context.Vouchers
                .AnyAsync(x => x.VoucherId == dto.VoucherId);

            if (!userExists || !voucherExists)
            {
                return BadRequest(new
                {
                    message = "User or Voucher not found"
                });
            }

            usage.UserId = dto.UserId;
            usage.VoucherId = dto.VoucherId;
            usage.OrderId = dto.OrderId;
            usage.UsedAt = dto.UsedAt;

            await context.SaveChangesAsync();

            return Ok(usage);
        }

        // PATCH
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(
            int id,
            VoucherUsage dto)
        {
            var usage = await context.VoucherUsages
                .FindAsync(id);

            if (usage == null)
                return NotFound();

            if (dto.UserId > 0)
            {
                bool userExists = await context.Users
                    .AnyAsync(x => x.UserId == dto.UserId);

                if (!userExists)
                    return BadRequest("User not found");

                usage.UserId = dto.UserId;
            }

            if (dto.VoucherId > 0)
            {
                bool voucherExists = await context.Vouchers
                    .AnyAsync(x => x.VoucherId == dto.VoucherId);

                if (!voucherExists)
                    return BadRequest("Voucher not found");

                usage.VoucherId = dto.VoucherId;
            }

            if (dto.OrderId.HasValue)
                usage.OrderId = dto.OrderId;

            if (dto.UsedAt.HasValue)
                usage.UsedAt = dto.UsedAt;

            await context.SaveChangesAsync();

            return Ok(usage);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usage = await context.VoucherUsages
                .FindAsync(id);

            if (usage == null)
                return NotFound();

            context.VoucherUsages.Remove(usage);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}