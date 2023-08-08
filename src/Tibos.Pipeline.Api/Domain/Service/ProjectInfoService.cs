using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public class ProjectInfoService: UserInfoExtensions,IProjectInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;

        public ProjectInfoService(PipelineDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateProjectInfoRequest request) 
        {
            var response = new JsonResponse();
            try
            {
                var model = _mapper.Map<ProjectInfoEntity>(request);
                model.CreateTime = DateTime.Now;
                await _context.ProjectInfo.AddAsync(model);
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
        public async Task<JsonResponse> Update(UpdateProjectInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var validRule = await GetValidRule();
                var model = await _context.ProjectInfo.WhereIf(validRule!=null,m=> m.TeamId.HasValue && validRule.Contains(m.TeamId.Value)).FirstOrDefaultAsync(m => m.Id == request.Id);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                model.Name = request.Name;
                model.Remark = request.Remark;
                model.TeamId = request.TeamId;
                model.Domain = request.Domain;
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
                var validRule = await GetValidRule();
                var model = await _context.ProjectInfo.WhereIf(validRule != null, m => m.TeamId.HasValue && validRule.Contains(m.TeamId.Value)).FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    _context.ProjectInfo.Remove(model);
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
        public async Task<JsonResponse<ProjectInfoEntity>> GetById(long id)
        {
            var response = new JsonResponse<ProjectInfoEntity>();
            try
            {
                var validRule = await GetValidRule();
                var model = await _context.ProjectInfo.WhereIf(validRule != null, m => m.TeamId.HasValue && validRule.Contains(m.TeamId.Value)).FirstOrDefaultAsync(m => m.Id == id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
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
        public async Task<JsonResponse<List<GetProjectInfoListResponse>>> GetList(GetProjectInfoListRequest request)
        {
            var response = new JsonResponse<List<GetProjectInfoListResponse>>();
            try
            {
                var query = from project in _context.ProjectInfo
                            join team in _context.TeamInfo on project.TeamId equals team.Id into temp
                            from t in temp.DefaultIfEmpty()
                            select new GetProjectInfoListResponse()
                            {
                                Id = project.Id,
                                CreateTime = project.CreateTime,
                                Name = project.Name,
                                Remark = project.Remark,
                                TeamId = t.Id,
                                TeamName = t.Name,
                                TeamLogo = t.Logo,
                                Domain = project.Domain,
                            };
                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => m.TeamId.HasValue && validRule.Contains(m.TeamId.Value));

                if (request.Id.HasValue) 
                {
                    query = query.Where(m => m.Id == request.Id);
                }
                if (!string.IsNullOrEmpty(request.Name)) 
                {
                    query = query.Where(m => m.Name.Contains(request.Name));
                }
                response.data = await query.OrderByDescending(m=>m.CreateTime).Skip(request.PageSize * (request.PageIndex-1)).Take(request.PageSize).ToListAsync();
                if (response.data.Count() > 0)
                {
                    foreach (var item in response.data) 
                    {
                        item.AppCount = await _context.AppInfo.CountAsync(m => m.ProjectId == item.Id);
                    }
                }
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
        /// 查询我的项目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetProjectInfoListResponse>>> GetUserProjectList()
        {
            var response = new JsonResponse<List<GetProjectInfoListResponse>>();
            try
            {
                var query = from project in _context.ProjectInfo
                            join team in _context.TeamInfo on project.TeamId equals team.Id into temp
                            from t in temp.DefaultIfEmpty()
                            select new GetProjectInfoListResponse()
                            {
                                Id = project.Id,
                                CreateTime = project.CreateTime,
                                Name = project.Name,
                                Remark = project.Remark,
                                TeamId = t.Id,
                                TeamName = t.Name,
                                TeamLogo = t.Logo,
                            };
                var validRule = await GetValidRule();
                query = query.WhereIf(validRule != null, m => m.TeamId.HasValue && validRule.Contains(m.TeamId.Value));

                response.data = await query.OrderByDescending(m => m.CreateTime).ToListAsync();
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
        /// 获取权限校验规则
        /// </summary>
        /// <returns></returns>
        private async Task<List<long>> GetValidRule() 
        {
            //项目归属团队,获取用户(非管理员)所有团队,作为二次校验条件
            if (IsAdmin) return null;
            var list_mapp = await _context.TeamUserMapp.Where(m => m.UserId == UserId).Select(m => m.TeamId).ToListAsync();
            return list_mapp;
        }

        
    }
}
