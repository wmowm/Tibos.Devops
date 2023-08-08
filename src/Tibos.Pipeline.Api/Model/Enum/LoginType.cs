using System.ComponentModel;

namespace Tibos.Pipeline.Api.Model.Enum
{
    public enum LoginType
    {
        [Description("账号密码登录")]
        Default = 0,
        [Description("Gitlab授权登录")]
        Gitlab = 1
    }
}
