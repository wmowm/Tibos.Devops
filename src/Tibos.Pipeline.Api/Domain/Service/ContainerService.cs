using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    /// <summary>
    /// 容器服务
    /// </summary>
    public class ContainerService : IContainerService
    {
        private readonly IKubenetesService _kubenetesService;
        private readonly PipelineDBContext _context;
        public ContainerService(IKubenetesService kubenetesService, PipelineDBContext context)
        {
            _kubenetesService = kubenetesService;
            _context = context;
        }

        /// <summary>
        /// 获取pod列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetPodListResponse>>> GetPodList(GetPodListRequest request)
        {
            JsonResponse<List<GetPodListResponse>> response = new JsonResponse<List<GetPodListResponse>>();
            try
            {
                var res = await GetK8SModel(request.EnvId, request.AppId);
                if (res.code != "0")
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                var list = await _kubenetesService.ListNamespacedPodAsync(res.data.Namespace, res.data.Name);

                PodMetricsList list_podsMetrics = new PodMetricsList();
                //pod资源占用(未安装普罗米修斯插件,会有异常)
                try
                {
                    list_podsMetrics = await _kubenetesService.GetKubernetesPodsMetricsByNamespaceAsync(res.data.Namespace, res.data.Name);
                }
                catch
                {
                }

                response.data = new List<GetPodListResponse>();
                foreach (var pod in list.Items)
                {
                    GetPodListResponse model = new GetPodListResponse();
                    model.Name = pod.Metadata.Name;
                    model.Restarts = pod.Status.ContainerStatuses.FirstOrDefault()?.RestartCount;
                    //时区转换
                    model.CreateTime = pod.Status.StartTime.HasValue? pod.Status.StartTime.Value.AddHours(8): pod.Status.StartTime;
                    model.Status = pod.Status.Phase;
                    if (list_podsMetrics.Items != null)
                    {
                        var podsMetrics = list_podsMetrics.Items.FirstOrDefault(m => m.Metadata.Name == pod.Metadata.Name);
                        if (podsMetrics != null)
                        {
                            model.CpuUsage = podsMetrics.Containers.FirstOrDefault(m => m.Name == res.data.Name)?.Usage["cpu"]?.Value;
                            model.MemoryUsage = podsMetrics.Containers.FirstOrDefault(m => m.Name == res.data.Name)?.Usage["memory"]?.Value;
                            var cpu = KubernetesUnitHelp.UsageUnit(1, model.CpuUsage);
                            var memory = KubernetesUnitHelp.UsageUnit(2, model.MemoryUsage);
                            model.CpuUsage = string.IsNullOrEmpty(cpu.Item2)?cpu.Item3 : $"{cpu.Item1}{cpu.Item2}";
                            model.MemoryUsage = string.IsNullOrEmpty(memory.Item2) ? memory.Item3 : $"{memory.Item1}{memory.Item2}";
                        }
                    }
                    response.data.Add(model);
                }
                response.total = response.data.Count;
                return response;

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }

        /// <summary>
        /// 重启容器
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> RestartPod(GetPodListRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var res = await GetK8SModel(request.EnvId, request.AppId);
                if (res.code != "0")
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                await _kubenetesService.DeleteNamespacedPodAsync(res.data.Namespace, res.data.Name);
                return response;
            }
            catch (Exception)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }


        /// <summary>
        /// 伸缩pod
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> ScalePod(ScalePodRequest request)
        {
            var response = new JsonResponse();
            try
            {
                request.Replicas = request.Replicas < 0 ? 0 : request.Replicas;
                request.Replicas = request.Replicas>10?10:request.Replicas;
                var res = await GetK8SModel(request.EnvId, request.AppId);
                if (res.code != "0")
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                await _kubenetesService.ReplaceNamespacedDeploymentAsync(res.data.Namespace, res.data.Name, request.Replicas);
                return response;
            }
            catch (Exception)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }


        /// <summary>
        /// 查看日志
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<string>>> GetPodLog(GetPodLogRequest request)
        {
            var response = new JsonResponse<List<string>>();
            try
            {
                var res = await GetK8SModel(request.EnvId, request.AppId);
                if (res.code != "0")
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                response.data = await _kubenetesService.ReadNamespacedPodLogAsync(res.data.Namespace, request.PodName);
                return response;
            }
            catch (Exception)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }

        private async Task<JsonResponse<K8SModel>> GetK8SModel(long envId, long appId)
        {
            JsonResponse<K8SModel> response = new JsonResponse<K8SModel>();
            try
            {
                var env = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == envId);
                if (env == null)
                {
                    response.code = "-1";
                    response.message = "环境错误";
                    return response;
                }
                var app = await _context.AppInfo.FirstOrDefaultAsync(m => m.Id == appId);
                if (app == null)
                {
                    response.code = "-1";
                    response.message = "应用错误";
                    return response;
                }
                response.data = new K8SModel();
                env.DomainSymbol = string.IsNullOrEmpty(env.DomainSymbol) ? "" : $"{env.DomainSymbol}-";

                response.data.Name = app.Name.ToLower().Replace(".", "-");
                response.data.Namespace = $"{env.DomainSymbol}{app.Group.ToLower()}";
                return response;

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误";
                return response;
            }
        }

     
    }
    public class K8SModel
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称空间
        /// </summary>
        public string Namespace { get; set; }
    }
}
