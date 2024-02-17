using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Data
{
    public class CrawlerDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IOptions<DbServerSettings> _dbConfig;

        public CrawlerDbContext(DbContextOptions<CrawlerDbContext> options, IOptions<DbServerSettings> dbConfig) : base(options)
        {
            _dbConfig = dbConfig;
        }
    }
}
