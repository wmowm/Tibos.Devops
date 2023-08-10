using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public class TemplateInfoService: UserInfoExtensions, ITemplateInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;

        public TemplateInfoService(PipelineDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateTemplateInfoDto request) 
        {
            var response = new JsonResponse();
            try
            {
                var model = _mapper.Map<TemplateInfoEntity>(request);
                model.CreateTime = DateTime.Now;
                model.CreateUserId = UserId;
                await _context.TemplateInfo.AddAsync(model);
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
        public async Task<JsonResponse> Update(UpdateTemplateInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
                var model = await _context.TemplateInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                model.Name = request.Name;
                model.Remark = request.Remark;
                model.TempVal = request.TempVal;
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
                var model = await _context.TemplateInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    _context.TemplateInfo.Remove(model);
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
        public async Task<JsonResponse<TemplateInfoEntity>> GetById(long id)
        {
            var response = new JsonResponse<TemplateInfoEntity>();
            try
            {
               
                var model = await _context.TemplateInfo.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<JsonResponse<List<GetTemplateInfoListResponse>>> GetList(GetTemplateInfoListRequest request)
        {
            var response = new JsonResponse<List<GetTemplateInfoListResponse>>();
            try
            {
                var query = from template in _context.TemplateInfo
                            join user in _context.UserInfo on template.CreateUserId equals user.Id into temp
                            from t in temp.DefaultIfEmpty()
                            select new GetTemplateInfoListResponse()
                            {
                                Id = template.Id,
                                CreateTime = template.CreateTime,
                                Name = template.Name,
                                Remark = template.Remark,
                                CreateUserId = template.CreateUserId,
                                Path = template.Path,
                                CreateUserName = t.NickName,
                                TempVal = template.TempVal,
                            };
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
    }
}
