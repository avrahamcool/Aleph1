namespace Aleph1.ClientFile.Models
{
    /// <summary>a file uploaded from an HTTP client using JSON</summary>
    /// <remarks>https://developer.mozilla.org/en-US/docs/Web/API/File</remarks>
    public class ClientFile
    {
        /// <summary>this constructor should not be use directly - it is present for the JSON serializer.</summary>
        /// <param name="name">the name of the file</param>
        /// <param name="lastModified">the last modified time of the file, in millisecond since the UNIX epoch (January 1st, 1970 at Midnight).</param>
        /// <param name="size">the size of the file in bytes.</param>
        /// <param name="type">the MIME type of the file.</param>
        /// <param name="content">the file itself.</param>
        public ClientFile(string name, long lastModified, long size, string type, byte[] content)
        {
            this.Name = name;
            this.LastModified = lastModified;
            this.Size = size;
            this.Type = type;
            this.Content = content;
        }

        /// <summary>the name of the file</summary>
        public string Name { get; private set; }

        /// <summary>the last modified time of the file, in millisecond since the UNIX epoch (January 1st, 1970 at Midnight).</summary>
        public long LastModified { get; private set; }

        /// <summary>the size of the file in bytes.</summary>
        public long Size { get; private set; }

        /// <summary>the MIME type of the file.</summary>
        public string Type { get; private set; }

        /// <summary>the file itself.</summary>
        public byte[] Content { get; private set; }
    }
}
