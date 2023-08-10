using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Model.Enum;
using Microsoft.Extensions.Options;
using Tibos.Pipeline.Api.Model.Config;
using System.Text.RegularExpressions;
using Tibos.Pipeline.Api.Domain.IService;
using Newtonsoft.Json;
using Tibos.Pipeline.Api.Domain.Service;


namespace TTibos.Pipeline.Api.Domain.Service
{
    public class PublishRecordService : UserInfoExtensions,IPublishRecordService
    {
        private readonly PipelineDBContext _context;
        private readonly IOptions<DockerOptions> _dockerOptions;
        private readonly IKubenetesService _kubenetesService;
        private readonly ILogger<PublishRecordService> _logger;

        public PublishRecordService(ILogger<PublishRecordService> logger,PipelineDBContext context, IOptions<DockerOptions> dockerOptions, IKubenetesService kubenetesService) 
        {
            _context = context;
            _dockerOptions = dockerOptions;
            _kubenetesService = kubenetesService;
            _logger = logger;
        }

        /// <summary>
        /// 创建部署
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> CreatePublish(CreatePublishRequest request)
        {

            JsonResponse response = new JsonResponse();

            try
            {
                var model = await _context.BuildRecord.FirstOrDefaultAsync(m => m.Id == request.BuildRecordId && m.BuildStatus == BuildStatus.success.ToString());
                if(model == null) 
                {
                    response.message = "数据不存在!";
                    response.code = "-1";
                    return response;
                }
                if (!model.EnvId.HasValue) 
                {
                    response.message = "该构建记录未匹配到对应的环境!";
                    response.code = "-1";
                    return response;
                }
                model.PublistStatus = true;
                var env = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == model.EnvId.Value);
                if(env == null) 
                {
                    response.code = "-1";
                    response.message = "尚未创建环境";
                    return response;
                }
                var publishRecord = new PublishRecordEntity()
                {
                    BuildRecordId = request.BuildRecordId,
                    CreateTime = DateTime.Now,
                    Status = env.CheckPublish ? (int)PublishStatus.Audit : (int)PublishStatus.Approved,
                    UserId = UserId,
                    AppId =model.AppId,
                    Remark = request.Remark,
                    EnvId = model.EnvId.Value,
                };
                await _context.PublishRecord.AddAsync(publishRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.message = "服务端错误";
                response.code = "-1";
            }
            return response;
        }

        /// <summary>
        /// 回滚部署
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> RollBackPublish(RollBackPublishRequest request)
        {

            JsonResponse response = new JsonResponse();

            try
            {
                var model = await _context.PublishRecord.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.Id && m.Status == (int)PublishStatus.Success);
                if (model == null)
                {
                    response.message = "数据不存在!";
                    response.code = "-1";
                    return response;
                }
                var env = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == model.EnvId);
                if (env == null)
                {
                    response.code = "-1";
                    response.message = "尚未创建环境";
                    return response;
                }

                model.Id = 0;
                model.Status = env.CheckPublish?(int)PublishStatus.Audit: (int)PublishStatus.Approved;
                model.CreateTime = DateTime.Now;
                model.UserId = UserId;
                await _context.PublishRecord.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.message = "服务端错误";
                response.code = "-1";
            }
            return response;
        }
        

        /// <summary>
        /// 发布应用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task PublishApp(PublishAppRequest request)
        {
            try
            {
                var model = await _context.PublishRecord.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.PublistRecordId && m.Status == (int)PublishStatus.Approved);
                if (model == null) return;
                var res = await PublishAppCheck(new PublishAppCheckRequest() { BuildRecordId = model.BuildRecordId});
                if(res.code != "0") 
                {
                    model.Status = (int)PublishStatus.Error;
                    model.Message = res.message;
                    _context.PublishRecord.Update(model);
                    await _context.SaveChangesAsync();
                    return;
                }

                model.Status = (int)PublishStatus.InProgress;
                _context.PublishRecord.Update(model);
                await _context.SaveChangesAsync();
                var response = await PublishCloud(res.data);
                if (response.code != "0")
                {
                    model.Status = (int)PublishStatus.Error;
                    model.Message = res.message;
                    _context.PublishRecord.Update(model);
                    await _context.SaveChangesAsync();
                    return;
                }

                model.Status = (int)PublishStatus.Success;
                _context.PublishRecord.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"发布应用使用-->{ex.Message} | {ex.StackTrace}");
            }
        }




        /// <summary>
        /// 查询发布记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetPublishListResponse>>> GetPublishList(GetPublishListRequest request)
        {
            JsonResponse<List<GetPublishListResponse>> response = new JsonResponse<List<GetPublishListResponse>>();
            try
            {
                var query = from publish in _context.PublishRecord
                            join build in _context.BuildRecord on publish.BuildRecordId equals build.Id
                            join env in _context.EnvInfo on publish.EnvId equals env.Id
                            join app in _context.AppInfo on publish.AppId equals app.Id
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            join user in _context.UserInfo on publish.UserId equals user.Id
                            select new GetPublishListResponse() 
                            {
                                AppId = app.Id,
                                AppName = app.Name,
                                BuildId = build.BuildId,
                                BuildRecordId = publish.BuildRecordId,
                                CreateTime = publish.CreateTime,
                                EnvId = publish.EnvId,
                                EnvName = env.Name,
                                HomePage = build.HomePage,
                                Id = publish.Id,
                                Message =build.Message,
                                NickName = user.NickName,
                                ProjectName = project.Name,
                                Remark = publish.Remark,
                                SHA = build.SHA,
                                Status = publish.Status,
                                UserId = user.Id,
                                Branch = build.Ref,
                            };
                query = query.Where(m => m.AppId == request.AppId && m.EnvId == request.EnvId);
                var count = await query.CountAsync();

                var list = await query.OrderByDescending(m => m.CreateTime).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
                response.code = "0";
                response.total = count;
                response.data = list;
            }
            catch (Exception ex)
            {
                response.message = "服务端错误";
                response.code = "-1";
            }
            return response;
        }




        /// <summary>
        /// 发布上云
        /// </summary>
        /// <param name="buildId"></param>
        /// <returns></returns>
        private async Task<JsonResponse> PublishCloud(PublishCloudRequest request) 
        {
            JsonResponse response = new JsonResponse();
            try
            {
                //参数转换,符合k8s规范
                var dto = new PublishCloudDto();

                request.EnvInfo.DomainSymbol = string.IsNullOrEmpty(request.EnvInfo.DomainSymbol) ? "" : $"{request.EnvInfo.DomainSymbol}-";

                dto.Name = request.AppName.ToLower().Replace(".","-");
                dto.Domain = $"{request.EnvInfo.DomainSymbol}{dto.Name}.{request.Domain}";
                dto.Namespace = $"{request.EnvInfo.DomainSymbol}{request.Group.ToLower()}";
                dto.Image = $"{_dockerOptions.Value.DokcerRegistry}/{_dockerOptions.Value.Namespace.ToLower()}/{request.AppName.ToLower()}:{request.PipelineId}";

                //创建名称空间
                await _kubenetesService.CreateNamespaceAsync(dto.Namespace);
                //创建密钥
                await _kubenetesService.CreateNamespacedSecretAsync(dto.Namespace, "docker-reg-secret", GetDokcerRegistryDict(), "kubernetes.io/dockerconfigjson");
                //创建部署
                var deployment = await _kubenetesService.GetNamespacedDeploymentAsync(dto.Namespace, dto.Name);
                if (deployment == null)
                {
                    deployment = _kubenetesService.CreateV1DeploymentFactory(new CreateV1DeploymentFactoryRequest()
                    {
                        Image = dto.Image,
                        Name = dto.Name,
                        Ns = dto.Namespace,
                    });
                    await _kubenetesService.CreateNamespacedDeploymentAsync(dto.Namespace, deployment);
                }
                else
                {
                    await _kubenetesService.ReplaceNamespacedDeploymentAsync(dto.Namespace, dto.Name, null, dto.Image);
                }
                //创建服务
                await _kubenetesService.CreateNamespacedServiceAsync(dto.Namespace, dto.Name);
                //创建网关
                await _kubenetesService.CreateNamespacedIngressAsync(dto.Namespace,dto.Name,dto.Domain);

                return response;

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
                return response;
            }
        }


        /// <summary>
        /// 发布应用校验
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<JsonResponse<PublishCloudRequest>> PublishAppCheck(PublishAppCheckRequest request)
        {

            JsonResponse<PublishCloudRequest> response = new JsonResponse<PublishCloudRequest>();
            try
            {
                var query = from build in _context.BuildRecord
                            join app in _context.AppInfo on build.AppId equals app.Id
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            where build.BuildStatus == BuildStatus.success.ToString() && build.Id == request.BuildRecordId
                            select new PublishCloudRequest()
                            {
                                AppName = build.AppName,
                                Group = app.Group,
                                PipelineId = build.PipelineId,
                                Domain = project.Domain,
                                Ref = build.Ref,
                                AppId = build.AppId,
                                EnvId = build.EnvId,
                            };
                var data = await query.FirstOrDefaultAsync();
                if (data == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在";
                    return response;
                }
                var list_env = await _context.EnvInfo.ToListAsync();
                if (list_env.Count() == 0)
                {
                    response.code = "-1";
                    response.message = "尚未创建环境";
                    return response;
                }
                //分支匹配对应的环境
                data.EnvInfo = await _context.EnvInfo.AsNoTracking().FirstOrDefaultAsync(m => m.Id == data.EnvId);
                if (data.EnvInfo == null)
                {
                    response.code = "-1";
                    response.message = "该分支未匹配到对应的环境";
                    return response;
                }
                response.data = data;
                return response;
            }
            catch (Exception ex)
            {
                response.message = "服务端错误";
                response.code = "-1";
            }
            return response;
        }


        #region 生成k8s配置模型

        /// <summary>
        /// DokcerRegistry机密的配置模型
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetDokcerRegistryDict()
        {
            var model = new
            {
                username = _dockerOptions.Value.UserName,
                password = _dockerOptions.Value.Password,
                auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_dockerOptions.Value.UserName}:{_dockerOptions.Value.Password}"))
            };
            var dict = new Dictionary<string, object>()
            {
                { "auths",new Dictionary<string,object>()
                    {
                        {_dockerOptions.Value.DokcerRegistry,model }
                    }
                }
            };
            var res = new Dictionary<string, string>()
            {
                { ".dockerconfigjson",JsonConvert.SerializeObject(dict)}
            };

            return res;
        }

        #endregion

    }


    public class PublishCloudRequest
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 流水线编号
        /// </summary>
        public long PipelineId { get; set; }

        /// <summary>
        /// 根域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 分支
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long? EnvId { get; set; }

        /// <summary>
        /// 环境
        /// </summary>
        public EnvInfoEntity EnvInfo { get; set; }

    }


    public class PublishCloudDto 
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 镜像
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
    }
}
