using k8s;
using k8s.Models;
using System.Xml.Linq;
using Tibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class KubenetesService : IKubenetesService
    {
        private readonly Kubernetes _client;
        public KubenetesService(Kubernetes client)
        {
            _client = client;
        }


        #region NameSpace
        /// <summary>
        /// 查询全部名称空间
        /// </summary>
        /// <returns></returns>
        public async Task<V1NamespaceList> ListNamespaceAsync()
        {
            var namespaces = await _client.ListNamespaceAsync();
            return namespaces;
        }

        /// <summary>
        /// 查询指定名称空间
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Namespace> GetNamespaceAsync(string ns)
        {
            var namespaces = await _client.ListNamespaceAsync();
            return namespaces.Items.FirstOrDefault(m => m.Metadata.Name == ns);
        }

        /// <summary>
        /// 创建名称空间
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Namespace> CreateNamespaceAsync(string ns)
        {
            var model = await GetNamespaceAsync(ns);
            if (model != null) return model;
            model = await _client.CreateNamespaceAsync(new V1Namespace()
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = ns,
                }
            });
            return model;
        }

        /// <summary>
        /// 删除名称空间
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Status> DeleteNamespaceAsync(string ns)
        {
            var model = await GetNamespaceAsync(ns);
            if (model != null)
            {
                var res = await _client.DeleteNamespaceAsync(ns);
            }
            return null;
        }

        #endregion

        #region Deployments
        /// <summary>
        /// 查询全部部署
        /// </summary>
        /// <returns></returns>
        public async Task<V1DeploymentList> ListNamespacedDeploymentAsync(string ns)
        {
            var list = await _client.ListNamespacedDeploymentAsync(ns);
            return list;
        }

        /// <summary>
        /// 查询指定部署
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Deployment> GetNamespacedDeploymentAsync(string ns, string name)
        {
            var list = await ListNamespacedDeploymentAsync(ns);
            return list.Items.FirstOrDefault(m => m.Metadata.Name == name);
        }

        /// <summary>
        /// 创建部署
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Deployment> CreateNamespacedDeploymentAsync(string ns, V1Deployment deployment)
        {
            var model = await GetNamespacedDeploymentAsync(ns, deployment.Metadata.Name);
            if (model == null)
            {
                model = await _client.CreateNamespacedDeploymentAsync(deployment, ns);
            }
            return model;
        }

        /// <summary>
        /// 修改部署(伸缩容器,或修改镜像)
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="replicas"></param>
        /// <returns></returns>
        public async Task<V1Deployment> ReplaceNamespacedDeploymentAsync(string ns, string name,int? replicas,string image = "")
        {
            var model = await GetNamespacedDeploymentAsync(ns, name);
            if (model != null)
            {
                if (replicas.HasValue)
                {
                    model.Spec.Replicas = replicas;
                }
                if (!string.IsNullOrEmpty(image))
                {
                    model.Spec.Template.Spec.Containers.FirstOrDefault().Image = image;
                }
                model = await _client.ReplaceNamespacedDeploymentAsync(model, name, ns);
            }
            return model;
        }


        /// <summary>
        /// 修改部署(挂载配置)
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="replicas"></param>
        /// <returns></returns>
        public async Task<V1Deployment> ReplaceNamespacedDeploymentAsync(string ns, string name, List<V1Volume> volumes, List<V1VolumeMount> volumeMounts)
        {
            var model = await GetNamespacedDeploymentAsync(ns, name);
            if (model != null)
            {
                model.Spec.Template.Spec.Volumes = volumes;
                model.Spec.Template.Spec.Containers.FirstOrDefault().VolumeMounts = volumeMounts;
                model = await _client.ReplaceNamespacedDeploymentAsync(model, name, ns);
            }
            return model;
        }

        /// <summary>
        /// 删除部署
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Status> DeleteNamespacedDeploymentAsync(string ns, string name)
        {
            var model = await GetNamespacedDeploymentAsync(ns, name);
            if (model != null)
            {

                return await _client.DeleteNamespacedDeploymentAsync(name, ns);
            }
            return null;
        }


        /// <summary>
        /// 工厂模式,创建Deployment配置模型
        /// </summary>
        /// <returns></returns>
        public V1Deployment CreateV1DeploymentFactory(CreateV1DeploymentFactoryRequest request)
        {
            var deployment = new k8s.Models.V1Deployment()
            {
                ApiVersion = "apps/v1",
                Kind = "Deployment",
                Metadata = new k8s.Models.V1ObjectMeta()
                {
                    Name = request.Name,
                    NamespaceProperty = request.Ns,
                },
                Spec = new k8s.Models.V1DeploymentSpec()
                {
                    Replicas = request.Replicas,
                    Selector = new k8s.Models.V1LabelSelector()
                    {
                        MatchLabels = new Dictionary<string, string>()
                        {
                                { "app",request.Name}
                            }
                    },
                    Template = new k8s.Models.V1PodTemplateSpec()
                    {
                        Metadata = new k8s.Models.V1ObjectMeta()
                        {
                            Labels = new Dictionary<string, string>()
                            {
                                    { "app",request.Name}
                                }
                        },
                        Spec = new k8s.Models.V1PodSpec()
                        {
                            ImagePullSecrets = new List<V1LocalObjectReference>()
                                {
                                    new V1LocalObjectReference()
                                    {
                                        Name= request.ImagePullSecretsName,
                                    }
                                },
                            Containers = new List<V1Container>()
                                {
                                    new V1Container()
                                    {
                                        Name=request.Name,
                                        Image = request.Image,
                                        ImagePullPolicy = "IfNotPresent",
                                        Ports = new List<V1ContainerPort>()
                                        {
                                            new V1ContainerPort()
                                            {
                                                ContainerPort=80
                                            }
                                        },
                                    }
                                },
                        }
                    },

                },
                Status = new k8s.Models.V1DeploymentStatus()
                {

                }
            };
            return deployment;
        }


        #endregion

        #region Services
        /// <summary>
        /// 查询全部服务
        /// </summary>
        /// <returns></returns>
        public async Task<V1ServiceList> ListNamespacedServiceAsync(string ns)
        {
            var list = await _client.ListNamespacedServiceAsync(ns);
            return list;
        }

        /// <summary>
        /// 查询指定服务
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Service> GetNamespacedServiceAsync(string ns, string name)
        {
            var list = await _client.ListNamespacedServiceAsync(ns);
            return list.Items.FirstOrDefault(m => m.Metadata.Name == name);
        }

        /// <summary>
        /// 创建服务
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Service> CreateNamespacedServiceAsync(string ns, string name)
        {
            var model = await GetNamespacedServiceAsync(ns, name);
            if (model == null)
            {
                var service = new V1Service()
                {
                    ApiVersion = "v1",
                    Kind = "Service",
                    Metadata = new V1ObjectMeta()
                    {
                        Name = name,
                        NamespaceProperty = ns,
                    },
                    Spec = new V1ServiceSpec()
                    {
                        Ports = new List<V1ServicePort>()
                        {
                            new V1ServicePort()
                            {
                                Name = "http",
                                Protocol = "TCP",
                                Port =80,
                                TargetPort=80
                            }
                        },
                        Selector = new Dictionary<string, string>()
                        {
                            { "app",name}
                        },
                        Type = "ClusterIP",
                        InternalTrafficPolicy = "Cluster"
                    }
                };

                model = await _client.CreateNamespacedServiceAsync(service, ns);
            }
            return model;
        }

        /// <summary>
        /// 修改服务
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public async Task<V1Service> ReplaceNamespacedServiceAsync(string ns, string name, V1ServiceSpec spec)
        {
            var model = await GetNamespacedServiceAsync(ns, name);
            if (model != null)
            {
                model.Spec = spec;
                model = await _client.ReplaceNamespacedServiceAsync(model, name, ns);
            }
            return model;
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Service> DeleteNamespacedServiceAsync(string ns, string name)
        {
            var model = await GetNamespacedServiceAsync(ns, name);
            if (model != null)
            {
                return await _client.DeleteNamespacedServiceAsync(name, ns);
            }
            return null;
        }

        #endregion

        #region ConfigMaps
        /// <summary>
        /// 查询全部配置
        /// </summary>
        /// <returns></returns>
        public async Task<V1ConfigMapList> ListNamespacedConfigMapAsync(string ns)
        {
            var list = await _client.ListNamespacedConfigMapAsync(ns);
            return list;
        }

        /// <summary>
        /// 查询指定配置
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1ConfigMap> GetNamespacedConfigMapAsync(string ns, string name)
        {
            var list = await _client.ListNamespacedConfigMapAsync(ns);
            return list.Items.FirstOrDefault(m => m.Metadata.Name == name);
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1ConfigMap> CreateNamespacedConfigMapAsync(string ns, string name, Dictionary<string, string> data)
        {
            var model = await GetNamespacedConfigMapAsync(ns, name);
            if (model == null)
            {
                var ConfigMap = new V1ConfigMap()
                {
                    ApiVersion = "v1",
                    Kind = "ConfigMap",
                    Metadata = new V1ObjectMeta()
                    {
                        Name = name,
                        NamespaceProperty = ns,
                    },
                    Data = data,
                };

                model = await _client.CreateNamespacedConfigMapAsync(ConfigMap, ns);
            }
            return model;
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public async Task<V1ConfigMap> ReplaceNamespacedConfigMapAsync(string ns, string name, Dictionary<string, string> data)
        {
            var model = await GetNamespacedConfigMapAsync(ns, name);
            if (model != null)
            {
                model.Data = data;
                model = await _client.ReplaceNamespacedConfigMapAsync(model, name, ns);
            }
            return model;
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Status> DeleteNamespacedConfigMapAsync(string ns, string name)
        {
            var model = await GetNamespacedConfigMapAsync(ns, name);
            if (model != null)
            {
                return await _client.DeleteNamespacedConfigMapAsync(name, ns);
            }
            return null;
        }
        #endregion

        #region Secrets
        /// <summary>
        /// 查询全部机密
        /// </summary>
        /// <returns></returns>
        public async Task<V1SecretList> ListNamespacedSecretAsync(string ns)
        {
            var list = await _client.ListNamespacedSecretAsync(ns);
            return list;
        }

        /// <summary>
        /// 查询指定机密
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Secret> GetNamespacedSecretAsync(string ns, string name)
        {
            var list = await _client.ListNamespacedSecretAsync(ns);
            return list.Items.FirstOrDefault(m => m.Metadata.Name == name);
        }

        /// <summary>
        /// 创建机密
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="stringData">value为base64的字符串</param>
        /// <returns></returns>
        public async Task<V1Secret> CreateNamespacedSecretAsync(string ns, string name, Dictionary<string, string> stringData,string type)
        {
            var model = await GetNamespacedSecretAsync(ns, name);
            if (model == null)
            {
                var Secret = new V1Secret()
                {
                    ApiVersion = "v1",
                    Kind = "Secret",
                    Metadata = new V1ObjectMeta()
                    {
                        Name = name,
                        NamespaceProperty = ns,
                    },
                    StringData = stringData,
                    Type = type
                };
                model = await _client.CreateNamespacedSecretAsync(Secret, ns);
            }
            return model;
        }

        /// <summary>
        /// 修改机密
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="stringData">value为base64的字符串</param>
        /// <returns></returns>
        public async Task<V1Secret> ReplaceNamespacedSecretAsync(string ns, string name, Dictionary<string, string> stringData)
        {
            var model = await GetNamespacedSecretAsync(ns, name);
            if (model != null)
            {
                model.StringData = stringData;
                model = await _client.ReplaceNamespacedSecretAsync(model, name, ns);
            }
            return model;
        }

        /// <summary>
        /// 删除机密
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Status> DeleteNamespacedSecretAsync(string ns, string name)
        {
            var model = await GetNamespacedSecretAsync(ns, name);
            if (model != null)
            {
                return await _client.DeleteNamespacedSecretAsync(name, ns);
            }
            return null;
        }
        #endregion

        #region Pods

        /// <summary>
        /// 查询全部Pod
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1PodList> ListNamespacedPodAsync(string ns, string name)
        {
            var list = await _client.ListNamespacedPodAsync(ns);
            if (string.IsNullOrEmpty(name)) return list;
            list.Items = list.Items.Where(m => m.Metadata.Labels.FirstOrDefault(m => m.Key == "app").Value == name).ToList();
            return list;
        }

        /// <summary>
        /// 查询指定pod日志
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="podName"></param>
        /// <returns></returns>
        public async Task<List<string>> ReadNamespacedPodLogAsync(string ns, string podName, int tailLines = 100000)
        {
            var stream = await _client.ReadNamespacedPodLogAsync(podName, ns, tailLines: tailLines);
            var res = new List<string>();
            using (StreamReader sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    res.Add( sr.ReadLine());
                }
            }
            return res;
        }

        /// <summary>
        /// 查询pod资源占用
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<PodMetricsList> GetKubernetesPodsMetricsByNamespaceAsync(string ns, string name)
        {
            var list = await _client.GetKubernetesPodsMetricsByNamespaceAsync(ns);
            if (string.IsNullOrEmpty(name)) return list;
            list.Items = list.Items.Where(m => m.Metadata.Labels.FirstOrDefault(m => m.Key == "app").Value == name).ToList();
            return list;
        }

        /// <summary>
        /// 删除pod
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteNamespacedPodAsync(string ns, string name)
        {
            var list = await ListNamespacedPodAsync(ns,name);
            foreach (var item in list) 
            {
                await _client.DeleteNamespacedPodAsync(item.Metadata.Name, ns);
            }      
        }

        #endregion

        #region Ingresses

        /// <summary>
        /// 查询全部网关
        /// </summary>
        /// <returns></returns>
        public async Task<V1IngressList> ListNamespacedIngressAsync(string ns)
        {
            var list = await _client.ListNamespacedIngressAsync(ns);
            return list;
        }

        /// <summary>
        /// 查询指定网关
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<V1Ingress> GetNamespacedIngressAsync(string ns, string name)
        {
            var list = await _client.ListNamespacedIngressAsync(ns);
            return list.Items.FirstOrDefault(m => m.Metadata.Name == name);
        }

        /// <summary>
        /// 创建网关
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public async Task<V1Ingress> CreateNamespacedIngressAsync(string ns, string name, string host)
        {
            var model = await GetNamespacedIngressAsync(ns, name);
            if (model == null)
            {
                var Ingress = new V1Ingress()
                {
                    ApiVersion = "networking.k8s.io/v1",
                    Kind = "Ingress",
                    Metadata = new V1ObjectMeta()
                    {
                        Name = name,
                        NamespaceProperty = ns,
                        Annotations = new Dictionary<string, string>()
                        {
                            { "kubernetes.io/ingress.class","nginx"}
                        }
                    },
                    Spec = new V1IngressSpec()
                    {
                        Rules = new List<V1IngressRule>()
                        {
                            new V1IngressRule()
                            {
                                Host=host,
                                Http=new V1HTTPIngressRuleValue()
                                {
                                    Paths = new List<V1HTTPIngressPath>()
                                    {
                                        new V1HTTPIngressPath()
                                        {
                                            Path = "/",
                                            PathType = "Prefix",
                                            Backend = new V1IngressBackend()
                                            {
                                                Service = new V1IngressServiceBackend()
                                                {
                                                    Name=name,
                                                    Port = new V1ServiceBackendPort()
                                                    {
                                                        Number = 80,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                model = await _client.CreateNamespacedIngressAsync(Ingress, ns);
            }
            return model;
        }

        /// <summary>
        /// 修改网关
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public async Task<V1Ingress> ReplaceNamespacedIngressAsync(string ns, string name, V1IngressSpec spec)
        {
            var model = await GetNamespacedIngressAsync(ns, name);
            if (model != null)
            {
                model.Spec = spec;
                model = await _client.ReplaceNamespacedIngressAsync(model, name, ns);
            }
            return model;
        }

        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<V1Status> DeleteNamespacedIngressAsync(string ns, string name)
        {
            var model = await GetNamespacedIngressAsync(ns, name);
            if (model != null)
            {
                return await _client.DeleteNamespacedIngressAsync(name, ns);
            }
            return null;
        }
        #endregion

        #region node

        public async Task<NodeMetricsList> GetKubernetesNodesMetricsAsync()
        {
            var list = await _client.GetKubernetesNodesMetricsAsync();
            return list;
        }


        public async Task<V1NodeList> ListNodeAsync()
        {

            var list = await _client.ListNodeAsync();
            return list;
        }

        #endregion
    }

    public class CreateV1DeploymentFactoryRequest 
    {
        /// <summary>
        /// 名称空间
        /// </summary>
        public string Ns { get; set; }

        /// <summary>
        /// 部署名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// docker仓库密钥
        /// </summary>
        public string ImagePullSecretsName { get; set; } = "docker-reg-secret";

        /// <summary>
        /// 镜像名称
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// pod数量
        /// </summary>
        public int Replicas { get; set; } = 1;

    }
}
