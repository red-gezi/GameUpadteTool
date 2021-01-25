using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
class FileList
{
    public string tag = "";
    public List<Info> fileInfos = new List<Info>();

    public FileList()
    {
    }

    public void Add(FileInfo fileInfo)
    {
        fileInfos.Add(new Info(fileInfo));
    }
    public bool Check(FileInfo fileInfo)
    {
        Info info = new Info(fileInfo);
        var target = fileInfos.FirstOrDefault(file =>
        {
            //Console.WriteLine("开始判断");
            //Console.WriteLine(file.ToJson());
            //Console.WriteLine(info.ToJson());
            //Console.WriteLine(file.ToJson() == info.ToJson());
            return file.ToJson() == info.ToJson();

        });
        return target != null;
    }
    public class Info
    {
        private const string Pattern = "[\\/\\\\]";
        public List<string> paths = new List<string>();
        public long length;

        public Info()
        {
        }

        public Info(FileInfo fileInfo)
        {
            length = fileInfo.Length;
            paths = Regex.Split(fileInfo.FullName.Replace(Directory.GetCurrentDirectory(), ""), Pattern).Skip(1).ToList();
        }
    }
}

