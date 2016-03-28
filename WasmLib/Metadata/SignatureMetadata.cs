using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata
{
	public class SignatureMetadata
	{
		public Signature Signature { get; set; } = new Signature();

		public static SignatureMetadata Read(BinaryReader reader)
		{
			var result = new SignatureMetadata();
			var count = reader.ReadVarUint32();

			result.Signature.ReturnType = reader.ReadValueType();

			for (var i = 0; i < count; i++)
				result.Signature.Parameters.Add(reader.ReadValueType());

			return result;
		}

		public void Write(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) Signature.Parameters.Count);
			writer.WriteValueType(Signature.ReturnType);

			foreach (var parameter in Signature.Parameters)
				writer.WriteValueType(parameter);
		}
	}
}
