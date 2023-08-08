using AutoMapper;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tibos.Domain.Service;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class WorkPlaceService : UserInfoExtensions, IWorkPlaceService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IKubenetesService _kubenetesService;

        public WorkPlaceService(PipelineDBContext context, IMapper mapper, IKubenetesService kubenetesService)
        {
            _context = context;
            _mapper = mapper;
            _kubenetesService = kubenetesService;
        }

        
        /// <summary>
        /// 头部看板视图
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<JsonResponse<GetTopViewResponse>> GetTopView(long envId)
        {
            var response = new JsonResponse<GetTopViewResponse>();
            try
            {
                var dt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                //今日构建数
                var dayBuildCount = await _context.BuildRecord.CountAsync(m=>m.BuildCreateTime >= dt);
                //昨日构建数
                var yesterDayBuildCount = await _context.BuildRecord.CountAsync(m => m.BuildCreateTime >= dt.AddDays(-1) && m.BuildCreateTime< dt);

                var query = from app in _context.AppInfo
                            join build in _context.BuildRecord on app.Id equals build.AppId
                            join userFavorite in _context.UserFavorite on app.Id equals userFavorite.AppId
                            where userFavorite.UserId == UserId && build.BuildCreateTime >= dt
                            select build;
                //我的今日构建数
                var myDayBuildCount= await query.CountAsync();

                //今日发布数
                var dayPublishCount= await _context.PublishRecord.CountAsync(m => m.CreateTime >= dt);
                var queryPublish = from app in _context.AppInfo
                        join publish in _context.PublishRecord on app.Id equals publish.AppId
                        join userFavorite in _context.UserFavorite on app.Id equals userFavorite.AppId
                        where userFavorite.UserId == UserId && publish.CreateTime >= dt
                        select publish;
                //我的今日发布数
                var myDayPublishCount = await queryPublish.CountAsync();


                var queryCount = from publish in _context.PublishRecord
                                 where publish.CreateTime >= dt.AddDays(-15)
                                 group publish by publish.CreateTime.Day into dateGroup
                                 select new ChartModel
                                 {
                                     X = dateGroup.Select(x => x.CreateTime).FirstOrDefault().ToString("yyyy-MM-dd"),
                                     Y = dateGroup.Count()
                                 };
                //近15天的发布统计
                var publishRepList = await queryCount.ToListAsync();


                //应用数
                var appCount = await _context.AppInfo.CountAsync();
                //我的应用数
                var queryApp = await _context.UserFavorite.CountAsync(m=>m.UserId == UserId);
                //近15天新增应用统计
                var queryAppCount = from app in _context.AppInfo
                                 where app.CreateTime >= dt.AddDays(-15)
                                 orderby app.CreateTime.Day descending
                                 group app by app.CreateTime.Day into dateGroup
                                 select new ChartModel
                                 {
                                     X = dateGroup.Select(x => x.CreateTime).FirstOrDefault().ToString("yyyy-MM-dd"),
                                     Y = dateGroup.Count()
                                 };
                var appRepList = await queryAppCount.ToListAsync();

                for (int i = 0; i < 16; i++)
                {
                    var bl = appRepList.Any(m => m.X == dt.AddDays(-i).ToString("yyyy-MM-dd"));
                    if (!bl) 
                    {
                        appRepList.Add(new ChartModel() 
                        {
                            X = dt.AddDays(-i).ToString("yyyy-MM-dd"),
                            Y = 0
                        });
                    }
                }
                appRepList = appRepList.OrderByDescending(m => Convert.ToDateTime(m.X)).ToList();

                //用户数
                var userCount = await _context.UserInfo.CountAsync();

                //活跃用户数
                var activeUserCount = await _context.UserLogin.Where(m=>m.LastLoginTime >= dt).GroupBy(m => m.UserId).CountAsync();


                //节点采集数据集合(100条)
                var nodeMetrics = await _context.NodeMetrics.OrderByDescending(m => m.CreateTime).Take(100).ToListAsync();

                var nodeMetricsDto = nodeMetrics.Select(m => new
                {
                    CreateTime = m.CreateTime.ToString("HH:mm:ss"),
                    m.Cpu,
                    m.Memory
                }).ToList();


                //我的应用列表
                var queryMyApp = from app in _context.AppInfo
                                 join userFavorite in _context.UserFavorite on app.Id equals userFavorite.AppId
                                 where userFavorite.UserId == UserId
                                 select new MyAppModel() 
                                 {
                                     Id = app.Id,
                                     Name = app.Name,
                                 };

                var myAppList = await queryMyApp.ToListAsync();

                foreach (var item in myAppList)
                {
                    var res = await GetK8SModel(envId, item.Id);
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
                    item.PodCount = list_podsMetrics.Items.Count();
                    foreach (var pod in list_podsMetrics.Items)
                    {
                        var podsMetrics = list_podsMetrics.Items.FirstOrDefault(m => m.Metadata.Name == pod.Metadata.Name);
                        if (podsMetrics != null)
                        {
                            var cpuUsage = podsMetrics.Containers.FirstOrDefault(m => m.Name == res.data.Name)?.Usage["cpu"]?.Value;
                            var memoryUsage = podsMetrics.Containers.FirstOrDefault(m => m.Name == res.data.Name)?.Usage["memory"]?.Value;
                            var cpu = KubernetesUnitHelp.UsageUnit(1, cpuUsage);
                            var memory = KubernetesUnitHelp.UsageUnit(2, memoryUsage);
                            item.CpuUsage += cpu.Item1;
                            item.MemoryUsage += memory.Item1;
                        }
                    }
                }


                response.data = new GetTopViewResponse()
                {
                    ActiveUserCount = activeUserCount,
                    AppCount = appCount,
                    AppRepList = appRepList,
                    DayBuildCount = dayBuildCount,
                    DayPublishCount = dayPublishCount,
                    MyAppCount = myAppList.Count,
                    MyAppList = myAppList,
                    MyDayBuildCount = dayBuildCount,
                    MyDayPublishCount = dayPublishCount,
                    NodeMetrics = nodeMetricsDto,
                    PublishRepList = publishRepList,
                    UserCount = userCount,
                    YesterDayBuildCount = yesterDayBuildCount
                };

                return response;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
                return response;
            }
        }



        private async Task<JsonResponse<K8SModel>> GetK8SModel(long envId, long appId)
        {
            JsonResponse<K8SModel> response = new JsonResponse<K8SModel>();
            try
            {
                var env = await _context.EnvInfo.AsNoTracking().FirstOrDefaultAsync(m => m.Id == envId);
                if (env == null)
                {
                    response.code = "-1";
                    response.message = "环境错误";
                    return response;
                }
                var app = await _context.AppInfo.AsNoTracking().FirstOrDefaultAsync(m => m.Id == appId);
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
}
