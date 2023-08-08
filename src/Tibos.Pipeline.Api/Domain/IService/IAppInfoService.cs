using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IAppInfoService
    {
        Task<JsonResponse> Create(CreateAppInfoRequest request);
        Task<JsonResponse> Update(UpdateAppInfoRequest request);

        Task<JsonResponse> Delete(long id);

        Task<JsonResponse<GetAppInfoResponse>> GetById(long id);

        Task<JsonResponse<List<GetAppInfoListResponse>>> GetList(GetAppInfoListRequest request);

        Task<JsonResponse> FavoriteApp(FavoriteAppRequest request);

        Task<JsonResponse<List<AppInfoEntity>>> GetFavoriteAppList();

    }
}
