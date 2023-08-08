using System.ComponentModel;

namespace Tibos.Pipeline.Api.Common
{
    public class EnumberHelper
    {
        public static List<EnumberEntity> EnumToList<T>()
        {
            List<EnumberEntity> list = new List<EnumberEntity>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumberEntity m = new EnumberEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }
    }

    public class EnumberEntity
    {
        /// <summary>
        /// 枚举的描述
        /// </summary>
        public string Desction { set; get; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string EnumName { set; get; }

        /// <summary>
        /// 枚举对象的值
        /// </summary>
        public int EnumValue { set; get; }
    }
}
