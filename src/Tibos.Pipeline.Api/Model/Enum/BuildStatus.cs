using System.ComponentModel;

namespace Tibos.Pipeline.Api.Model.Enum
{
    /// <summary>
    /// 构建状态
    /// </summary>
    public enum BuildStatus
    {

        //created->pending->running->success
        [Description("创建")]
        created = 0,
        [Description("等待")]
        pending =1,
        [Description("执行中")]
        running =2,
        [Description("完成")]
        success =3
    }
}
