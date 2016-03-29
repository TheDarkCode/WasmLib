using System.Collections.Generic;
using System.IO;
using WasmLib.Metadata;

namespace WasmLib
{
	public class Module
	{
		public uint Version { get; set; }
		public MemoryInfo MemoryInfo { get; set; } = new MemoryInfo();
		public IList<Function> Functions { get; set; } = new List<Function>();
		public IList<Function> Imports { get; set; } = new List<Function>();
		public IList<Function> Indirects { get; set; } = new List<Function>();

		public static Module Read(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				return Read(stream);
		}

		public static Module Read(Stream stream)
		{
			using (var reader = new BinaryReader(stream))
				return Read(reader);
		}

		public static Module Read(BinaryReader reader)
		{
			var moduleMetadata = ModuleMetadata.Read(reader);
			return moduleMetadata.Module;
		}

		public void Write(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
				Write(stream);
		}

		public void Write(Stream stream)
		{
			using (var writer = new BinaryWriter(stream))
				Write(writer);
		}

		public void Write(BinaryWriter writer)
		{
			var moduleMetadata = new ModuleMetadata(this);
			moduleMetadata.Write(writer);
		}
	}
}
