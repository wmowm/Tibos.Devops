using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IWorkPlaceService
    {
        Task<JsonResponse<GetTopViewResponse>> GetTopView(long envId);
    }
}
