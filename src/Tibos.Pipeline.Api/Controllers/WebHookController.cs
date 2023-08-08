using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Tibos.Pipeline.Api.Model.Request;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WebHookController : ControllerBase
    {
        private readonly ILogger<WebHookController> _logger;
        private readonly IBuildRecordService _buildRecordService;


        public WebHookController(
            ILogger<WebHookController> logger
            , IBuildRecordService buildRecordService
            )
        {
            _logger = logger;
            _buildRecordService = buildRecordService;
        }

        [HttpPost]
        public async Task<JsonResult> GitlabWebHook([FromBody] HookRequest model)
        {
            try
            {
                await _buildRecordService.CreateOrUpdate(model);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"±®¥Ì¡À:{ex.Message}");
            }
            return new JsonResult(200);
        }

    }
}