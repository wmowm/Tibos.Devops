using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 发布记录
    /// </summary>
    [Table("publish_record")]
    public class PublishRecordEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 发布描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 发布状态(-1:发布失败,0:待审核,1:审核通过,2:发布中,3:发布完成)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }

        /// <summary>
        /// 构建编号
        /// </summary>
        public long BuildRecordId { get; set; }

        /// <summary>
        /// 发布消息
        /// </summary>
        public string Message { get; set; }
    }
}
