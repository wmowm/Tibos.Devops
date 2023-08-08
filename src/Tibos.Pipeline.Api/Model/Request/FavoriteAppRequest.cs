namespace Tibos.Pipeline.Api.Model.Request
{
    public class FavoriteAppRequest
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// true 收藏, false 取消
        /// </summary>
        public bool FavoriteType { get; set; }
    }
}
