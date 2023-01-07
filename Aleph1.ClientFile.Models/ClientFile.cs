namespace Aleph1.ClientFile.Models
{
	/// <summary>a file uploaded from an HTTP client using JSON</summary>
	/// <remarks>https://developer.mozilla.org/en-US/docs/Web/API/File</remarks>
	public class ClientFile
	{
		/// <summary>the name of the file</summary>
		public string Name { get; set; }

		/// <summary>the last modified time of the file, in millisecond since the UNIX epoch (January 1st, 1970 at Midnight).</summary>
		public long LastModified { get; set; }

		/// <summary>the size of the file in bytes.</summary>
		public long Size { get; set; }

		/// <summary>the MIME type of the file.</summary>
		public string Type { get; set; }

		/// <summary>the file itself.</summary>
		public byte[] Content { get; set; }
	}
}
