using System.Threading.Tasks;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace TTibos.Pipeline.Api.Domain.IService
{
    public interface IPublishRecordService
    {
        Task<JsonResponse<List<GetPublishListResponse>>> GetPublishList(GetPublishListRequest request);

        Task<JsonResponse> CreatePublish(CreatePublishRequest request);

        Task<JsonResponse> RollBackPublish(RollBackPublishRequest request);

        Task PublishApp(PublishAppRequest request);
    }
}
