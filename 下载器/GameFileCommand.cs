using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebSocketSharp;

namespace 下载器
{
    internal class GameFileManeger
    {
        static string direName = Directory.GetCurrentDirectory() + "/game/";

        static List<string> config => File.ReadAllLines("config.ini").ToList();
        static List<(string, long)> serverFileList = new List<(string, long)>();
        static List<(string, long)> clientFileList = new List<(string, long)>();
        static List<(string, long)> deleteFileList = new List<(string, long)>();
        static List<(string, long)> downLoadFileList = new List<(string, long)>();
        //向服务器要取文件清单
        internal static void GetServerFileList()
        {
            string ip = config[0];
            var fileServer = new WebSocket($"ws://{ip}/GetFileList");
            TaskManager.Init();
            fileServer.OnMessage += (send, ev) =>
            {
                serverFileList = ev.Data.ToObject<List<(string, long)>>();
                Console.WriteLine("文件校验完成");
            };
            fileServer.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            fileServer.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            
        }
        //获取本地文件清单
        internal static void GetClientFileList()
        {
            clientFileList = new DirectoryInfo("game")
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Select(file => (file.FullName.Replace(direName, ""), file.Length))
                .ToList();
            Console.WriteLine("文件校验完成");
        }
        //对比获取需要下载的文件清单
        internal static bool  CheckFileList()
        {
            deleteFileList= clientFileList.Except(serverFileList).ToList();
            downLoadFileList = serverFileList.Except(clientFileList).ToList();
            deleteFileList.ToList().ForEach(file => new FileInfo(direName + file.Item1).Delete());
            return !(deleteFileList.Any() || downLoadFileList.Any());
        }
        //向服务端请求数据
        internal static void DownFileList()
        {
            string ip = config[0];
            var downloadServer = new WebSocket($"ws://{ip}/DownloadFileList");
            bool isDownOver;
            string path="";
            for (int i = 0; i < downLoadFileList.Count; i++)
            {
                isDownOver = false;
                path = direName + downLoadFileList[i].Item1;
                downloadServer.Send(downLoadFileList[i].Item1);
                while (!isDownOver)
                {
                    
                }
            }
            downloadServer.OnMessage += (send, ev) =>
            {
                File.WriteAllBytes(path, ev.RawData);
                Console.WriteLine("文件校验完成");
                isDownOver = true;
            };
            downloadServer.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            downloadServer.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
        }
    }
}