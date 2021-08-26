//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//class FlieList
//{
//    public string tag = "";
//    public List<Info> fileInfos = new List<Info>();

//    public FlieList(string tag) => this.tag = tag;

//    public void Add(FileInfo fileInfo)
//    {
//        fileInfos.Add(new Info(fileInfo));
//    }
//    public class Info
//    {
//        private const string Pattern = "[\\/\\\\]";
//        public List<string> paths = new List<string>();
//        public long length;

//        public Info(FileInfo fileInfo)
//        {
//            length = fileInfo.Length;
//            paths = Regex.Split(fileInfo.FullName.Replace(Directory.GetCurrentDirectory(), ""), Pattern).Skip(1).ToList();
//        }
//    }
//}

