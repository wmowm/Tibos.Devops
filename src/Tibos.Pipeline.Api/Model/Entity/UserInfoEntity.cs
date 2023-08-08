using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 用户实体
    /// </summary>
    [Table("user_info")]
    public class UserInfoEntity
    {

        /// <summary>
        /// 用户编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string NickName { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public virtual string Group { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 状态(0:禁用,1:启用)
        /// </summary>
        public int Status { get; set; }

    }
}
