namespace Tibos.Pipeline.Api.Model.Response
{
    public class ChildrenResponse
    {
        /// <summary>
        /// 路由
        /// </summary>
        public string router { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public string authority { get; set; }
        /// <summary>
        /// 子路由
        /// </summary>
        public List<ChildrenResponse> children { get; set; }
    }
}
