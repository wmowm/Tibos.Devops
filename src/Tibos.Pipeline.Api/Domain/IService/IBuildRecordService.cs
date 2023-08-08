using System.Threading.Tasks;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace TTibos.Pipeline.Api.Domain.IService
{
    public interface IBuildRecordService
    {
        Task<JsonResponse> CreateOrUpdate(HookRequest request);

        Task<JsonResponse<List<GetBuildListResponse>>> GetBuildList(GetBuildListRequest request);

        Task<JsonResponse<List<GetBuildListResponse>>> GetSuccessBuildList(GetSuccessBuildListRequest request);
    }
}
