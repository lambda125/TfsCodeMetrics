using System.Data.Entity;
using System.Threading.Tasks;

namespace Metrics.Data
{
    public interface IMetricsStore
    {
        IDbSet<Build> Builds { get; set; }
        IDbSet<Target> Targets { get; set; }
        IDbSet<Module> Modules { get; set; }
        IDbSet<Namespace> Namespaces { get; set; }
        IDbSet<CodeType> CodeTypes { get; set; }
        IDbSet<CodeMember> CodeMembers { get; set; }

        Task<int> SaveChangesAsync();
    }
}