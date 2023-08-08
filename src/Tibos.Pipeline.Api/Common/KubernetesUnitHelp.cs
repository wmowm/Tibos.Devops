namespace Tibos.Pipeline.Api.Common
{
    public class KubernetesUnitHelp
    {
        public static (decimal,string,string) UsageUnit(int type, string value)
        {
            switch (type)
            {
                case 1://cpu
                    {
                        if (value.Substring(value.Length - 1) == "n")
                        {
                            var temp = value.Substring(0, value.Length - 1);
                            var res = Convert.ToInt64(temp) / (1000 * 1000m);
                            return (res,"m",value);
                        }
                        else
                        {
                            return (0,"",value);
                        }
                    }
                case 2: //memory
                    {
                        if (value.ToLower().Substring(value.Length - 2) == "ki")
                        {
                            var temp = value.Substring(0, value.Length - 2);
                            var res = Convert.ToInt64(temp) / 1024m;
                            return (res , "Mi",value);
                        }
                        else
                        {
                            return (0, "", value);
                        }
                    }
                default:
                    {
                        return (0, "", value);
                    }
            }
        }
    }
}
