namespace Tibos.Pipeline.Api.Model.Request
{
    public class BaseRequest
    {
        private int _pageIndex;
        private int _pageSize;
        public int PageIndex {
            get
            {
                return _pageIndex < 1 ? 1 : _pageIndex;
            }
            set 
            {
                _pageIndex = value;
            } 
        }

        public int PageSize 
        {
            get 
            {
                return _pageSize < 1 ? 1 : _pageSize;
            }
            set { _pageSize = value; }
        }
    }
}
