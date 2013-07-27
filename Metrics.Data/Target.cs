using System.Collections.Generic;

namespace Metrics.Data
{
    public class Target : MetricElement
    {
        public virtual Build Build { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
    }
}