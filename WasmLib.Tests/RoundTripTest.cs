using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WasmLib.Metadata;
using WasmLib.Metadata.Sections;

namespace WasmLib.Tests
{
	[TestClass]
	public class RoundTripTest : BaseTest
	{
		[TestMethod]
		public void TestLowLevelRoundTrip()
		{
			foreach (var file in Directory.GetFiles(_filesDirectory, "*.wasm"))
			{
				TestContext.WriteLine("Testing {0}", file);
				var tmpFile = Path.GetTempFileName();

				ModuleMetadata moduleMetadata;
				using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				using (var reader = new BinaryReader(stream))
					moduleMetadata = ModuleMetadata.Read(reader);

				using (var stream = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
				using (var writer = new BinaryWriter(stream))
					moduleMetadata.Write(writer);

				FileHelper.CompareFiles(file, tmpFile);
			}
		}

		[TestMethod]
		public void TesHighLevelRoundTrip()
		{
			foreach (var file in Directory.GetFiles(_filesDirectory, "*.wasm"))
			{
				TestContext.WriteLine("Testing {0}", file);
				var tmpFile = Path.GetTempFileName();

				ModuleMetadata moduleMetadata;

				using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				using (var reader = new BinaryReader(stream))
					moduleMetadata = ModuleMetadata.Read(reader);

				var newModuleMetadata = new ModuleMetadata(moduleMetadata.Module);
				using (var stream = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
				using (var writer = new BinaryWriter(stream))
					newModuleMetadata.Write(writer);

				CompareSections(moduleMetadata, newModuleMetadata);
			}
		}

		private void CompareSections(ModuleMetadata left, ModuleMetadata right)
		{
			Assert.AreEqual(left.Sections.Count, right.Sections.Count);
			for (var i = 0; i < left.Sections.Count; i++)
				Assert.AreEqual(left.Sections[i].Id, right.Sections[i].Id);

			var lsignatures = left.GetSection<SignaturesSection>();
			var rsignatures = right.GetSection<SignaturesSection>();
			Assert.AreEqual(lsignatures.Signatures.Count, rsignatures.Signatures.Count, lsignatures.Id);

			var lbodies = left.GetSection<FunctionBodiesSection>();
			var rbodies = right.GetSection<FunctionBodiesSection>();
			Assert.AreEqual(lbodies.Bodies.Count, rbodies.Bodies.Count, lbodies.Id);
			for (var i = 0; i < lbodies.Bodies.Count; i++)
			{
				var lfunctionBody = lbodies.Bodies[i].FunctionBody;
				var rfunctionBody = rbodies.Bodies[i].FunctionBody;
				Assert.AreEqual(lfunctionBody.Ast.Length, rfunctionBody.Ast.Length);
				Assert.AreEqual(lfunctionBody.Locals.Count, rfunctionBody.Locals.Count);
			}
		}
	}
}

