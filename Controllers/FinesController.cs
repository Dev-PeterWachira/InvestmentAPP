using Investment.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Investment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/fines/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyFines()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            var fines = await _context.Fines
                .Where(f => f.UserId == userGuid)
                .OrderByDescending(f => f.DateIssued)
                .ToListAsync();

            return Ok(fines);
        }

        // GET: api/fines/total
        [HttpGet("total")]
        public async Task<IActionResult> GetMyTotalFines()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            var total = await _context.Fines
                .Where(f => f.UserId == userGuid)
                .SumAsync(f => f.Amount);

            return Ok(new { totalFines = total });
        }
    }
}
