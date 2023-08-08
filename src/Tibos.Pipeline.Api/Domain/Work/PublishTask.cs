using Microsoft.EntityFrameworkCore;
using Tibos.Pipeline.Api.Model.Enum;
using TTibos.Pipeline.Api.Domain.IService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tibos.Pipeline.Api.Domain.Work
{
    public class PublishTask : BackgroundService
    {
        private readonly ILogger<PublishTask> _logger;
        private readonly IPublishRecordService  _publishRecordService;
        private readonly PipelineDBContext _context;

        public PublishTask(ILogger<PublishTask> logger,
                           IPublishRecordService publishRecordService,
                           PipelineDBContext context
                            ) 
        {
            _logger = logger;
            _publishRecordService = publishRecordService;
            _context = context;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var pageIndex = 1;
                    var pageSize = 10;
                    try
                    {
                        while (true)
                        {
                            var query = _context.PublishRecord.Where(m => m.Status == (int)PublishStatus.Approved);
                            var list = await query.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                            if (list.Count > 0)
                            {
                                pageIndex++;
                                foreach (var item in list)
                                {
                                    await _publishRecordService.PublishApp(new Model.Request.PublishAppRequest()
                                    {
                                        PublistRecordId = item.Id
                                    });
                                }
                            }
                            else
                            {
                                await Task.Delay(TimeSpan.FromSeconds(3));
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"执行发布任务异常 报错记录:{ex.StackTrace},{ex.Message}");
                    }
                }
            });
        }
    }
}
