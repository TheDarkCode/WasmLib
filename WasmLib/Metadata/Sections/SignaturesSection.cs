using System.Collections.Generic;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public class SignaturesSection : Section
	{
		public IList<SignatureMetadata> Signatures { get; } = new List<SignatureMetadata>();

		public SignaturesSection()
		{
			Id = SectionNames.Signatures;
		}

		protected override void ReadSpecific(BinaryReader reader)
		{
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				Signatures.Add(SignatureMetadata.Read(reader));
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) Signatures.Count);

			foreach (var signatureMetadata in Signatures)
				signatureMetadata.Write(writer);
		}
	}
}
