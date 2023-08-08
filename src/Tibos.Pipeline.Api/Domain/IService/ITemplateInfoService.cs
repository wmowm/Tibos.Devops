using System.Threading.Tasks;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface ITemplateInfoService
    {
        Task<JsonResponse> Create(CreateTemplateInfoDto request);
        Task<JsonResponse> Update(UpdateTemplateInfoRequest request);

        Task<JsonResponse> Delete(long id);

        Task<JsonResponse<TemplateInfoEntity>> GetById(long id);

        Task<JsonResponse<List<GetTemplateInfoListResponse>>> GetList(GetTemplateInfoListRequest request);

    }
}
