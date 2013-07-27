using System.Collections.Generic;

namespace Metrics.Data
{
    public class Module : MetricElement
    {
        public virtual Target Target { get; set; }
        public virtual ICollection<Namespace> Namespaces { get; set; } 
    }
}