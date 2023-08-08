namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetBuildListRequest:BaseRequest
    {

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set;}
    }
}
