extern alias metrix;
//aliases for global metrics xml classes
using XBuild = metrix::CodeMetricsReport;
using XTarget = metrix::Target;
using XModule = metrix::Module;
using XNamespace = metrix::Namespace;
using XType = metrix::Type;
using XMember = metrix::Member;
using XMetric = metrix::Metric;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metrics.Data;

namespace Metrics.Import
{
    public class XmlImporter
    {
        private readonly IMetricsStore _metricsStore;

        public XmlImporter(IMetricsStore metricsStore)
        {
            _metricsStore = metricsStore;
        }

        public async Task Import(XBuild metricsXmlData, string buildName)
        {
            var build = new Build
            {
                Name = buildName, 
                Targets = new List<Target>()
            };

            CreateTargets(build, metricsXmlData.Targets.Target);

            UpdateBuildMetrics(build);

            _metricsStore.Builds.Add(build);

            await _metricsStore.SaveChangesAsync();
        }

        private static void CreateTargets(Build build, IEnumerable<XTarget> targets)
        {
            foreach (var target in targets)
            {
                var buildTarget = new Target
                {
                    Name = target.Name,
                    Build = build,
                    Modules = new List<Module>()
                };

                CreateModules(buildTarget, target.Modules.Module);

                UpdateTargetMetrics(buildTarget);

                build.Targets.Add(buildTarget);
            }
        }

        private static void CreateModules(Target target, IEnumerable<XModule> modules)
        {
            foreach (var module in modules)
            {
                var moduleInTarget = new Module
                {
                    Name = module.Name,
                    Target = target,
                    Namespaces = new List<Namespace>(),
                    Metrics = CreateMetrics(module.Metrics.Metric)
                };

                CreateNamespaces(moduleInTarget, module.Namespaces.Namespace);

                target.Modules.Add(moduleInTarget);
            }
        }

        private static void CreateNamespaces(Module module, IEnumerable<XNamespace> namespaces)
        {
            foreach (var ns in namespaces)
            {
                var nsInModule = new Namespace
                {
                    Name = ns.Name,
                    Module = module,
                    Types = new List<CodeType>(),
                    Metrics = CreateMetrics(ns.Metrics.Metric)
                };

                CreateTypes(nsInModule, ns.Types.Type);

                module.Namespaces.Add(nsInModule);
            }
        }

        private static void CreateTypes(Namespace ns, IEnumerable<XType> types)
        {
            foreach (var type in types)
            {
                var typeInNamespace = new CodeType
                {
                    Name = type.Name,
                    Namespace = ns,
                    Metrics = CreateMetrics(type.Metrics.Metric),
                    Members = new List<CodeMember>()
                };

                CreateMembers(typeInNamespace, type.Members.Member);

                ns.Types.Add(typeInNamespace);
            }
        }

        private static void CreateMembers(CodeType type, IEnumerable<XMember> members)
        {
            foreach (var member in members)
            {
                var memberInType = new CodeMember
                {
                    Name = member.Name,
                    ParentType = type,
                    Metrics = CreateMetrics(member.Metrics.Metric)
                };

                type.Members.Add(memberInType);
            }
        }

        private static void UpdateBuildMetrics(Build build)
        {
            var buildMetrics = 
                build.Targets
                     .SelectMany(t => t.Metrics)
                     .GroupBy(m => m.Name)
                     .Select(m => new Metric
                     {
                         Name = m.Key,
                         Value = m.Average(v => v.Value)
                     });

            build.Metrics = buildMetrics.ToList();
        }

        private static void UpdateTargetMetrics(Target target)
        {
            var targetMetrics =
                target.Modules
                      .SelectMany(t => t.Metrics)
                      .GroupBy(m => m.Name)
                      .Select(m => new Metric
                      {
                          Name = m.Key,
                          Value = m.Average(v => v.Value)
                      });

            target.Metrics = targetMetrics.ToList();
        }

        private static ICollection<Metric> CreateMetrics(IEnumerable<XMetric> metrics)
        {
            var convertedMetrics = 
                metrics.Select(m => new Metric
                {
                    Name = m.Name,
                    Value = ParseDecimalSafe(m.Value)
                });

            return convertedMetrics.ToList();
        }

        private static decimal ParseDecimalSafe(string value)
        {
            decimal decimalvalue;
            decimal.TryParse(value, out decimalvalue);
            return decimalvalue;
        }

    }
}
