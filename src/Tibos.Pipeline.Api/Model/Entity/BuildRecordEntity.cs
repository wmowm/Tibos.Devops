﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 构建记录
    /// </summary>
    [Table("build_record")]
    public class BuildRecordEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string HomePage { get; set; }

        /// <summary>
        /// 命令
        /// </summary>
        public string ObjectKind { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string SHA { get; set; }

        /// <summary>
        /// 构建编号
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 流水线编号
        /// </summary>
        public int PipelineId { get; set; }

        /// <summary>
        /// 构建状态
        /// </summary>
        public string BuildStatus { get; set; }

        /// <summary>
        /// 构建时间
        /// </summary>
        public DateTime BuildCreateTime { get; set; }

        /// <summary>
        /// 构建时长
        /// </summary>
        public decimal? BuildDuration { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string RunnerDescription { get; set; }

        /// <summary>
        /// 分支
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 是否为tag
        /// </summary>
        public bool Tag { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long? EnvId { get; set; }

        /// <summary>
        /// 发布状态(0:未发布,1:已发布)
        /// </summary>
        public bool PublistStatus { get; set; }
    }
}
