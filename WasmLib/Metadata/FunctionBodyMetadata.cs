using System;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata
{
	public class FunctionBodyMetadata
	{
		public FunctionBody FunctionBody { get; set; } = new FunctionBody();

		internal static FunctionBodyMetadata Read(BinaryReader reader)
		{
			var bodyMetadata = new FunctionBodyMetadata();
			var body = new FunctionBody();
			bodyMetadata.FunctionBody = body;

			var bodysize = reader.ReadVarUint32();
			var marker = reader.BaseStream.Position;
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				body.Locals.Add(LocalEntryMetadata.Read(reader).Local);

			body.Ast = reader.ReadBytes((int) (bodysize - reader.BaseStream.Position + marker));

			if (reader.BaseStream.Position != marker + bodysize)
				throw new NotSupportedException("unexpected read offset");

			return bodyMetadata;
		}

		public void Write(BinaryWriter writer)
		{
			writer.WriteSizedPart(w =>
			{
				w.WriteVarUint32((uint) FunctionBody.Locals.Count);

				foreach (var local in FunctionBody.Locals)
					new LocalEntryMetadata {Local = local}.Write(w);

				w.Write(FunctionBody.Ast);
			}, 5);
		}
	}
}