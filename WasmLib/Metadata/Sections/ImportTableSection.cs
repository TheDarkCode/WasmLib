using System.Collections.Generic;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public class ImportTableSection : Section
	{
		public IList<ImportMetadata> Imports { get; } = new List<ImportMetadata>();

		public ImportTableSection()
		{
			Id = SectionNames.ImportTable;
		}

		protected override void ReadSpecific(BinaryReader reader)
		{
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				Imports.Add(ImportMetadata.Read(reader));
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) Imports.Count);

			foreach (var importMetadata in Imports)
				importMetadata.Write(writer);
		}
	}
}