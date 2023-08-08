using k8s.KubeConfigModels;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Enum;
using TTibos.Pipeline.Api.Domain.IService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tibos.Pipeline.Api.Domain.Work
{
    public class NodeMetricsTask : BackgroundService
    {
        private readonly ILogger<NodeMetricsTask> _logger;
        private readonly PipelineDBContext _context;
        private readonly IKubenetesService _kubenetesService;

        public NodeMetricsTask(ILogger<NodeMetricsTask> logger,
                           PipelineDBContext context,
                           IKubenetesService kubenetesService
                            ) 
        {
            _logger = logger;
            _context = context;
            _kubenetesService = kubenetesService;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var res = await _kubenetesService.GetKubernetesNodesMetricsAsync();
                        foreach (var node in res.Items) 
                        {
                            if (!node.Timestamp.HasValue) continue;

                            var cpuUsage = node.Usage["cpu"]?.Value;
                            var memoryUsage = node.Usage["memory"]?.Value;

                            var cpuUnit = KubernetesUnitHelp.UsageUnit(1, cpuUsage);
                            var memoryUnit = KubernetesUnitHelp.UsageUnit(2, memoryUsage);

                            var model = new NodeMetricsEntity()
                            {
                                CreateTime = node.Timestamp.Value.AddHours(8),
                                Name = node.Metadata.Name,
                                Cpu = cpuUnit.Item1,
                                CpuUnit = cpuUnit.Item2,
                                Memory = memoryUnit.Item1,
                                MemoryUnit = memoryUnit.Item2,
                            };
                            var bl = await _context.NodeMetrics.AnyAsync(m => m.Name == model.Name && m.CreateTime == model.CreateTime);
                            if (!bl) 
                            {
                                await _context.NodeMetrics.AddAsync(model);

                                //只保留近15天的采集数据
                                var query = _context.NodeMetrics.Where(m=>m.CreateTime < DateTime.Now.AddDays(-15));
                                _context.NodeMetrics.RemoveRange(query);
                                await _context.SaveChangesAsync();
                            }
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"执行节点数据采集任务异常 报错记录:{ex.StackTrace},{ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            });
        }
    }
}
