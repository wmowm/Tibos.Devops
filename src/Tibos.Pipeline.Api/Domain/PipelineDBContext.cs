using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Tibos.Pipeline.Api.Model.Entity;

namespace Tibos.Pipeline.Api.Domain
{



    public class PipelineDBContext : DbContext
    {
        /// <summary>
        /// 日志工厂
        /// </summary>
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
            new DebugLoggerProvider()
        });


        public PipelineDBContext(DbContextOptions<PipelineDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        public DbSet<BuildRecordEntity> BuildRecord { get; set; }

        public DbSet<PublishRecordEntity> PublishRecord { get; set; }

        public DbSet<UserInfoEntity> UserInfo { get; set; }

        public DbSet<UserLoginEntity> UserLogin { get; set; }

        public DbSet<UserFavoriteEntity> UserFavorite { get; set; }

        public DbSet<ProjectInfoEntity> ProjectInfo { get; set; }

        public DbSet<AppInfoEntity> AppInfo { get; set; }

        public DbSet<TeamInfoEntity> TeamInfo { get; set; }

        public DbSet<TeamUserMappEntity> TeamUserMapp { get; set; }

        public DbSet<TemplateInfoEntity> TemplateInfo { get; set; }

        public DbSet<EnvInfoEntity> EnvInfo { get; set; }

        public DbSet<ConfigInfoEntity> ConfigInfo { get; set; }

        public DbSet<ConfigRecordEntity> ConfigRecord { get; set; }


        public DbSet<NodeMetricsEntity> NodeMetrics { get; set; }
        
    }
}
