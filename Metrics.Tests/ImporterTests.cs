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
using FluentAssertions;
using Metrics.Data;
using Metrics.Import;
using Ploeh.AutoFixture;
using Xunit;

namespace Metrics.UnitTests
{
    public class ImporterTests
    {
        [Fact]
        public void Importer_Should_Import_Metrics()
        {
            //arrange
            var fixture = new Fixture();

            string expectedBuildName = fixture.Create("BuildName");
            var dataToImport = CreateTestDataToImport(fixture);

            var fakeMetricsStore = new InMemoryMetricsStore();
            var importer = new XmlImporter(fakeMetricsStore);

            //act
            Task importTask = importer.Import(dataToImport, expectedBuildName);
            importTask.Wait();

            //assert
            VerifyBuild(dataToImport, fakeMetricsStore, expectedBuildName);
        }

        private static void VerifyBuild(
            XBuild expectedBuild,
            InMemoryMetricsStore fakeMetricsStore,
            string expectedBuildName)
        {
            var actualBuilds = fakeMetricsStore.Builds;
            actualBuilds.Should().NotBeNull().And.HaveCount(1);
            actualBuilds.Should().OnlyContain(aBuild => aBuild.Name.Equals(expectedBuildName) &&
                                                        aBuild.Metrics.IsNotNullOrEmpty());

            Build actualBuild = actualBuilds.First();
            actualBuild.Should().NotBeNull();

            VerifyTargets(expectedBuild, actualBuild);
        }

        private static void VerifyTargets(XBuild expectedBuild, Build actualBuild)
        {
            var expectedTargets = expectedBuild.Targets.Target;
            var expectedTargetNames = expectedBuild.Targets.Target.Select(t => t.Name);
            
            var actualTargets = actualBuild.Targets;
            actualTargets.Should().NotBeNull();
            actualTargets.Should().NotBeEmpty().And.HaveCount(expectedTargets.Count);
            actualTargets.Should().OnlyContain(aTarget => aTarget.Name.IsIn(expectedTargetNames) &&
                                                          aTarget.Build.IsNotNull() &&
                                                          aTarget.Metrics.IsNotNullOrEmpty());

            VerifyModules(expectedTargets, actualTargets);
        }

        private static void VerifyModules(IEnumerable<XTarget> targets, IEnumerable<Target> actualTargets)
        {
            var expectedModules = targets.SelectMany(t => t.Modules.Module).ToList();
            var expectedModuleNames = expectedModules.Select(m => m.Name);

            var actualModules = actualTargets.SelectMany(t => t.Modules).ToList();
            actualModules.Should().NotBeNull();
            actualModules.Should().NotBeEmpty().And.HaveCount(expectedModules.Count);
            actualModules.Should().OnlyContain(aModule => aModule.Name.IsIn(expectedModuleNames) &&
                                                          aModule.Target.IsNotNull() &&
                                                          aModule.Metrics.IsNotNullOrEmpty());

            VerifyNamespaces(expectedModules, actualModules);
        }

        private static void VerifyNamespaces(IEnumerable<XModule> expectedModules, IEnumerable<Module> actualModules)
        {
            var expectedNamespaces = expectedModules.SelectMany(m => m.Namespaces.Namespace).ToList();
            var expectedNamespaceNames = expectedNamespaces.Select(t => t.Name);
            
            var actualNamespaces = actualModules.SelectMany(m => m.Namespaces).ToList();
            actualNamespaces.Should().NotBeNull();
            actualNamespaces.Should().NotBeEmpty().And.HaveCount(expectedNamespaces.Count);
            actualNamespaces.Should().OnlyContain(aNamespace => aNamespace.Name.IsIn(expectedNamespaceNames) &&
                                                                aNamespace.Module.IsNotNull() &&
                                                                aNamespace.Metrics.IsNotNullOrEmpty());

            VerifyTypes(expectedNamespaces, actualNamespaces);
        }

        private static void VerifyTypes(IEnumerable<XNamespace> expectedNamespaces, IEnumerable<Namespace> actualNamespaces)
        {
            var expectedTypes = expectedNamespaces.SelectMany(n => n.Types.Type).ToList();
            var expectedTypeNames = expectedTypes.Select(t => t.Name);
            
            var actualTypes = actualNamespaces.SelectMany(m => m.Types).ToList();
            actualTypes.Should().NotBeNull();
            actualTypes.Should().NotBeEmpty().And.HaveCount(expectedTypes.Count);
            actualTypes.Should().OnlyContain(aType => aType.Name.IsIn(expectedTypeNames) &&
                                                      aType.Namespace.IsNotNull() &&
                                                      aType.Metrics.IsNotNullOrEmpty());

            VerifyMembers(expectedTypes, actualTypes);
        }

        private static void VerifyMembers(IEnumerable<XType> expectedTypes, IEnumerable<CodeType> actualTypes)
        {
            var expectedMembers = expectedTypes.SelectMany(t => t.Members.Member).ToList();
            var expectedMemberNames = expectedMembers.Select(m => m.Name);
            var actualMembers = actualTypes.SelectMany(t => t.Members).ToList();
            actualMembers.Should().NotBeNull();
            actualMembers.Should().NotBeEmpty().And.HaveCount(expectedMembers.Count);
            actualMembers.Should().OnlyContain(aMember => aMember.Name.IsIn(expectedMemberNames) &&
                                                          aMember.ParentType.IsNotNull() &&
                                                          aMember.Metrics.IsNotNullOrEmpty());
        }

        private XBuild CreateTestDataToImport(Fixture fixture)
        {
            fixture.Customize<XMetric>(c => c.Without(m => m.Untyped));
            fixture.Customize<XType>(c => c.Without(m => m.Untyped));
            fixture.Customize<XNamespace>(c => c.Without(m => m.Untyped));
            fixture.Customize<XModule>(c => c.Without(m => m.Untyped));
            fixture.Customize<XTarget>(c => c.Without(m => m.Untyped));
            fixture.Customize<XBuild>(c => c.Without(m => m.Untyped));

            var data = fixture.Create<XBuild>();

            return data;
        }
    }
}
