using System.Collections.Generic;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public abstract class BaseSignatureIndexesSection : Section
	{
		public IList<uint> SignatureIndexes { get; } = new List<uint>();

		protected override void ReadSpecific(BinaryReader reader)
		{
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				SignatureIndexes.Add(reader.ReadVarUint32());
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) SignatureIndexes.Count);

			foreach (var signatureIndex in SignatureIndexes)
				writer.WriteVarUint32(signatureIndex);
		}
	}
}
