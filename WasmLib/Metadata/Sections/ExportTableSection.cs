using System.Collections.Generic;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public class ExportTableSection : Section
	{
		public IList<ExportMetadata> Exports { get; } = new List<ExportMetadata>();

		public ExportTableSection()
		{
			Id = SectionNames.ExportTable;
		}

		protected override void ReadSpecific(BinaryReader reader)
		{
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				Exports.Add(ExportMetadata.Read(reader));
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) Exports.Count);

			foreach (var exportMetadata in Exports)
				exportMetadata.Write(writer);
		}
	}
}
