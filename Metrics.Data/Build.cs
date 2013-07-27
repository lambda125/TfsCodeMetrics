using System.Collections.Generic;

namespace Metrics.Data
{
    public class Build : MetricElement
    {
        public virtual ICollection<Target> Targets { get; set; }
    }
}
