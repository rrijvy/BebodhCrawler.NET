using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class CrawlerDbContext : IdentityDbContext<ApplicationUser>
    {
        public CrawlerDbContext(DbContextOptions<CrawlerDbContext> options) : base(options) { }
    }
}
