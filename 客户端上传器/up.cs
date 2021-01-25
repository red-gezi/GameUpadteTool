using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebSocketSharp;
namespace 客户端上传器
{
    class up
    {
        static List<string> config => File.ReadAllLines(@"E:\东方格致录\TouHouCardServer\更新器\客户端上传器\bin\Debug\config.ini").ToList();
        static void Main(string[] args)
        {
           
            while (true)
            {
                string ip = config[0];
                string tag = config[1];
                string tragetParentPath = config[2];
                var client = new WebSocket($"ws://{ip}/Update");
                client.Connect();
                Console.WriteLine("连接完成");
                File.WriteAllText(tragetParentPath+"/GameVersion.txt",DateTime.Now.ToString());
                new DirectoryInfo(tragetParentPath).GetFiles("*.*", SearchOption.AllDirectories)
                    .Select(file => new FileData(tag, tragetParentPath, file))
                    .ToList().ForEach(fileData =>
                    {
                        Console.WriteLine($"上传至{fileData.tag}\n\t{fileData.direPath}\n\t{fileData.fileName}\n\t{fileData.data.Length}");
                        client.Send(fileData.ToJson());
                    });
                Console.WriteLine("发送完毕");
                Console.ReadLine();
                client.Close();

            }
        }
    }
}
