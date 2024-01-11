using Core.Entities;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CrawlRepository : BaseRepository<Crawl>, ICrawlRepository
    {
        public CrawlRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
        }
    }
}
