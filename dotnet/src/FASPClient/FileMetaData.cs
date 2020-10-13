using System;
using System.IO;

namespace FASPClient
{
    public class FileMetaData
    {
        public string physicalName { get; set; }
        public string physicalPrefix { get; set; }
        public string physicalKey { get; set; }
        public string virtualName { get; set; }
        public string virtualFullName { get; set; }
        public string virtualParent { get; set; }
        public long size { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string owner { get; set; }
        

        public FileMetaData(string prefix, string key, FileInfo fileInfo)
        {
            this.physicalName = key;
            physicalPrefix = prefix;
            physicalKey = Path.Combine(prefix, key).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            virtualName = fileInfo.Name;
            virtualFullName = fileInfo.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            virtualParent = Path.GetDirectoryName(fileInfo.FullName).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            size = fileInfo.Length;
            created = fileInfo.CreationTimeUtc;
            modified = fileInfo.LastWriteTimeUtc;
            try
            {
                owner = fileInfo.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            catch { }
        }


    }
}