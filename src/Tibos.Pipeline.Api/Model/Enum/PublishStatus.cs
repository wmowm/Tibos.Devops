using System.ComponentModel;

namespace Tibos.Pipeline.Api.Model.Enum
{
    /// <summary>
    /// 发布状态(-1:发布失败,0:待审核,1:审核通过,2:发布中,3:发布完成)
    /// </summary>
    public enum PublishStatus
    {
        [Description("发布失败")]
        Error = 0,
        [Description("待审核")]
        Audit = 0,
        [Description("审核通过")]
        Approved = 1,
        [Description("发布中")]
        InProgress = 2,
        [Description("发布完成")]
        Success =3
    }
}
