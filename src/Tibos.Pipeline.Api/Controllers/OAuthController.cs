using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class OAuthController : Controller
    {
        private readonly IGitlabService _gitlabService;
        private readonly IUserInfoService _userInfoService;

        public OAuthController(HttpClientHelper httpClient, IGitlabService gitlabService, IUserInfoService userInfoService) 
        {
            _gitlabService = gitlabService;
            _userInfoService = userInfoService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<string>), 200)]
        public async Task<IActionResult> GetAuthorize() 
        {
            var res = await _gitlabService.Authorize();
            return Ok(res);
        }

        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<OAuthTokenResponse>), 200)]
        public async Task<IActionResult> GitlabLogin([FromQuery] string code)
        {
            var res = await _userInfoService.GitlabLogin(code);
            return Ok(res);
        }
    }
}
