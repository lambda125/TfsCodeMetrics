extern alias metrix;
//aliases for global metrics xml classes
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Metrics.UnitTests;
using Xunit;
using XBuild = metrix::CodeMetricsReport;
using XTarget = metrix::Target;
using XModule = metrix::Module;
using XNamespace = metrix::Namespace;
using XType = metrix::Type;
using XMember = metrix::Member;
using XMetric = metrix::Metric;

namespace Metrics.IntegrationTests
{
    public class XmlParsingTests
    {
        [Fact]
        public void Parsing_MetricsXmlFile_Should_CreateBuildObjects()
        {
            var xBuild = XBuild.Load(new StreamReader("Sample.xml"));

            xBuild.Should().NotBeNull();          
            xBuild.Targets.Should().NotBeNull();
            var xTargets = xBuild.Targets.Target;
            xTargets.ShouldNotBeNullAndHaveCount(3);
            xTargets.Should().OnlyContain(aTarget => aTarget.Name.IsNotNullOrEmpty() &&
                                                     aTarget.Modules.IsNotNull());

            var xModules = xTargets.First().Modules.Module;
            xModules.ShouldNotBeNullAndHaveCount(3);
            xModules.Should().OnlyContain(aModule => aModule.Name.IsNotNullOrEmpty() &&
                                                     aModule.Metrics.IsNotNull() &&
                                                     aModule.Namespaces.IsNotNull());
            VerifyMetricsAreOk(xModules.First().Metrics.Metric, expectedCount: 3);

            var xNamespaces = xModules.First().Namespaces.Namespace;
            xNamespaces.ShouldNotBeNullAndHaveCount(3);
            xNamespaces.Should().OnlyContain(aNamespace => aNamespace.Name.IsNotNullOrEmpty() &&
                                                           aNamespace.Metrics.IsNotNull() &&
                                                           aNamespace.Types.IsNotNull());
            VerifyMetricsAreOk(xNamespaces.First().Metrics.Metric, expectedCount: 3);

            var xTypes = xNamespaces.First().Types.Type;
            xTypes.ShouldNotBeNullAndHaveCount(3);
            xTypes.Should().OnlyContain(aType => aType.Name.IsNotNullOrEmpty() &&
                                                 aType.Metrics.IsNotNull() &&
                                                 aType.Members.IsNotNull());

            var xMembers = xTypes.First().Members.Member;
            xMembers.ShouldNotBeNullAndHaveCount(3);
            xMembers.Should().OnlyContain(aMember => aMember.Name.IsNotNullOrEmpty() &&
                                                     aMember.Metrics.IsNotNull());
            VerifyMetricsAreOk(xMembers.First().Metrics.Metric, expectedCount: 3);
        }

        private void VerifyMetricsAreOk(IList<XMetric> xMetrics, int expectedCount)
        {
            xMetrics.ShouldNotBeNullAndHaveCount(expectedCount);
            xMetrics.Should().OnlyContain(aMetric => aMetric.Name.IsNotNullOrEmpty() &&
                                                     aMetric.Value.IsNotNullOrEmpty());
        }
    }

}
