using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademySampleProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BloggingContext _dbcontext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, BloggingContext bloggingContext)
        {
            _logger = logger;
            _dbcontext = bloggingContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetBlogs")]
        public Task<List<Blog>> GetBlogs()
        {
            return _dbcontext.Blogs.ToListAsync();
        }

        [HttpGet("GetBlog/{id}")]
        public Task<Blog?> GetBlog(int id)
        {
            return _dbcontext.Blogs.FirstOrDefaultAsync(x => x.BlogId == id);
        }

        [HttpPost("AddBlog")]
        public async Task<Blog> GetBlog(Blog blog)
        {
            _dbcontext.Blogs.Add(blog);
            await _dbcontext.SaveChangesAsync();
            return blog;
        }
    }
}