using System.ComponentModel;

namespace Tibos.Pipeline.Api.Model.Enum
{
    /// <summary>
    /// 构建状态
    /// </summary>
    public enum ConfigActionType
    {

        //操作类型(0:创建,1:修改设置,2:修改配置,3:挂载设置,4:删除操作
        [Description("创建")]
        Create = 0,
        [Description("修改设置")]
        Setting =1,
        [Description("修改配置")]
        EditConfig =2,
        [Description("挂载设置")]
        EditMounts = 3,
        [Description("删除操作")]
        Delete =4
    }
}
