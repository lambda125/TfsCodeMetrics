using System.Collections.Generic;

namespace Metrics.Data
{
    public class CodeType : MetricElement
    {
        public virtual Namespace Namespace { get; set; }
        public virtual ICollection<CodeMember> Members { get; set; }
    }
}