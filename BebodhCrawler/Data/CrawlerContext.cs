using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BebodhCrawler.Data
{
    public class CrawlerContext : IdentityDbContext<ApplicationUser>
    {
        public CrawlerContext(DbContextOptions<CrawlerContext> options) : base(options) { }
    }
}
