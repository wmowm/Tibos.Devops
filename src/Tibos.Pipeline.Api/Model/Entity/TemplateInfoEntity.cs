using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 模板
    /// </summary>
    [Table("template_info")]
    public class TemplateInfoEntity
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
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserId { get; set; }


        /// <summary>
        /// 替换变量
        /// </summary>
        public string TempVal { get; set; }

    }
}
