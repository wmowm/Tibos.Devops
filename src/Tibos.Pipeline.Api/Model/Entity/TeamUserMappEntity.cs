using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{

    [Table("team_user_mapp")]
    public class TeamUserMappEntity
    {
        /// <summary>
        /// 主键,自增
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 团队id
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
