using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 环境
    /// </summary>
    [Table("env_info")]
    public class EnvInfoEntity
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
        /// 域名
        /// </summary>
        public string DomainSymbol { get; set; }

        /// <summary>
        /// 关联标签
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 标签类型(0:branch,1:tag)
        /// </summary>
        public int TagType { get; set; }

        /// <summary>
        /// 映射配置
        /// </summary>
        public bool MappingConfig { get; set; }

        /// <summary>
        /// 审核发布
        /// </summary>
        public bool CheckPublish { get; set; }
    }
}
