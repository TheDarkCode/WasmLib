using System.Collections.Generic;
using System.IO;
using System.Text;
using WasmLib.Metadata;

namespace WasmLib
{
	public class Module
	{
		public uint Version { get; set; }
		public MemoryInfo MemoryInfo { get; set; } = new MemoryInfo();
		public IList<Function> Functions { get; set; } = new List<Function>();
		public IList<Function> Imports { get; set; } = new List<Function>();

		public static Module Read(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				return Read(stream);
		}

		public static Module Read(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ASCII))
				return Read(reader);
		}

		public static Module Read(BinaryReader reader)
		{
			var moduleMetadata = ModuleMetadata.Read(reader);
			return moduleMetadata.Module;
		}
	}
}
