namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetPodLogRequest: GetPodListRequest
    {
        /// <summary>
        /// pod名称
        /// </summary>
        public string PodName { get; set; }
    }
}
