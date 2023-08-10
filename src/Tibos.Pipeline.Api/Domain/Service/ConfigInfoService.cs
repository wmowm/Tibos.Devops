using AutoMapper;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class ConfigInfoService : UserInfoExtensions, IConfigInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IKubenetesService _kubenetesService;

        public ConfigInfoService(PipelineDBContext context, IMapper mapper, IKubenetesService kubenetesService)
        {
            _context = context;
            _mapper = mapper;
            _kubenetesService = kubenetesService;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateConfigInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {

                var app = await _context.AppInfo.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.AppId);
                if (app == null)
                {
                    response.code = "-1";
                    response.message = "应用不存在!";
                    return response;
                }
                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(app.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }

                var model = _mapper.Map<ConfigInfoEntity>(request);
                model.ProjectId = app.ProjectId;
                model.Remark=model.Remark??"-";
                model.CreateTime = DateTime.Now;
                model.CreateUserId = UserId;
                model.UpdateUserId = UserId;
                await _context.ConfigInfo.AddAsync(model);
                await _context.SaveChangesAsync();

                await AddConfigRecord(model, ConfigActionType.Create);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Update(UpdateConfigInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.ConfigInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(model.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }
                model.SubPath = request.SubPath;
                model.MountPath = request.MountPath;
                model.Remark = request.Remark??"-";
                model.UpdateUserId = UserId;
                await _context.SaveChangesAsync();

                await AddConfigRecord(model, ConfigActionType.Setting);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Delete(long id)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.ConfigInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                await AddConfigRecord(model, ConfigActionType.Delete);

                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(model.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }
                if (model != null)
                {
                    _context.ConfigInfo.Remove(model);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResponse<ConfigInfoEntity>> GetById(long id)
        {
            var response = new JsonResponse<ConfigInfoEntity>();
            try
            {
                var model = await _context.ConfigInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(model.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }
                response.data = model;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<ConfigInfoEntity>>> GetList(GetConfigInfoListRequest request)
        {
            var response = new JsonResponse<List<ConfigInfoEntity>>();
            try
            {
                var query = _context.ConfigInfo.AsQueryable().Where(m=>m.EnvId==request.EnvId && m.AppId == request.AppId);
                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => validRule.Contains(m.ProjectId));
                response.data = await query.OrderByDescending(m => m.CreateTime).ToListAsync();
                response.total = await query.CountAsync();
                if(response.data.Count > 0) 
                {
                    var res = await GetK8SModel(response.data.FirstOrDefault().EnvId, response.data.FirstOrDefault().AppId);
                    if(res.code != "0") 
                    {
                        response.code = res.code;
                        response.message = res.message;
                        return response;
                    }

                    foreach(var item in response.data) 
                    {
                        var result = await GetVolumeMountsStatus(res.data, item.SubPath);
                        item.Status = result.data;
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }



        /// <summary>
        /// 修改配置内容
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateConfigContent(UpdateConfigContentRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.ConfigInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(model.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }
                model.Content = request.Content;
                await _context.SaveChangesAsync();
                //TODO 重启容器
                if (request.RestartContainer)
                {
                    response = await Redeployment(model.EnvId, model.AppId);
                }
                await AddConfigRecord(model, ConfigActionType.EditConfig);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 修改配置状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateConfigStatus(UpdateConfigStatusRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.ConfigInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                var validRule = await GetValidRule();
                if (validRule != null)
                {
                    var valid = validRule.Contains(model.ProjectId);
                    if (!valid)
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }
                model.Status = request.Status;
                await _context.SaveChangesAsync();
                //TODO 重启容器
                response = await Redeployment(model.EnvId, model.AppId);
                await AddConfigRecord(model, ConfigActionType.EditMounts);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 刷新挂载配置
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Redeployment(long envId, long appId)
        {
            var response = new JsonResponse();
            try
            {
                var res = await GetK8SModel(envId, appId);
                if (res.code != "0")
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                //获取该应用,所有已挂载的配置
                var list = await _context.ConfigInfo.AsNoTracking().Where(m => m.EnvId == envId && m.AppId == appId && m.Status).ToListAsync();
                var dict = new Dictionary<string, string>();
                //卷集合
                List<V1Volume> volumes = new List<V1Volume>();
                var volume = new V1Volume()
                {
                    ConfigMap = new V1ConfigMapVolumeSource()
                    {
                        DefaultMode = 420,
                        Name = res.data.Name,
                        Items = new List<V1KeyToPath>()
                    },
                    Name = "config"
                };
                //挂载卷集合
                List<V1VolumeMount> volumeMounts = new List<V1VolumeMount>();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        dict.Add(item.SubPath, item.Content);

                        //添加卷
                        volume.ConfigMap.Items.Add(new V1KeyToPath()
                        {
                            Key = item.SubPath,
                            Path = item.SubPath
                        });

                        //挂载卷
                        volumeMounts.Add(new V1VolumeMount()
                        {
                            MountPath = item.MountPath,
                            Name = "config",
                            ReadOnlyProperty = true,
                            SubPath = item.SubPath,
                        });

                    }
                }
                volumes.Add(volume);

                var configMap = await _kubenetesService.GetNamespacedConfigMapAsync(res.data.Namespace, res.data.Name);
                if (configMap == null)
                {
                    await _kubenetesService.CreateNamespacedConfigMapAsync(res.data.Namespace, res.data.Name, dict);
                }
                else
                {
                    await _kubenetesService.ReplaceNamespacedConfigMapAsync(res.data.Namespace, res.data.Name, dict);
                }

                await _kubenetesService.ReplaceNamespacedDeploymentAsync(res.data.Namespace, res.data.Name, volumes, volumeMounts);

                await _kubenetesService.DeleteNamespacedPodAsync(res.data.Namespace, res.data.Name);
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
        /// 查询配置修改记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetConfigRecordResponse>>> GetConfigRecord(GetConfigRecordRequest request)
        {
            var response = new JsonResponse<List<GetConfigRecordResponse>>();
            try
            {
                var query = from record in _context.ConfigRecord
                            join app in _context.AppInfo on record.AppId equals app.Id
                            join project in _context.ProjectInfo on record.ProjectId equals project.Id
                            join user in _context.UserInfo on record.UpdateUserId equals user.Id
                            where record.EnvId == request.EnvId && record.AppId == request.AppId
                            select new GetConfigRecordResponse() 
                            {
                                ActionType = record.ActionType,
                                AppId = record.AppId,
                                AppName = app.Name,
                                ConfigId = record.ConfigId,
                                Content = record.Content,
                                CreateTime = record.CreateTime,
                                CreateUserId = record.CreateUserId,
                                EnvId = record.EnvId,
                                Id = record.Id,
                                MountPath = record.MountPath,
                                ProjectId = record.ProjectId,
                                ProjectName = project.Name,
                                Remark = record.Remark,
                                Status = record.Status,
                                SubPath = record.SubPath,
                                UpdateUserAvatarUrl = user.AvatarUrl,
                                UpdateUserId = record.UpdateUserId,
                                UpdateUserName = user.NickName
                            };

                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => validRule.Contains(m.ProjectId));
                response.data = await query.OrderByDescending(m => m.CreateTime).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
                response.total = await query.CountAsync();
                if(response.data.Count > 0) 
                {
                    foreach (var item in response.data)
                    {
                        item.ActionTypeName = EnumberHelper.EnumToList<ConfigActionType>().FirstOrDefault(m => m.EnumValue == item.ActionType).EnumName;
                        item.ActionTypeDesction = EnumberHelper.EnumToList<ConfigActionType>().FirstOrDefault(m => m.EnumValue == item.ActionType).Desction;
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }



        /// <summary>
        /// 查询挂载状态
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        private async Task<JsonResponse<bool>> GetVolumeMountsStatus(K8SModel model, string subPath) 
        {
            JsonResponse<bool> response = new JsonResponse<bool>();
            try
            {
                var deployment = await _kubenetesService.GetNamespacedDeploymentAsync(model.Namespace, model.Name);
                if(deployment == null) 
                {
                    response.code = "-1";
                    response.message = "部署错误";
                    return response;
                }
                response.data = deployment.Spec.Template.Spec.Containers.FirstOrDefault().VolumeMounts.Any(m => m.SubPath == subPath);
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
        /// 获取权限校验规则
        /// </summary>
        /// <returns></returns>
        private async Task<List<long>> GetValidRule()
        {
            //应用归属项目,项目归属团队,获取用户(非管理员)所有项目,作为二次校验条件
            if (IsAdmin) return null;
            var query = from mapp in _context.TeamUserMapp
                        join project in _context.ProjectInfo on mapp.TeamId equals project.TeamId
                        where mapp.UserId == UserId
                        select project.Id;
            return await query.ToListAsync();
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


        /// <summary>
        /// 记录配置操作记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private async Task<JsonResponse> AddConfigRecord(ConfigInfoEntity model,ConfigActionType actionType) 
        {
            JsonResponse response = new JsonResponse();
            try
            {
                var dto = _mapper.Map<ConfigRecordEntity>(model);
                dto.Id = 0;
                dto.ConfigId = model.Id;
                dto.ActionType = (int)actionType;
                dto.CreateTime = DateTime.Now;
                await _context.AddAsync(dto);
                await _context.SaveChangesAsync();
                return  response;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
                return response;
            }
        }
    }
}
