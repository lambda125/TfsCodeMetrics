using System.Collections.Generic;

namespace Metrics.Data
{
    public class Namespace : MetricElement
    {
        public virtual Module Module { get; set; }
        public virtual ICollection<CodeType> Types { get; set; }
    }
}