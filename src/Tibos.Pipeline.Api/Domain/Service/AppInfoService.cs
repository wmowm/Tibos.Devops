using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class AppInfoService: UserInfoExtensions,IAppInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IGitlabService _gitlabService;
        private readonly IOptions<GitlabOptions> _gitlabOptions;
        private readonly IKubenetesService _kubenetesService;

        public AppInfoService(PipelineDBContext context, IMapper mapper, IGitlabService gitlabService, IOptions<GitlabOptions> gitlabOptions, IKubenetesService kubenetesService)
        {
            _context = context;
            _mapper = mapper;
            _gitlabService = gitlabService;
            _gitlabOptions = gitlabOptions;
            _kubenetesService = kubenetesService;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateAppInfoRequest request) 
        {
            var response = new JsonResponse();
            try
            {
                var validRule = await GetValidRule();
                if (validRule != null) 
                {
                    var valid = validRule.Contains(request.ProjectId);
                    if (!valid) 
                    {
                        response.code = "-1";
                        response.message = "操作受限";
                        return response;
                    }
                }

                var bl = await _context.AppInfo.AnyAsync(m => m.Name == request.Name && m.Group == request.Group);
                if (bl) 
                {
                    response.code = "-1";
                    response.message = "同一个组内,项目名称唯一";
                    return response;
                }

                var res = await CreateApp(request);
                if(res.code != "0") 
                {
                    response.code = "-1";
                    response.message = res.message;
                    return response;
                }
                var model = _mapper.Map<AppInfoEntity>(request);
                model.Id = res.data.ProjectId;
                model.CreateTime = DateTime.Now;
                model.CreateUserId = UserId;
                model.WebUrl = res.data.WebUrl;
                await _context.AppInfo.AddAsync(model);
                await _context.SaveChangesAsync();
               
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
        public async Task<JsonResponse> Update(UpdateAppInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.AppInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
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
                model.Remark = request.Remark;
                await _context.SaveChangesAsync();

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
                if (!IsAdmin) 
                {
                    response.code = "-1";
                    response.message = "仅管理员可以执行此操作";
                    return response;
                }
                var model = await _context.AppInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    var list_env = await _context.EnvInfo.ToListAsync();
                    foreach (var env in list_env)
                    {

                        var domainSymbol = string.IsNullOrEmpty(env.DomainSymbol) ? "" : $"{env.DomainSymbol}-";
                        K8SModel k8SModel = new K8SModel()
                        {
                           
                            Name = model.Name.ToLower().Replace(".", "-"),
                            Namespace = $"{domainSymbol}{model.Group.ToLower()}",
                            
                        };

                        try
                        {
                            //删除部署
                            await _kubenetesService.DeleteNamespacedDeploymentAsync(k8SModel.Namespace, k8SModel.Name);
                            //删除网关
                            await _kubenetesService.DeleteNamespacedIngressAsync(k8SModel.Namespace, k8SModel.Name);
                            //删除服务
                            await _kubenetesService.DeleteNamespacedServiceAsync(k8SModel.Namespace, k8SModel.Name);
                            //删除配置字典
                            await _kubenetesService.DeleteNamespacedConfigMapAsync(k8SModel.Namespace, k8SModel.Name);
                        }
                        catch
                        {

                        }
                    }
                    _context.AppInfo.Remove(model);
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
        public async Task<JsonResponse<GetAppInfoResponse>> GetById(long id)
        {
            var response = new JsonResponse<GetAppInfoResponse>();
            try
            {
                var query = from app in _context.AppInfo
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            join user in _context.UserInfo on app.CreateUserId equals user.Id
                            join temp in _context.TemplateInfo on app.TemplateId equals temp.Id
                            where id == app.Id
                            select new GetAppInfoResponse()
                            {
                                CreateTime = app.CreateTime,
                                CreateUserName = user.NickName,
                                Group = app.Group,
                                Id = app.Id,
                                Name = app.Name,
                                ProjectId = app.ProjectId,
                                ProjectName = project.Name,
                                Remark = app.Remark,
                                TemplateId = app.TemplateId,
                                TemplateName = temp.Name,
                                WebUrl = app.WebUrl
                            };
                var model = await query.FirstOrDefaultAsync();
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
                model.DomainMap = new Dictionary<string, List<string>>();
                var list_env = await _context.EnvInfo.ToListAsync();
                foreach (var env in list_env)
                {
                    var domainSymbol = string.IsNullOrEmpty(env.DomainSymbol) ? "" : $"{env.DomainSymbol}-";
                    K8SModel k8SModel = new K8SModel()
                    {

                        Name = model.Name.ToLower().Replace(".", "-"),
                        Namespace = $"{domainSymbol}{model.Group.ToLower()}"
                    };

                    var ingress = await _kubenetesService.GetNamespacedIngressAsync(k8SModel.Namespace, k8SModel.Name);
                    var list_domain = new List<string>();
                    if (ingress != null)
                    {
                        //https
                        var https = ingress.Spec.Tls?.FirstOrDefault().Hosts.ToList();
                        //http
                        var http = ingress.Spec.Rules?.Select(m => m.Host).ToList();
                        if (https != null)
                        {
                            foreach (var host in https)
                            {
                                list_domain.Add($"https://{host}");
                            }
                        }
                        if (http != null)
                        {
                            foreach (var host in http)
                            {
                                list_domain.Add($"http://{host}");
                            }
                        }

                    }
                    model.DomainMap.Add(env.Name, list_domain);
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
        public async Task<JsonResponse<List<GetAppInfoListResponse>>> GetList(GetAppInfoListRequest request)
        {
            var response = new JsonResponse<List<GetAppInfoListResponse>>();
            try
            {
                var query = from app in _context.AppInfo
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            join user in _context.UserInfo on app.CreateUserId equals user.Id
                            select new GetAppInfoListResponse() 
                            {
                                CreateTime = app.CreateTime,
                                CreateUserId = app.CreateUserId,
                                CreateUserName = user.NickName,
                                Id = app.Id,
                                Name = app.Name,
                                ProjectId = app.ProjectId,
                                Remark = app.Remark,
                                ProjectName = project.Name,
                                Group = app.Group,
                                WebUrl = app.WebUrl,
                            };

                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => validRule.Contains(m.ProjectId));

                if (request.Id.HasValue) 
                {
                    query = query.Where(m => m.Id == request.Id);
                }
                if (request.ProjectId.HasValue)
                {
                    query = query.Where(m => m.ProjectId == request.ProjectId);
                }
                if (!string.IsNullOrEmpty(request.Name)) 
                {
                    query = query.Where(m => m.Name.Contains(request.Name));
                }
                response.data = await query.OrderByDescending(m=>m.CreateTime).Skip(request.PageSize * (request.PageIndex-1)).Take(request.PageSize).ToListAsync();
                response.total = await query.CountAsync();
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }



        /// <summary>
        /// 应用收藏/取消
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResponse> FavoriteApp(FavoriteAppRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var res = await GetById(request.AppId);
                if(res.code != "0") 
                {
                    response.code = res.code;
                    response.message = res.message;
                    return response;
                }

                var model = await _context.UserFavorite.FirstOrDefaultAsync(m => m.UserId == UserId && m.AppId == request.AppId);
                if (request.FavoriteType) 
                {
                    if (model == null) 
                    {
                        await _context.UserFavorite.AddAsync(new UserFavoriteEntity() 
                        {
                            AppId = request.AppId,
                            UserId = UserId,
                        });
                    }
                }
                else
                {
                    if (model != null) 
                    {
                        _context.UserFavorite.Remove(model);
                    }
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 收藏应用列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<List<AppInfoEntity>>> GetFavoriteAppList() 
        {
            var response = new JsonResponse<List<AppInfoEntity>>();
            try
            {
                var query = from userFavorite in _context.UserFavorite
                            join app in _context.AppInfo on userFavorite.AppId equals app.Id
                            where userFavorite.UserId == UserId
                            select app;
                response.data = await query.OrderBy(m => m.Name).ToListAsync();
                response.total = response.data.Count();
                return response;
            }
            catch (Exception)
            {

                throw;
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


        /// <summary>
        /// 创建应用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<JsonResponse<GitlabCreateAppResponse>> CreateApp(CreateAppInfoRequest request) 
        {
            JsonResponse<GitlabCreateAppResponse> response = new JsonResponse<GitlabCreateAppResponse>();
            response.data = new GitlabCreateAppResponse();
            try
            {
                //1.创建gitlab项目
                //2.pull代码
                //3.根据模板生成项目
                //4.push代码
                //5.删除本地仓库

                var template = await _context.TemplateInfo.FirstOrDefaultAsync(m => m.Id == request.TemplateId);
                if (template == null)
                {
                    response.code = "-1";
                    response.message = "模板不存在";
                    return response;
                }

                var res_group = await _gitlabService.GetGroups();
                if(res_group.code != "0") 
                {
                    response.code = res_group.code;
                    response.message = res_group.message;
                    return response;
                }
                var group = res_group.data.FirstOrDefault(m => m.Name == request.Group);
                if(group == null) 
                {
                    response.code = "-1";
                    response.message = "组不存在";
                    return response;
                }
                var res_project = await _gitlabService.CreaetProject(group.Id, request.Name);
                if (res_project.code != "0")
                {
                    response.code = res_project.code;
                    response.message = res_project.message;
                    return response;
                }
                response.data.WebUrl = res_project.data.web_url;
                response.data.ProjectName = res_project.data.name;
                response.data.ProjectId = res_project.data.id;
                Console.WriteLine("gitlab项目创建成功");


                var appPath = Directory.GetCurrentDirectory() + $"/Template/{request.Name}";

                //拉取代码
                GitHelp.GitPull(_gitlabOptions.Value.Account, _gitlabOptions.Value.Password, res_project.data.http_url_to_repo, appPath);
                //根据模板生成代码
                var tempPath = TemplateHelp.ExtractToDirectory(template.Path);

                if(string.IsNullOrEmpty(tempPath)) 
                {
                    response.code = "-1";
                    response.message = "模板减压错误";
                    return response;
                }
                TemplateHelp.CreateProjectApp(tempPath, appPath, template.TempVal, request.Name);
                GitHelp.GitPush(_gitlabOptions.Value.Account, _gitlabOptions.Value.Password, appPath);

                TemplateHelp.DeleteProjectApp(appPath);

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
