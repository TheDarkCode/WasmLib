using System;
using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata.Sections
{
	public abstract class Section
	{
		public string Id { get; protected set; }
		protected internal ulong Size { get; set; }
		protected ulong Marker { get; set; }

		public static Section Read(BinaryReader reader)
		{
			var size = reader.ReadVarUint32();

			var marker = reader.BaseStream.Position;
			var Id = reader.ReadVarString();

			var section = CreateInstance(Id);
			section.Id = Id;
			section.Size = size;
			section.Marker = (ulong) marker;
			section.ReadSpecific(reader);

			if (reader.BaseStream.Position != marker + size)
				throw new NotSupportedException($"unexpected read offset after section '{section.Id}'");

			return section;
		}

		public void Write(BinaryWriter writer)
		{
			Size = (ulong) writer.WriteSizedPart(w =>
			{
				w.WriteVarString(Id);
				WriteSpecific(w);
			}, 5);
		}

		protected abstract void ReadSpecific(BinaryReader reader);
		protected abstract void WriteSpecific(BinaryWriter writer);

		public override string ToString()
		{
			return Id;
		}

		private static Section CreateInstance(string id)
		{
			switch (id)
			{
				case SectionNames.Signatures:
					return new SignaturesSection();
				case SectionNames.ImportTable:
					return new ImportTableSection();
				case SectionNames.FunctionSignatures:
					return new FunctionSignaturesSection();
				case SectionNames.FunctionTable:
					return new FunctionTableSection();
				case SectionNames.Memory:
					return new MemorySection();
				case SectionNames.ExportTable:
					return new ExportTableSection();
				case SectionNames.FunctionBodies:
					return new FunctionBodiesSection();
				default:
					return new UnknownSection();
			}
		}
	}
}
