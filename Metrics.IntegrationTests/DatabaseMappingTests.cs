using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Metrics.UnitTests;
using Ploeh.AutoFixture;
using Metrics.Data;
using Xunit;

namespace Metrics.IntegrationTests
{
    public class DatabaseMappingTests
    {
        private string GetConnectionString()
        {
            const string connectionStringName = "TFSCodeMetricsConnection";
            var connectionString = 
                ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;

            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return string.Format(connectionString, location);
        }

        [Fact]
        public void Mappings_AreCorrectly_Setup()
        {
            string connectionString = GetConnectionString();
            TruncateAllTables(connectionString);

            Build expectedBuild = CreateBuild();

            using (var db = new MetricsDbContext(connectionString))
            {
                db.Builds.Add(expectedBuild);

                Task saveTask = db.SaveChangesAsync();
                saveTask.Wait();
            }

            List<Build> allBuilds;
            using (var anotherDbInstance = new MetricsDbContext(connectionString))
            {
                var db = anotherDbInstance;

                var queryTask = (from b in db.Builds select b).ToListAsync();
                queryTask.Wait();

                allBuilds = queryTask.Result;
            }
            allBuilds.ShouldNotBeNullAndHaveCount(1);

            Build actualBuild = allBuilds.First();
            actualBuild.Id.Should().NotBe(0);

            actualBuild.ShouldBeEquivalentTo(expectedBuild, 
                options => options.Including(p => p.Name));
        }

        private Build CreateBuild()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var build = fixture.Create<Build>();
            foreach (var target in build.Targets)
            {
                target.Build = build;
                foreach (var module in target.Modules)
                {
                    module.Target = target;
                    foreach (var ns in module.Namespaces)
                    {
                        ns.Module = module;
                        foreach (var codeType in ns.Types)
                        {
                            codeType.Namespace = ns;
                            foreach (var member in codeType.Members)
                            {
                                member.ParentType = codeType;
                            }
                        }
                    }
                }
            }

            return build;
        }

        private void TruncateAllTables(string connectionString)
        {
            var asmName = Assembly.GetExecutingAssembly().GetName().Name;
            var resourceName = string.Format("{0}.Scripts.DeleteAllData.sql", asmName);
            
            // ReSharper disable once AssignNullToNotNullAttribute
            var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));

            string sql;
            using (streamReader)
                sql = streamReader.ReadToEnd();

            using (var db = new MetricsDbContext(connectionString))
                db.Database.ExecuteSqlCommand(sql);
        }
    }
}
