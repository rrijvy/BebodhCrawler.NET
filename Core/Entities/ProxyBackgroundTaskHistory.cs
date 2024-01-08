using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class ProxyBackgroundTaskHistory : BaseModel
    {
        public long StartedAt { get; set; }
        public long EndedAt { get; set; }
        public bool IsRunning { get; set; }
    }
}
