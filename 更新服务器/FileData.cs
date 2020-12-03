using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace 更新服务器
{
    class FileData
    {
        public int totalRank = 1;
        public int currentRank = 1;

        public string tag;
        public string direPath;
        public string fileName;
        public byte[] data;
        public FileData()
        {
        }
        public FileData(string tag, string tragetParentPath, FileInfo fileInfo)
        {
            this.tag = tag;
            string path = tragetParentPath + "/" + tag + "/";
            //direPath = fileInfo.DirectoryName.Replace(path, "").Replace('\\', '/');
            direPath = fileInfo.DirectoryName.Replace(Directory.GetCurrentDirectory(), "");
            fileName = fileInfo.Name;
            //data = new byte[] { 5,6 };
            System.Console.WriteLine("开始下载" + fileInfo.FullName.Replace('\\', '/'));
            data = File.ReadAllBytes(fileInfo.FullName.Replace('\\', '/'));
            System.Console.WriteLine("载入完成" + data.Length);
        }

        public FileData(int totalRank, int currentRank, string tag, string direPath, string fileName, byte[] data)
        {
            this.totalRank = totalRank;
            this.currentRank = currentRank;
            this.tag = tag;
            this.direPath = direPath;
            this.fileName = fileName;
            this.data = data;
        }

        public List<FileData> ToClip()
        {
            int length = 100000;
            List<FileData> standFileData = new List<FileData>();
            for (int i = 0; i < data.Length; i += length)
            {
                standFileData.Add(new FileData(data.Length / length + 1, i / length + 1, tag, direPath, fileName, data.Skip(i).Take(length).ToArray()));
            }
            return (standFileData);
        }
    }
}
