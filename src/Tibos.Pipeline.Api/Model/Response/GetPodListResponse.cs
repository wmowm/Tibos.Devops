namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetPodListResponse
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 重启次数
        /// </summary>
        public int? Restarts { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// cpu资源占用
        /// </summary>
        public string CpuUsage { get; set; }

        /// <summary>
        /// 内存资源占用
        /// </summary>
        public string MemoryUsage { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }
}
