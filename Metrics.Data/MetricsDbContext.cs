using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Metrics.Data
{
    public class MetricsDbContext : DbContext, IMetricsStore
    {
        public MetricsDbContext() : base("name=TFSCodeMetricsConnection")
        {    
        }

        public IDbSet<Build> Builds { get; set; }
        public IDbSet<Target> Targets { get; set; }
        public IDbSet<Module> Modules { get; set; }
        public IDbSet<Namespace> Namespaces { get; set; }
        public IDbSet<CodeType> CodeTypes { get; set; }
        public IDbSet<CodeMember> CodeMembers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //http://stackoverflow.com/questions/1334143/sql-server-datetime2-vs-datetime
            modelBuilder.Properties<DateTime>()
                        .Configure(c => c.HasColumnType("datetime2"));
        }
    }
}
