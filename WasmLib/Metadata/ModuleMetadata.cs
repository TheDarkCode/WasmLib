using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WasmLib.Metadata.Sections;

namespace WasmLib.Metadata
{
	public class ModuleMetadata
	{
		private const int MagicNumber = 0x6d736100;
		public Module Module { get; set; }
		private IList<Section> Sections { get; set; } = new List<Section>();

		private T GetSection<T>() where T : Section
		{
			return Sections.FirstOrDefault(s => s is T) as T;
		}

		public static ModuleMetadata Read(BinaryReader reader)
		{
			var moduleMetadata = new ModuleMetadata();
			var module = new Module();

			moduleMetadata.Module = module;

			if (reader.ReadUInt32() != MagicNumber)
				throw new NotSupportedException("bad signature");

			module.Version = reader.ReadUInt32();

			var sections = new List<Section>();
			while (reader.BaseStream.Position < reader.BaseStream.Length)
				sections.Add(Section.Read(reader));

			moduleMetadata.Sections = sections.ToList();

			var memory = moduleMetadata.GetSection<MemorySection>();
			if (memory != null)
				module.MemoryInfo = memory.MemoryInfo;

			moduleMetadata.BuildObjectGraph();

			return moduleMetadata;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((uint) MagicNumber);
			writer.Write(Module.Version);

			foreach (var section in Sections)
				section.Write(writer);
		}

		private void BuildObjectGraph()
		{
			var signaturesSection = GetSection<SignaturesSection>();
			var bodiesSection = GetSection<FunctionBodiesSection>();
			var functionSignaturesSection = GetSection<FunctionSignaturesSection>();
			var exportsSection = GetSection<ExportTableSection>();
			var importsSection = GetSection<ImportTableSection>();
			var memorySection = GetSection<MemorySection>();

			foreach (var signatureIndex in functionSignaturesSection.SignatureIndexes)
			{
				var index = (int)signatureIndex;
				var function = new Function
				{
					Signature = signaturesSection.Signatures[index].Signature,
					Body = bodiesSection.Bodies[index].FunctionBody
				};
				Module.Functions.Add(function);
			}

			if (exportsSection != null)
			{
				foreach (var export in exportsSection.Exports)
				{
					var exportedFunction = Module.Functions[(int)export.FunctionIndex];
					exportedFunction.Name = export.FunctionName;
					exportedFunction.IsExported = true;
				}
			}

			if (importsSection != null)
			{
				foreach (var import in importsSection.Imports)
				{
					var function = new Function
					{
						IsImported = true,
						Name = import.FunctionName,
						ModuleName = import.ModuleName,
						Signature = signaturesSection.Signatures[(int) import.SignatureIndex].Signature
					};
					Module.Imports.Add(function);
				}
			}

			if (memorySection != null)
				Module.MemoryInfo = memorySection.MemoryInfo;
		}
	}
}
