using System;
using System.Collections.Generic;

namespace Metrics.Data
{
    public abstract class MetricElement
    {
        protected MetricElement()
        {
            Created = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Metric> Metrics { get; set; }
        public DateTime Created { get; set; }
    }
}
