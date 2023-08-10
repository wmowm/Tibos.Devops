using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Model.Enum;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class BuildRecordService : UserInfoExtensions,IBuildRecordService
    {

        private readonly PipelineDBContext _context;

        public BuildRecordService(PipelineDBContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// 生成构建记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> CreateOrUpdate(HookRequest request)
        {
            //created->pending->running->success
            JsonResponse response = new JsonResponse();
            using (var db = _context)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var model = await _context.BuildRecord.AsQueryable().FirstOrDefaultAsync(m => m.AppId == request.project_id && m.BuildId == request.build_id);
                        if (model == null)
                        {
                            model = new BuildRecordEntity()
                            {
                                AppId = request.project_id,
                                AppName = request.repository?.name,
                                BuildCreateTime = request.build_created_at.GetFmortDateTime(),
                                BuildDuration = request.build_duration,
                                BuildId = request.build_id,
                                PipelineId = request.pipeline_id,
                                BuildStatus = request.build_status,
                                HomePage = request.repository?.homepage,
                                SHA = request.sha,
                                Message = request.commit?.message,
                                ObjectKind = request.object_kind,
                                UserName = request.user?.username,
                                RunnerDescription = request.runner?.description,
                                Ref = request.@ref,
                                Tag = request.tag,
                                EnvId = (await AdaptiveEnv(request.@ref))?.Id
                            };
                            await _context.BuildRecord.AddAsync(model);
                        }
                        else
                        {
                            model.BuildCreateTime = request.build_created_at.GetFmortDateTime();
                            model.BuildDuration = request.build_duration;
                            model.BuildStatus = request.build_status;
                            model.RunnerDescription = request.runner.description;
                            _context.Update(model);
                        }
                        await _context.SaveChangesAsync();
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return response;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        response.code = "-1";
                        response.message = ex.Message;
                        return response;
                    }
                }
            }
           
        }


        /// <summary>
        /// 查询构建记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetBuildListResponse>>> GetBuildList(GetBuildListRequest request) 
        {
            JsonResponse<List<GetBuildListResponse>> response = new JsonResponse<List<GetBuildListResponse>>();
            try
            {
                var query = from build in _context.BuildRecord
                            join app in _context.AppInfo on build.AppId equals app.Id
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            join team in _context.TeamInfo on project.TeamId equals team.Id
                            join env in _context.EnvInfo on build.EnvId equals env.Id
                            select new GetBuildListResponse() 
                            {
                                AppId = app.Id,
                                AppName = app.Name,
                                BuildCreateTime = build.BuildCreateTime,
                                BuildDuration = build.BuildDuration,
                                BuildId = build.BuildId,
                                BuildStatus = build.BuildStatus,
                                HomePage = build.HomePage,
                                Id = build.Id,
                                Message = build.Message,
                                ObjectKind = build.ObjectKind,
                                PipelineId = build.PipelineId,
                                ProjectId = project.Id,
                                ProjectName = project.Name,
                                RunnerDescription = build.RunnerDescription,
                                SHA = build.SHA,
                                TeamName = team.Name,
                                UserName = build.UserName,
                                Ref = build.Ref,
                                Tag = build.Tag,
                                EnvId = build.EnvId,
                                EnvName = env.Name
                            };

                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => validRule.Contains(m.ProjectId));

                if (!string.IsNullOrEmpty(request.AppName))
                {
                    query = query.Where(m => m.AppName == request.AppName);
                }
                if (!string.IsNullOrEmpty(request.ProjectName))
                {
                    query = query.Where(m => m.ProjectName == request.ProjectName);
                }
                var count = await query.CountAsync();

                var list = await query.OrderByDescending(m=>m.PipelineId).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
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
        /// 查询成功的构建记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetBuildListResponse>>> GetSuccessBuildList(GetSuccessBuildListRequest request)
        {
            JsonResponse<List<GetBuildListResponse>> response = new JsonResponse<List<GetBuildListResponse>>();
            try
            {
                var query = from build in _context.BuildRecord
                            join app in _context.AppInfo on build.AppId equals app.Id
                            join project in _context.ProjectInfo on app.ProjectId equals project.Id
                            join team in _context.TeamInfo on project.TeamId equals team.Id
                            join env in _context.EnvInfo on build.EnvId equals env.Id
                            select new GetBuildListResponse()
                            {
                                AppId = app.Id,
                                AppName = app.Name,
                                BuildCreateTime = build.BuildCreateTime,
                                BuildDuration = build.BuildDuration,
                                BuildId = build.BuildId,
                                BuildStatus = build.BuildStatus,
                                HomePage = build.HomePage,
                                Id = build.Id,
                                Message = build.Message,
                                ObjectKind = build.ObjectKind,
                                PipelineId = build.PipelineId,
                                ProjectId = project.Id,
                                ProjectName = project.Name,
                                RunnerDescription = build.RunnerDescription,
                                SHA = build.SHA,
                                TeamName = team.Name,
                                UserName = build.UserName,
                                Ref = build.Ref,
                                Tag = build.Tag,
                                EnvId = build.EnvId,
                                EnvName = env.Name,
                                PublistStatus = build.PublistStatus,
                            };

                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => validRule.Contains(m.ProjectId));

                query = query.Where(m => m.BuildStatus == BuildStatus.success.ToString());
                query = query.Where(m => m.AppId == request.AppId);
                query = query.Where(m => m.EnvId == request.EnvId);

                var count = await query.CountAsync();

                var list = await query.OrderByDescending(m => m.PipelineId).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
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
        /// 分支匹配对应的环境
        /// </summary>
        /// <param name="list_env"></param>
        /// <param name="branch"></param>
        private async Task<EnvInfoEntity> AdaptiveEnv(string branch)
        {
            var list_env = await _context.EnvInfo.ToListAsync();
            foreach (var env in list_env)
            {
                if (string.IsNullOrEmpty(env.Key)) continue;
                if (env.Key.Substring(env.Key.Length - 1, 1) == "*")
                {
                    env.Key = env.Key.Substring(0, env.Key.Length - 1);
                }
                if (branch.Length >= env.Key.Length && branch.Substring(0, env.Key.Length) == env.Key)
                {
                    return env;
                }
            }
            return null;
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
    }
}
