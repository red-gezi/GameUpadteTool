//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//    class TaskManager
//    {
//        static Dictionary<string, List<FileData>> tasks = new Dictionary<string, List<FileData>>();
//        public static void Init() => tasks.Clear();
//        public static void Add(FileData fileData)
//        {
//            string key = fileData.direPath + "//" + fileData.fileName;
//            if (!tasks.ContainsKey(key))
//            {
//                tasks[key] = new List<FileData>() { };
//            }
//            tasks[key].Add(fileData);
//            //如果完全接收
//            if (tasks[key].Count == fileData.totalRank)
//            {
//                Console.WriteLine("开始生成文件" + key);
//                var datas = tasks[key].OrderBy(x => x.currentRank).SelectMany(x => x.data).ToArray();
//                new FileInfo(Directory.GetCurrentDirectory() + "//" + key).Directory.Create();
//                File.WriteAllBytes(Directory.GetCurrentDirectory() + "//" + key, datas);
//            }
//        }
//    }

