using System.Collections.Generic;
using System.IO;
class FileData
{
    public int totalRank=1;
    public int currentRank=1;

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
        string path = tragetParentPath;
        direPath = fileInfo.DirectoryName.Replace(path, "").Replace('\\', '/');
        fileName = fileInfo.Name;
        data = File.ReadAllBytes(fileInfo.FullName);
    }
    
}