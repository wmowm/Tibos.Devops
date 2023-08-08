using System.Threading.Tasks;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IEnvInfoService
    {
        Task<JsonResponse> Create(CreateEnvInfoRequest request);
        Task<JsonResponse> Update(UpdateEnvInfoRequest request);

        Task<JsonResponse> Delete(long id);

        Task<JsonResponse<EnvInfoEntity>> GetById(long id);

        Task<JsonResponse<List<EnvInfoEntity>>> GetList(GetEnvInfoListRequest request);

        Task<JsonResponse> UpdateMappingConfig(UpdateMappingConfigRequest request);

        Task<JsonResponse> UpdateCheckPublish(UpdateCheckPublishRequest request);
    }
}
