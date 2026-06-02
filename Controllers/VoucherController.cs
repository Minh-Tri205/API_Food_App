using API_Food_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Food_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly FoodAppContext context;

        public VoucherController(FoodAppContext context)
        {
            this.context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(context.Vouchers.ToList());
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var voucher = context.Vouchers.Find(id);

            if (voucher == null)
                return NotFound();

            return Ok(voucher);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            bool codeExists = await context.Vouchers
                .AnyAsync(v => v.Code == voucher.Code);

            if (codeExists)
            {
                return BadRequest(new
                {
                    message = "Voucher code already exists"
                });
            }

            context.Vouchers.Add(voucher);

            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = voucher.VoucherId },
                voucher
            );
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            Voucher dto)
        {
            if (id != dto.VoucherId)
                return BadRequest();

            var voucher = await context.Vouchers.FindAsync(id);

            if (voucher == null)
                return NotFound();

            bool codeExists = await context.Vouchers
                .AnyAsync(v =>
                    v.Code == dto.Code &&
                    v.VoucherId != id);

            if (codeExists)
            {
                return BadRequest(new
                {
                    message = "Voucher code already exists"
                });
            }

            voucher.Code = dto.Code;
            voucher.Description = dto.Description;
            voucher.DiscountAmount = dto.DiscountAmount;
            voucher.MinOrderValue = dto.MinOrderValue;
            voucher.StartDate = dto.StartDate;
            voucher.EndDate = dto.EndDate;
            voucher.UsageLimit = dto.UsageLimit;
            voucher.IsActive = dto.IsActive;

            await context.SaveChangesAsync();

            return Ok(voucher);
        }

        // PATCH
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(
            int id,
            Voucher dto)
        {
            var voucher = await context.Vouchers.FindAsync(id);

            if (voucher == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                bool codeExists = await context.Vouchers
                    .AnyAsync(v =>
                        v.Code == dto.Code &&
                        v.VoucherId != id);

                if (codeExists)
                {
                    return BadRequest(new
                    {
                        message = "Voucher code already exists"
                    });
                }

                voucher.Code = dto.Code;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
                voucher.Description = dto.Description;

            if (dto.DiscountAmount > 0)
                voucher.DiscountAmount = dto.DiscountAmount;

            if (dto.MinOrderValue.HasValue)
                voucher.MinOrderValue = dto.MinOrderValue;

            if (dto.StartDate.HasValue)
                voucher.StartDate = dto.StartDate;

            if (dto.EndDate.HasValue)
                voucher.EndDate = dto.EndDate;

            if (dto.UsageLimit.HasValue)
                voucher.UsageLimit = dto.UsageLimit;

            if (dto.IsActive.HasValue)
                voucher.IsActive = dto.IsActive;

            await context.SaveChangesAsync();

            return Ok(voucher);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await context.Vouchers.FindAsync(id);

            if (voucher == null)
                return NotFound();

            context.Vouchers.Remove(voucher);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}