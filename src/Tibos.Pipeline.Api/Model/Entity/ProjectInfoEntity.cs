using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 用户登录模型
    /// </summary>
    [Table("project_info")]
    public class ProjectInfoEntity
    {
        /// <summary>
        /// 主键,自增
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 团队编号
        /// </summary>
        public long? TeamId { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
    }
}
