using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IConfigInfoService
    {
        Task<JsonResponse> Create(CreateConfigInfoRequest request);
        Task<JsonResponse> Delete(long id);
        Task<JsonResponse<ConfigInfoEntity>> GetById(long id);
        Task<JsonResponse<List<ConfigInfoEntity>>> GetList(GetConfigInfoListRequest request);
        Task<JsonResponse> Redeployment(long envId, long appId);
        Task<JsonResponse> Update(UpdateConfigInfoRequest request);
        Task<JsonResponse> UpdateConfigContent(UpdateConfigContentRequest request);
        Task<JsonResponse> UpdateConfigStatus(UpdateConfigStatusRequest request);

        Task<JsonResponse<List<GetConfigRecordResponse>>> GetConfigRecord(GetConfigRecordRequest request);
    }
}
