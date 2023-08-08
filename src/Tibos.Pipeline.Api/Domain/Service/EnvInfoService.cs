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
    public class EnvInfoService: UserInfoExtensions,IEnvInfoService
    {
        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;

        public EnvInfoService(PipelineDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> Create(CreateEnvInfoRequest request) 
        {
            var response = new JsonResponse();
            try
            {
                var model = _mapper.Map<EnvInfoEntity>(request);
                await _context.EnvInfo.AddAsync(model);
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
        public async Task<JsonResponse> Update(UpdateEnvInfoRequest request)
        {
            var response = new JsonResponse();
            try
            {
              
                var model = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if(model == null) 
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                model.Name = request.Name;
                model.Remark = request.Remark;
                model.TagType = request.TagType;
                model.Key = request.Key;
                model.DomainSymbol = request.DomainSymbol;
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
                var model = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    _context.EnvInfo.Remove(model);
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
        public async Task<JsonResponse<EnvInfoEntity>> GetById(long id)
        {
            var response = new JsonResponse<EnvInfoEntity>();
            try
            {
                var model = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<JsonResponse<List<EnvInfoEntity>>> GetList(GetEnvInfoListRequest request)
        {
            var response = new JsonResponse<List<EnvInfoEntity>>();
            try
            {
                var query = _context.EnvInfo.AsQueryable();
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
        /// 修改映射配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateMappingConfig(UpdateMappingConfigRequest request)
        {
            var response = new JsonResponse();
            try
            {

                var model = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }

                if(!request.MappingConfig && model.MappingConfig != request.MappingConfig) 
                {
                    model.MappingConfig = request.MappingConfig;
                    _context.EnvInfo.Update(model);
                }
                else 
                {
                    var list = await _context.EnvInfo.ToListAsync();
                    list.ForEach(m => 
                    {
                        if (m.Equals(model)) 
                        {
                            m.MappingConfig = request.MappingConfig;
                        }
                        else 
                        {
                            m.MappingConfig = !request.MappingConfig;
                        }
                    });
                    _context.EnvInfo.UpdateRange(list);
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
        /// 修改审核发布
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResponse> UpdateCheckPublish(UpdateCheckPublishRequest request)
        {
            var response = new JsonResponse();
            try
            {

                var model = await _context.EnvInfo.FirstOrDefaultAsync(m => m.Id == request.Id);
                if (model == null)
                {
                    response.code = "-1";
                    response.message = "数据不存在!";
                    return response;
                }
                model.CheckPublish = request.CheckPublish;
                await _context.SaveChangesAsync();
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
