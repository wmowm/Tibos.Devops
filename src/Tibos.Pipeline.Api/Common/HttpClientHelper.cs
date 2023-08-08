using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tibos.Pipeline.Api.Common
{
    public class HttpClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = new TimeSpan(0, 0, 20);
            return client;
        }

        public async Task<string> PostDataWithToken(string url, string data, string token, string contentType = "application/json", Encoding encode = null)
        {
            try
            {
                var client = CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                encode = encode == null ? Encoding.UTF8 : encode;
                var response = await client.PostAsync(url, new StringContent(data, encode, contentType));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<string> PostDataWithCert(string url, string data, string certPath, string password, string contentType = "application/json", Encoding encode = null)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                    ServerCertificateCustomValidationCallback = (x, y, z, m) => true
                };
                handler.ClientCertificates.Add(new X509Certificate2(certPath, password));
                using (var client = new HttpClient(handler))
                {
                    encode = encode == null ? Encoding.UTF8 : encode;
                    var response = await client.PostAsync(url, new StringContent(data, encode, contentType));
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PostData(string url, string data, string contentType = "application/json", Encoding encode = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var client = CreateClient();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        if (client.DefaultRequestHeaders.Contains(item.Key))
                        {
                            client.DefaultRequestHeaders.Remove(item.Key);
                        }
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                encode = encode == null ? Encoding.UTF8 : encode;
                var response = await client.PostAsync(url, new StringContent(data, encode, contentType));
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PostData(string url, string data,Stream stream, string contentType = "application/json", Encoding encode = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var client = CreateClient();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        if (client.DefaultRequestHeaders.Contains(item.Key))
                        {
                            client.DefaultRequestHeaders.Remove(item.Key);
                        }
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                encode = encode == null ? Encoding.UTF8 : encode;
                var response = await client.PostAsync(url, new StringContent(data, encode, contentType));
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetDataWithToken(string url, string token)
        {
            try
            {
                var client = CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetData(string url)
        {
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetData(string url, Dictionary<string, string> headers)
        {
            try
            {
                var client = CreateClient();
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<byte[]> GetDataFile(string url, Dictionary<string, string> headers)
        {
            try
            {
                var client = CreateClient();
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }


        /// <summary>
        /// form-data
        /// </summary>
        /// <param name="token"></param>
        /// <param name="reauestUrl"></param>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public async  Task<string> PostFrom(string token, string reauestUrl, byte[] bytes, string fileName, Dictionary<string, string> dic)
        {
            try
            {
                var httpClient = CreateClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"api:{token}");
                ByteArrayContent fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = fileName };
                using (var formData = new MultipartFormDataContent()) 
                {
                    formData.Add(fileContent);
                    foreach (var item in dic)
                    {
                        formData.Add(new StringContent(item.Value), item.Key);
                    }
                    var result = await httpClient.PostAsync(reauestUrl, formData);
                    Console.WriteLine(JsonConvert.SerializeObject(result));
                    return await result.Content.ReadAsStringAsync();
                } 
               
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 字典转url参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string DictionaryToStr(IDictionary<string, string> dic)
        {
            //使用排序字典
            dic = new SortedDictionary<string, string>(dic);
            string strTemp = "?";
            foreach (KeyValuePair<string, string> item in dic)
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    strTemp += item.Key + "=" + item.Value + "&";
                }
            }
            strTemp = strTemp.TrimEnd('&');
            return strTemp;

        }
    }
}
