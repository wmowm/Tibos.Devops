using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Domain.Service;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    /// <summary>
    /// 模板管理
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TemplateInfoController : ControllerBase
    {
        private readonly ITemplateInfoService _templateInfoService;
        private readonly IMapper _mapper;


        public TemplateInfoController(ITemplateInfoService templateInfoService, IMapper mapper)
        {
            _templateInfoService = templateInfoService;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create(IFormCollection filesForm, [FromForm] CreateTemplateInfoRequest request)
        {

            JsonResponse response = new JsonResponse();
            try
            {
                var dto = _mapper.Map<CreateTemplateInfoDto>(request);
                if (filesForm.Files.Count == 0)
                {
                    response.code = "-1";
                    response.message = "请选择要上传的文件";
                    return Ok(response);
                }
                if (filesForm.Files.Count != 1)
                {
                    response.code = "-1";
                    response.message = "一次只能上传一个文件";
                    return Ok(response);
                }
                var fullName = filesForm.Files[0].FileName;

                var strLog = string.Empty;
                var suffix = fullName.Substring(fullName.LastIndexOf(".") + 1, (fullName.Length - fullName.LastIndexOf(".") - 1)); //扩展名
                if (suffix != "zip")
                {
                    response.code = "-1";
                    response.message = "只能上传zip类型文件";
                    return Ok(response);
                }
                var tempName = fullName.Substring(0, fullName.IndexOf("."));
                bool bl = Regex.IsMatch(tempName, @"^[a-zA-Z0-9]*$");
                if (!bl)
                {
                    response.code = "-1";
                    response.message = "zip文件名称只能由数字字母组成";
                    return Ok(response);
                }

                var filePath = Directory.GetCurrentDirectory() + $"/Template/{fullName}";
                if (System.IO.File.Exists(filePath))
                {
                    response.code = "-1";
                    response.message = "该模板路径已存在";
                    return Ok(response);
                }
                using (var stream = System.IO.File.Create(filePath))
                {
                    await Request.Form.Files[0].CopyToAsync(stream);
                }
                dto.Path = $"/Template/{fullName}";
                response = await _templateInfoService.Create(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误1";
                return Ok(response);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<TemplateInfoEntity>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _templateInfoService.GetById(id);
            return Ok(res);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Update(IFormCollection filesForm, [FromForm] UpdateTemplateInfoRequest request)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                var dto = _mapper.Map<CreateTemplateInfoDto>(request);
                if (filesForm.Files.Count != 0)
                {
                    if (filesForm.Files.Count != 1)
                    {
                        response.code = "-1";
                        response.message = "一次只能上传一个文件";
                        return Ok(response);
                    }
                    var fullName = filesForm.Files[0].FileName;

                    var strLog = string.Empty;
                    var suffix = fullName.Substring(fullName.LastIndexOf(".") + 1, (fullName.Length - fullName.LastIndexOf(".") - 1)); //扩展名
                    if (suffix != "zip")
                    {
                        response.code = "-1";
                        response.message = "只能上传zip类型文件";
                        return Ok(response);
                    }
                    var tempName = fullName.Substring(0, fullName.IndexOf("."));
                    bool bl = Regex.IsMatch(tempName, @"^[a-zA-Z0-9]*$");
                    if (!bl)
                    {
                        response.code = "-1";
                        response.message = "zip文件名称只能由数字字母组成";
                        return Ok(response);
                    }
                    var res = await _templateInfoService.GetById(request.Id);
                    if (res.code != "0")
                    {
                        response.code = "-1";
                        response.message = "数据不存在";
                        return Ok(response);
                    }
                    if (res.data.Path != $"/Template/{fullName}")
                    {
                        response.code = "-1";
                        response.message = "模板的路径发生变化,无法修改";
                        return Ok(response);
                    }
                    var filePath = Directory.GetCurrentDirectory() + $"/Template/{fullName}";
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await Request.Form.Files[0].CopyToAsync(stream);
                    }
                }
                response = await _templateInfoService.Update(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = "系统错误1";
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            JsonResponse response = new JsonResponse();
            var res = await _templateInfoService.GetById(id);
            if (res.code != "0")
            {
                response.code = "-1";
                response.message = "数据不存在";
                return Ok(response);
            }
            var filePath = Directory.GetCurrentDirectory() + res.data.Path;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            response = await _templateInfoService.Delete(id);
            return Ok(res);
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetTemplateInfoListResponse>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetTemplateInfoListRequest request)
        {
            var res = await _templateInfoService.GetList(request);
            return Ok(res);
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Download([FromQuery] long id)
        {
            JsonResponse response = new JsonResponse();
            var res = await _templateInfoService.GetById(id);
            if (res.code != "0")
            {
                response.code = "-1";
                response.message = "数据不存在";
                return Ok(response);
            }
            var filePath = Directory.GetCurrentDirectory() + res.data.Path;
            var stream = System.IO.File.OpenRead(filePath);
            string suffix = Path.GetExtension(filePath);
            var name = Path.GetFileName(filePath);
            var provider = new FileExtensionContentTypeProvider();
            var contentType = provider.Mappings[suffix];

            return File(stream, contentType, name);
        }
    }
}