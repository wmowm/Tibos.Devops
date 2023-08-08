using System.Globalization;

namespace Tibos.Pipeline.Api.Common
{
    public static class StringDateTimeExtension
    {
        public static DateTime GetFmortDateTime(this string strDate)
        {
            string[] format = { "yyyy-MM-dd HH:mm:ss UTC" };
            DateTime dt;
            if (DateTime.TryParseExact(strDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt.AddHours(8);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
