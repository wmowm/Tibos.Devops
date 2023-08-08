namespace Tibos.Pipeline.Api.Model.Response
{
    public class JsonResponse<T> : BaseResponse
    {
        public T data { get; set; }//数据
    }


    public class JsonResponse : BaseResponse
    {
        public object data { get; set; }//数据
    }


    public class BaseResponse
    {
        public string message { get; set; }//消息


        public string code { get; set; } = "0"; //状态码

        public int total { get; set; }//总条数

        public string returnUrl { get; set; } //跳转地址
    }
}
