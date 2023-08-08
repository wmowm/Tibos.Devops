using System.Threading.Tasks;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IProjectInfoService
    {
        Task<JsonResponse> Create(CreateProjectInfoRequest request);
        Task<JsonResponse> Update(UpdateProjectInfoRequest request);

        Task<JsonResponse> Delete(long id);

        Task<JsonResponse<ProjectInfoEntity>> GetById(long id);

        Task<JsonResponse<List<GetProjectInfoListResponse>>> GetList(GetProjectInfoListRequest request);

        Task<JsonResponse<List<GetProjectInfoListResponse>>> GetUserProjectList();
    }
}
