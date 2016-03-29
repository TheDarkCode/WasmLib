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

		private ModuleMetadata()
		{
		}

		public ModuleMetadata(Module module)
		{
			Module = module;
			BuildSections();
		}

		public Module Module { get; set; }
		internal IList<Section> Sections { get; set; } = new List<Section>();

		internal T GetSection<T>() where T : Section
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

			moduleMetadata.BuildModule();

			return moduleMetadata;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((uint) MagicNumber);
			writer.Write(Module.Version);

			foreach (var section in Sections)
				section.Write(writer);
		}

		private void BuildSections()
		{
			// Factorize signatures
			var signaturesSet = new HashSet<Signature>();
			foreach (var function in Module.Functions)
				signaturesSet.Add(function.Signature);

			foreach (var function in Module.Imports)
				signaturesSet.Add(function.Signature);

			var signaturesSection = new SignaturesSection();
			Sections.Add(signaturesSection);
			foreach (var signature in signaturesSet)
				signaturesSection.Signatures.Add(new SignatureMetadata {Signature = signature});

			// compute imports
			var signatures = signaturesSet.ToList();
			var importsSection = new ImportTableSection();
			foreach (var function in Module.Imports)
			{
				importsSection.Imports.Add(new ImportMetadata
				{
					FunctionName = function.Name,
					ModuleName = function.ModuleName,
					SignatureIndex = (uint) signatures.IndexOf(function.Signature)
				});
			}
			if (importsSection.Imports.Count > 0)
				Sections.Add(importsSection);

			// compute function signatures
			var functionSignaturesSection = new FunctionSignaturesSection();
			foreach (var function in Module.Functions)
				functionSignaturesSection.SignatureIndexes.Add((uint) signatures.IndexOf(function.Signature));
			Sections.Add(functionSignaturesSection);

			// compute indirects
			var indirectsSection = new FunctionTableSection();
			foreach (var function in Module.Indirects)
				indirectsSection.SignatureIndexes.Add((uint)Module.Functions.IndexOf(function));
			if (Module.Indirects.Count > 0)
				Sections.Add(indirectsSection);

			// compute Memory
			Sections.Add(new MemorySection { MemoryInfo = Module.MemoryInfo });

			// compute exports
			var exportsSection = new ExportTableSection();
			foreach (var function in Module.Functions.Where(f => f.IsExported))
			{
				exportsSection.Exports.Add(new ExportMetadata
				{
					FunctionName = function.Name,
					FunctionIndex = (uint) Module.Functions.IndexOf(function)
				});
			}
			if (exportsSection.Exports.Count > 0)
				Sections.Add(exportsSection);

			// compute bodies
			var bodiesSection = new FunctionBodiesSection();
			foreach (var function in Module.Functions)
			{
				bodiesSection.Bodies.Add(new FunctionBodyMetadata
				{
					FunctionBody = function.Body
				});
			}
			Sections.Add(bodiesSection);
		}

		private void BuildModule()
		{
			var signaturesSection = GetSection<SignaturesSection>();
			var bodiesSection = GetSection<FunctionBodiesSection>();
			var functionSignaturesSection = GetSection<FunctionSignaturesSection>();
			var exportsSection = GetSection<ExportTableSection>();
			var importsSection = GetSection<ImportTableSection>();
			var memorySection = GetSection<MemorySection>();
			var indirectsSection = GetSection<FunctionTableSection>();

			foreach (var signatureIndex in functionSignaturesSection.SignatureIndexes)
			{
				var index = (int)signatureIndex;
				var function = new Function
				{
					Signature = signaturesSection.Signatures[index].Signature.Clone(),
					Body = bodiesSection.Bodies[Module.Functions.Count].FunctionBody
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
						Signature = signaturesSection.Signatures[(int) import.SignatureIndex].Signature.Clone()
					};
					Module.Imports.Add(function);
				}
			}

			if (indirectsSection != null)
			{
				foreach (var signatureIndex in indirectsSection.SignatureIndexes)
				{
					var index = (int)signatureIndex;
					var function = Module.Functions[index];
					Module.Indirects.Add(function);
				}
			}

			if (memorySection != null)
				Module.MemoryInfo = memorySection.MemoryInfo;
		}
	}
}
