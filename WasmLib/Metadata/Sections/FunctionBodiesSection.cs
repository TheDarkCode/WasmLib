using System.Collections.Generic;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public class FunctionBodiesSection : Section
	{
		public IList<FunctionBodyMetadata> Bodies { get; } = new List<FunctionBodyMetadata>();

		public FunctionBodiesSection()
		{
			Id = SectionNames.FunctionBodies;
		}

		protected override void ReadSpecific(BinaryReader reader)
		{
			var count = reader.ReadVarUint32();

			for (var i = 0; i < count; i++)
				Bodies.Add(FunctionBodyMetadata.Read(reader));
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.WriteVarUint32((uint) Bodies.Count);

			foreach (var functionBodyMetadata in Bodies)
				functionBodyMetadata.Write(writer);
		}
	}
}
