using Investment.API.Data;
using Investment.API.DTOs;
using Investment.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Investment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // User must be logged in
    public class ContributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContributionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/contributions
        [HttpPost]
        public async Task<IActionResult> AddContribution(ContributionDto dto)
        {
            // Minimum contribution check
            if (dto.Amount < 2000)
                return BadRequest("Minimum contribution is KES 2000");

            // Get logged-in user ID from JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            // Check if payment is late
            var today = DateTime.UtcNow;
            bool isLate = today.Day > 15;
            decimal fine = isLate ? 500 : 0;

            var contribution = new Contribution
            {
                UserId = userGuid,
                Amount = dto.Amount,
                Date = today,
                IsLate = isLate,
                FineAmount = fine
            };

            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contribution recorded",
                contribution.Amount,
                contribution.IsLate,
                contribution.FineAmount
            });
        }

        // GET: api/contributions/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyContributions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            var contributions = await _context.Contributions
                .Where(c => c.UserId == userGuid)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return Ok(contributions);
        }

        // GET: api/contributions/total
        [HttpGet("total")]
        public async Task<IActionResult> GetMyTotal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            var total = await _context.Contributions
                .Where(c => c.UserId == userGuid)
                .SumAsync(c => c.Amount);

            return Ok(new { total });
        }
    }
}
