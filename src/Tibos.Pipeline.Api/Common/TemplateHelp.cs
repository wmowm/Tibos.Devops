using System.IO.Compression;
using System.Xml.Linq;

namespace Tibos.Pipeline.Api.Common
{
    public class TemplateHelp
    {


        /// <summary>
        /// 解压模板
        /// </summary>
        /// <param name="zipFilePath">模板路径</param>
        public static string ExtractToDirectory(string zipFilePath)
        {
            var name = Path.GetFileName(Directory.GetCurrentDirectory() + zipFilePath);
            name = name.Substring(0, name.LastIndexOf("."));
            if (!Directory.Exists(Directory.GetCurrentDirectory() + $"/Template/{name}"))
            {
                if (File.Exists(Directory.GetCurrentDirectory() + zipFilePath))
                {
                    ZipFile.ExtractToDirectory(Directory.GetCurrentDirectory() + zipFilePath, Directory.GetCurrentDirectory() + $"/Template/");
                }
                else
                {
                    Console.WriteLine("模板文件不存在!");
                    return "";
                }
            }
            return Directory.GetCurrentDirectory() + $"/Template/{name}";
        }

        /// <summary>
        /// 根据模板生成项目
        /// </summary>
        /// <param name="tempPath">模板地址</param>
        /// <param name="appPath">项目地址</param>
        /// <param name="tempProjectName">模板名称</param>
        /// <param name="projectName">项目名称</param>
        public static void CreateProjectApp(string tempPath, string appPath,string tempProjectName,string projectName)
        {
            try
            {
                //创建项目目录
                if (!Directory.Exists(Directory.GetCurrentDirectory() + $"/Template/{projectName}")) 
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + $"/Template/{projectName}");
                }
                //特殊变量替换
                var dict = new Dictionary<string, Dictionary<string, string>>();
                //gitlab-ci
                dict.Add(".gitlab-ci.yml", new Dictionary<string, string>() { { "${TEMP_PROJECT_NAME}", projectName.ToLower() } });
                DirectoryInfo dir = new DirectoryInfo(tempPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    //文件名,变量替换
                    var name = i.Name.Replace(tempProjectName, projectName);
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        if (!Directory.Exists($"{appPath}/{name}"))
                        {
                            Directory.CreateDirectory($"{appPath}/{name}"); //目标目录下不存在此文件夹即创建子文件夹
                        }

                        CreateProjectApp(i.FullName, $"{appPath}/{name}",tempProjectName,projectName); //递归调用复制子文件夹
                    }
                    else
                    {
                        //文件内,变量替换
                        using (var sr = new StreamReader(i.FullName))
                        {
                            using (var sw = new StreamWriter($"{appPath}/{name}"))
                            {
                                while (!sr.EndOfStream)
                                {
                                    string strLine = sr.ReadLine();
                                    if (!string.IsNullOrEmpty(strLine))
                                    {
                                        if (dict.Keys.Contains(i.Name))
                                        {
                                            var map = dict[i.Name];
                                            foreach (var key in map.Keys)
                                            {
                                                strLine = strLine.Replace(key, map[key]);
                                            }
                                        }
                                        strLine = strLine.Replace(tempProjectName, projectName);
                                    }
                                    sw.WriteLine(strLine);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="appPath"></param>
        public static void DeleteProjectApp(string appPath) 
        {
            try
            {
                if (Directory.Exists(appPath))
                {
                    Directory.Delete(appPath,true);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
