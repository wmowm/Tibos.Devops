using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IContainerService
    {
        Task<JsonResponse<List<GetPodListResponse>>> GetPodList(GetPodListRequest request);
        Task<JsonResponse<List<string>>> GetPodLog(GetPodLogRequest request);
        Task<JsonResponse> RestartPod(GetPodListRequest request);
        Task<JsonResponse> ScalePod(ScalePodRequest request);
    }
}
