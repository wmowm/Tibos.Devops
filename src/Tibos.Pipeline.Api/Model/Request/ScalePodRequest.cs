namespace Tibos.Pipeline.Api.Model.Request
{
    public class ScalePodRequest: GetPodListRequest
    {
        /// <summary>
        /// pod数
        /// </summary>
        public int Replicas { get; set; }
    }
}
