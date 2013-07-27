using System.Data.Entity;
using System.Threading.Tasks;
using Metrics.Data;

namespace Metrics.Tests
{
    class InMemoryMetricsStore : IMetricsStore
    {
        public InMemoryMetricsStore()
        {
            Builds = new InMemoryDbSet<Build>();
            Targets = new InMemoryDbSet<Target>();
            Modules = new InMemoryDbSet<Module>();
            Namespaces = new InMemoryDbSet<Namespace>();
            CodeTypes = new InMemoryDbSet<CodeType>();
            CodeMembers = new InMemoryDbSet<CodeMember>();
        }

        public IDbSet<Build> Builds { get; set; }
        public IDbSet<Target> Targets { get; set; }
        public IDbSet<Module> Modules { get; set; }
        public IDbSet<Namespace> Namespaces { get; set; }
        public IDbSet<CodeType> CodeTypes { get; set; }
        public IDbSet<CodeMember> CodeMembers { get; set; }
        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
