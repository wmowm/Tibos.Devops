using k8s.Models;
using Tibos.Pipeline.Api.Domain.Service;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IKubenetesService
    {
        Task<V1NamespaceList> ListNamespaceAsync();

        Task<V1Namespace> GetNamespaceAsync(string ns);

        Task<V1Namespace> CreateNamespaceAsync(string ns);

        Task<V1Status> DeleteNamespaceAsync(string ns);



        Task<V1DeploymentList> ListNamespacedDeploymentAsync(string ns);
        Task<V1Deployment> GetNamespacedDeploymentAsync(string ns, string name);

        Task<V1Deployment> CreateNamespacedDeploymentAsync(string ns, V1Deployment deployment);

        Task<V1Deployment> ReplaceNamespacedDeploymentAsync(string ns, string name, int? replicas, string image = "");


        Task<V1Deployment> ReplaceNamespacedDeploymentAsync(string ns, string name, List<V1Volume> volumes, List<V1VolumeMount> volumeMounts);

        Task<V1Status> DeleteNamespacedDeploymentAsync(string ns, string name);

        V1Deployment CreateV1DeploymentFactory(CreateV1DeploymentFactoryRequest request);

        Task<V1ServiceList> ListNamespacedServiceAsync(string ns);

        Task<V1Service> GetNamespacedServiceAsync(string ns, string name);

        Task<V1Service> CreateNamespacedServiceAsync(string ns, string name);

        Task<V1Service> ReplaceNamespacedServiceAsync(string ns, string name, V1ServiceSpec spec);

        Task<V1Service> DeleteNamespacedServiceAsync(string ns, string name);

        Task<V1ConfigMapList> ListNamespacedConfigMapAsync(string ns);

        Task<V1ConfigMap> GetNamespacedConfigMapAsync(string ns, string name);

        Task<V1ConfigMap> CreateNamespacedConfigMapAsync(string ns, string name, Dictionary<string, string> data);

        Task<V1ConfigMap> ReplaceNamespacedConfigMapAsync(string ns, string name, Dictionary<string, string> data);

        Task<V1Status> DeleteNamespacedConfigMapAsync(string ns, string name);

        Task<V1SecretList> ListNamespacedSecretAsync(string ns);

        Task<V1Secret> GetNamespacedSecretAsync(string ns, string name);

        Task<V1Secret> CreateNamespacedSecretAsync(string ns, string name, Dictionary<string, string> stringData,string type);

        Task<V1Secret> ReplaceNamespacedSecretAsync(string ns, string name, Dictionary<string, string> stringData);

        Task<V1Status> DeleteNamespacedSecretAsync(string ns, string name);

        Task<V1PodList> ListNamespacedPodAsync(string ns, string name);

        Task<List<string>> ReadNamespacedPodLogAsync(string ns, string podName, int tailLines = 100000);

        Task DeleteNamespacedPodAsync(string ns, string name);

        Task<PodMetricsList> GetKubernetesPodsMetricsByNamespaceAsync(string ns, string name);

        Task<V1IngressList> ListNamespacedIngressAsync(string ns);

        Task<V1Ingress> GetNamespacedIngressAsync(string ns, string name);

        Task<V1Ingress> CreateNamespacedIngressAsync(string ns, string name, string host);

        Task<V1Ingress> ReplaceNamespacedIngressAsync(string ns, string name, V1IngressSpec spec);

        Task<V1Status> DeleteNamespacedIngressAsync(string ns, string name);

        Task<NodeMetricsList> GetKubernetesNodesMetricsAsync();

        Task<V1NodeList> ListNodeAsync();

    }
}
