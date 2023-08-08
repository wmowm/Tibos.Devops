using k8s;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tibos.Pipeline.Api.Model.Config;

namespace Tibos.Pipeline.Api.Common
{
    public static class KubernetesExtensions
    {
        public static void AddKubernetes(this IServiceCollection services, Action<KubernetesOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            var options = new KubernetesOptions();
            setupAction(options);
            var path = $"{AppContext.BaseDirectory}{options.KubePath}";
            var kubernetesConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile(path);
            services.Configure(setupAction);
            services.AddSingleton<Kubernetes>(new Kubernetes(kubernetesConfig));
        }
    }
}
