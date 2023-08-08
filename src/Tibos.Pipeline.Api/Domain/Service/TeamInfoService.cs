using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class TeamInfoService : ITeamInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;

        public TeamInfoService(PipelineDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateTeamInfoRequest request) 
        {
            var response = new JsonResponse();
            try
            {
                var model = _mapper.Map<TeamInfoEntity>(request);
                model.Groups = string.Join(",",request.Groups);
                model.Domains = string.Join(",", request.Domains.Where(m=>!string.IsNullOrEmpty(m)));
                model.CreateTime = DateTime.Now;
                await _context.TeamInfo.AddAsync(model);
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
        public async Task<JsonResponse> Update(UpdateTeamInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.TeamInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                model.Name = request.Name;
                model.Groups = string.Join(",", request.Groups);
                model.Domains = string.Join(",", request.Domains.Where(m => !string.IsNullOrEmpty(m)));
                model.Logo = request.Logo;
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
                var model = await _context.TeamInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    _context.TeamInfo.Remove(model);
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
        public async Task<JsonResponse<TeamInfoEntity>> GetById(long id)
        {
            var response = new JsonResponse<TeamInfoEntity>();
            try
            {
                var model = await _context.TeamInfo.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<JsonResponse<List<TeamInfoEntity>>> GetList(GetTeamInfoListRequest request)
        {
            var response = new JsonResponse<List<TeamInfoEntity>>();
            try
            {
                var query =  _context.TeamInfo.AsQueryable();
                if (request.Id.HasValue) 
                {
                    query = query.Where(m => m.Id == request.Id);
                }
                if (!string.IsNullOrEmpty(request.Name)) 
                {
                    query = query.Where(m => m.Name.Contains(request.Name));
                }
                response.data = await query.Skip(request.PageSize * (request.PageIndex-1)).Take(request.PageSize).ToListAsync();
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
        /// 加入团队
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> JoinTeam(JoinTeamRequest request)
        {
            var response = new JsonResponse();
            try
            {
                if(request ==null || request.UserIds == null) 
                {
                    response.code = "-1";
                    response.message = "参数错误!";
                    return response;
                }
                
                var isTeam = await _context.TeamInfo.AnyAsync(m => m.Id == request.TeamId);
                if (!isTeam) 
                {
                    response.code = "-1";
                    response.message = "团队不存在!";
                    return response;
                }
                var list_teamUser = new List<TeamUserMappEntity>();
                foreach (var userId in request.UserIds)
                {
                    var isUser = await _context.UserInfo.AnyAsync(m => m.Id == userId);
                    if (isUser) 
                    {
                        var isTeamUser = await _context.TeamUserMapp.AnyAsync(m => m.TeamId == request.TeamId && m.UserId == userId);
                        if (!isTeamUser) 
                        {
                            var model = new TeamUserMappEntity()
                            {
                                TeamId = request.TeamId,
                                UserId = userId,
                                CreateTime = DateTime.Now
                            };
                            list_teamUser.Add(model);
                        }
                    }
                }
                if(list_teamUser.Count() > 0) 
                {
                    await _context.TeamUserMapp.AddRangeAsync(list_teamUser);
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
        /// 查看团队用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<GetTeamUserListResponse>>> GetTeamUserList(GetTeamUserListRequest request) 
        {
            var response = new JsonResponse<List<GetTeamUserListResponse>>();
            try
            {
                var query = from teamUser in _context.TeamUserMapp
                            join team in _context.TeamInfo on teamUser.TeamId equals team.Id
                            join user in _context.UserInfo on teamUser.UserId equals user.Id
                            select new GetTeamUserListResponse()
                            {
                                Id = teamUser.Id,
                                AvatarUrl = user.AvatarUrl,
                                CreateTime = teamUser.CreateTime,
                                Logo = team.Logo,
                                TeamId = teamUser.TeamId,
                                TeamName = team.Name,
                                UserId = teamUser.UserId,
                                UserName = user.NickName,
                                Domains = team.Domains,
                            };

                if (request.TeamId.HasValue)
                {
                    query = query.Where(m => m.TeamId == request.TeamId);
                }
                if (!string.IsNullOrEmpty(request.UserName))
                {
                    query = query.Where(m => m.UserName.Contains(request.UserName));
                }
                response.data = await query.OrderByDescending(m=>m.CreateTime).Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize).ToListAsync();
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
        /// 查询未加入该团队的全部用户
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<List<UserInfoEntity>>> GetNotJoinUserList(long teamId) 
        {
            var response = new JsonResponse<List<UserInfoEntity>>();
            try
            {
                var teamUser = await _context.TeamUserMapp.Where(m=>m.TeamId == teamId).ToListAsync();
                response.data = await _context.UserInfo.Where(m => !teamUser.Select(m => m.UserId).Contains(m.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 移出团队
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> RemoveTeamUser(RemoveTeamUserRequest request)
        {
            var response = new JsonResponse();
            try
            {
                if (request == null || request.UserIds == null)
                {
                    response.code = "-1";
                    response.message = "参数错误!";
                    return response;
                }
                var isTeam = await _context.TeamInfo.AnyAsync(m => m.Id == request.TeamId);
                if (!isTeam)
                {
                    response.code = "-1";
                    response.message = "团队不存在!";
                    return response;
                }

                var list_teamUser = _context.TeamUserMapp.Where(f => f.TeamId == request.TeamId && request.UserIds.Contains(f.UserId));
                _context.TeamUserMapp.RemoveRange(list_teamUser);
                await _context.SaveChangesAsync();
                return response;

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询用户加入的团队
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse<List<TeamInfoEntity>>> GetUserTeamList(string userId)
        {
            var response = new JsonResponse<List<TeamInfoEntity>>();
            try
            {
                if (string.IsNullOrEmpty(userId)) 
                {
                    response.code = "-1";
                    response.message = "参数错误!";
                    return response;
                }
                var query = from mapp in _context.TeamUserMapp
                            join team in _context.TeamInfo on mapp.TeamId equals team.Id
                            where mapp.UserId == userId
                            select team;
                response.data = await query.OrderByDescending(m=>m.CreateTime).ToListAsync();
                response.total = response.data.Count;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }
    }
}
