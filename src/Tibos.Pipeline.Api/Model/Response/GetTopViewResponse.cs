using Tibos.Pipeline.Api.Model.Entity;

namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetTopViewResponse
    {
        /// <summary>
        /// 今日构建数
        /// </summary>
        public int DayBuildCount { get; set; }


        /// <summary>
        /// 昨日构建数
        /// </summary>
        public int YesterDayBuildCount { get; set; }

        /// <summary>
        /// 日环比(今天数/昨天数*100%)
        /// </summary>
        public decimal DayContrast
        { 
            get 
            {
                if (DayBuildCount == 0) return 0;
                if (YesterDayBuildCount == 0) return 0;
                return Convert.ToDecimal(DayBuildCount * 100m / YesterDayBuildCount);
            } 
        }

        /// <summary>
        /// 我的今日构建数
        /// </summary>
        public int MyDayBuildCount { get; set;}




        /// <summary>
        /// 今日发布数
        /// </summary>
        public int DayPublishCount { get; set; }


        /// <summary>
        /// 我的今日发布数
        /// </summary>
        public int MyDayPublishCount { get; set; }

        /// <summary>
        /// 近15天的发布统计
        /// </summary>
        public List<ChartModel> PublishRepList { get; set; }




        /// <summary>
        /// 应用数
        /// </summary>
        public int AppCount { get; set; }


        /// <summary>
        /// 我的应用数
        /// </summary>
        public int MyAppCount { get; set; }

        /// <summary>
        /// 近15天的应用统计
        /// </summary>
        public List<ChartModel> AppRepList { get; set; }


        /// <summary>
        /// 用户数
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 今日活跃用户数
        /// </summary>
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// 节点采集数据集合
        /// </summary>
        public object NodeMetrics { get; set; }

        /// <summary>
        /// 我的应用列表
        /// </summary>
        public List<MyAppModel> MyAppList { get; set; }

    }

    public class ChartModel 
    {
        public string X { get; set; }

        public decimal Y { get; set; }
    }

    public class MyAppModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// pod数量
        /// </summary>
        public int PodCount { get; set; }

        /// <summary>
        /// cpu资源占用
        /// </summary>
        public decimal CpuUsage { get; set; }

        /// <summary>
        /// 内存资源占用
        /// </summary>
        public decimal MemoryUsage { get; set; }
    }
}
