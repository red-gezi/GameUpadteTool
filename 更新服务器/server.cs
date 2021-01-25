
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace 更新服务器
{
    partial class server
    {
        static List<DownLoadTask> downLoadTasks = new List<DownLoadTask>();
        static void Main(string[] args)
        {

            var server = new WebSocketServer($"ws://0.0.0.0:495");
            server.AddWebSocketService<Update>("/Update");
            server.AddWebSocketService<Download>("/Download");
            server.AddWebSocketService<CheckVersion>("/CheckVersion");
            ///server.AddWebSocketService<ReceiveOver>("/ReceiveOver");

            server.Start();
            Console.WriteLine("服务器开始！");
            while (true)
            {

            }
        }

        private class Update : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                FileData fileData = e.Data.ToObject<FileData>();
                Console.WriteLine($"收到了{fileData.tag}-{fileData.direPath}-{fileData.fileName}-大小:{fileData.data.Length}");
                string targetPath = (fileData.tag + "/" + fileData.direPath + "/" + fileData.fileName).Replace('\\', '/');
                new FileInfo(targetPath).Directory.Create();
                Console.WriteLine("保存于" + targetPath);
                File.WriteAllBytes(targetPath, fileData.data);
            }
            protected override void OnClose(CloseEventArgs e)
            {
                Console.WriteLine(e.Reason);
            }
        }
        private class CheckVersion : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                string tag = e.Data;
                FileInfo gameVersion = new FileInfo($"{tag}/GameVerious.txt");
                Console.WriteLine(gameVersion.FullName);
                string version = "error";
                if (gameVersion.Exists)
                {
                    version = File.ReadAllText(gameVersion.FullName.Replace('\\', '/'));
                }
                Send(version);
            }
            protected override void OnClose(CloseEventArgs e) => Console.WriteLine(e.Reason);
            protected override void OnError(WebSocketSharp.ErrorEventArgs e) => Console.WriteLine(e.Message);
        }
        private class Download : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                string info = e.Data;
                if (info != "Success")
                {
                    DownLoadTask newTask = new DownLoadTask();
                    downLoadTasks.Add(newTask);
                    newTask.Start(ID, info, Sessions);
                }
                else
                {
                    Console.WriteLine("发送成功");
                    downLoadTasks.First(task => task.ID == ID).isSuccess = true;
                }

            }
            protected override void OnClose(CloseEventArgs e) => Console.WriteLine(e.Reason);
            protected override void OnError(WebSocketSharp.ErrorEventArgs e) => Console.WriteLine(e.Message);
        }
        class DownLoadTask
        {
            public string ID;
            public bool isSuccess;
            public void Start(string ID, string info, WebSocketSessionManager sessions)
            {
                //Console.WriteLine("www:"+info);
                var clientFileList = info.ToObject<FileList>();
                this.ID = ID;
                string tag = clientFileList.tag;
                if (Directory.Exists(tag))
                {
                    FileInfo[] fileInfo = new DirectoryInfo(tag)
                        .GetFiles("*", SearchOption.AllDirectories);
                    IEnumerable<FileInfo> enumerable = fileInfo
                        .Where(info => !clientFileList.Check(info));
                    var files = enumerable
                         .SelectMany(file => new FileData(tag, Directory.GetCurrentDirectory(), file)
                         .ToClip())
                         .ToList();
                    Console.WriteLine("共需要下载" + enumerable.Count() + "个文件");
                    Console.WriteLine("共分割为" + files.Count() + "个文件");
                    Task.Run(() =>
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            isSuccess = false;
                            //Console.WriteLine("开始发送" + files[i].fileName + ":" + files[i].currentRank + "/" + files[i].totalRank);
                            sessions.SendTo(files[i].ToJson(), ID);
                            while (!isSuccess) { }
                            //Console.WriteLine("发送完毕" + files[i].fileName + ":" + files[i].currentRank + "/" + files[i].totalRank);
                        }
                        Console.WriteLine("完全发送完成！");
                    });
                }

            }
        }
    }


}
