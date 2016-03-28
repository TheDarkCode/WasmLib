using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public class MemorySection : Section
	{
		public MemoryInfo MemoryInfo { get; set; } = new MemoryInfo();

		public MemorySection()
		{
			Id = SectionNames.Memory;
		}

		protected override void ReadSpecific(BinaryReader reader)
		{
			MemoryInfo.MinMemoryPages = reader.ReadVarUint32();
			MemoryInfo.MaxMemoryPages = reader.ReadVarUint32();
			MemoryInfo.IsExported = reader.ReadBoolean();
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32(MemoryInfo.MinMemoryPages);
			writer.WriteVarUint32(MemoryInfo.MaxMemoryPages);
			writer.Write(MemoryInfo.IsExported);
		}
	}
}
