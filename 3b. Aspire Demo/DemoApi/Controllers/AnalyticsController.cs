using DemoApi.Data;
using DemoLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    { 
        private readonly ILogger<AnalyticsController> _logger;
        private readonly ApiDbContext _context;

        public AnalyticsController(ILogger<AnalyticsController> logger, ApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/analytics/emails
        [HttpGet("emails")]
        public async Task<ActionResult<IEnumerable<Email>>> GetEmails()
        {
            try
            {
                var emails = await _context.Emails.Include(e => e.ServiceMessage).ToListAsync();
                if (emails == null || !emails.Any())
                {
                    _logger.LogInformation("No emails found.");
                    return NotFound("No emails found.");
                }

                return Ok(emails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching emails.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/analytics/pageviews
        [HttpGet("pageviews")]
        public async Task<ActionResult<IEnumerable<PageView>>> GetPageViews()
        {
            try
            {
                var pageViews = await _context.PageViews.OrderBy(x=>x.URL).ToListAsync();
                if (pageViews == null || !pageViews.Any())
                {
                    _logger.LogInformation("No page views found.");
                    return NotFound("No page views found.");
                }

                return Ok(pageViews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching page views.");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
