using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata
{
	public class ImportMetadata
	{
		public string ModuleName { get; set; }
		public string FunctionName { get; set; }
		public uint SignatureIndex { get; set; }

		public static ImportMetadata Read(BinaryReader reader)
		{
			var import = new ImportMetadata
			{
				SignatureIndex = reader.ReadVarUint32(),
				ModuleName = reader.ReadVarString(),
				FunctionName = reader.ReadVarString()
			};

			return import;
		}

		public void Write(BinaryWriter writer)
		{
			writer.WriteVarUint32(SignatureIndex);
			writer.WriteVarString(ModuleName);
			writer.WriteVarString(FunctionName);
		}
	}
}
