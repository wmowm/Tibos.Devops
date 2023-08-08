using System.ComponentModel;

namespace Tibos.Pipeline.Api.Model.Enum
{
    public enum RoleType
    {
        [Description("管理员")]
        Admin = 0,
        [Description("开发者")]
        Developer = 1,
        [Description("经理")]
        Manager = 2,
        [Description("测试")]
        Test = 3,
        [Description("其它")]
        Other = 4,
    }
}
