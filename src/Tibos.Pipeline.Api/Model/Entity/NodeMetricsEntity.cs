using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 节点监控数据
    /// </summary>
    [Table("node_metrics")]
    public class NodeMetricsEntity
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
        /// cpu用量
        /// </summary>
        public decimal Cpu { get; set; }

        /// <summary>
        /// 内存用量
        /// </summary>
        public decimal Memory { get; set; }

        /// <summary>
        /// cpu用量单位
        /// </summary>
        public string CpuUnit { get; set; }

        /// <summary>
        /// 内存用量单位
        /// </summary>
        public string MemoryUnit { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
