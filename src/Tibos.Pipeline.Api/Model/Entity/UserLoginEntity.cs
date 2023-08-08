using System.ComponentModel.DataAnnotations.Schema;

namespace Tibos.Pipeline.Api.Model.Entity
{
    /// <summary>
    /// 用户登录模型
    /// </summary>
    [Table("user_login")]
    public class UserLoginEntity
    {
        /// <summary>
        /// 主键,自增
        /// </summary>
        public long Id { get; set; }

      
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 登录类型(0:账号密码,1:gitlab)
        /// </summary>
        public int LoginType { get; set; }


        /// <summary>
        /// 账号
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public  string? Pwd { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

    }
}
