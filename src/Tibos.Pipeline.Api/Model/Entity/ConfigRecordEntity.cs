﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    [Table("config_record")]
    public class ConfigRecordEntity
    {
        /// <summary>
        /// 主键,自增
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 配置编号
        /// </summary>
        public long ConfigId { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }


        /// <summary>
        /// 挂载路径
        /// </summary>
        public string MountPath { get; set; }

        /// <summary>
        /// 子路径
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 最近修改人
        /// </summary>
        public string UpdateUserId { get; set; }

        /// <summary>
        /// 状态(0:未挂载,1:已挂载)
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 操作类型(0:创建,1:修改设置,2:修改配置,3:挂载设置,4:删除操作)
        /// </summary>
        public int ActionType { get; set; }
    }
}
