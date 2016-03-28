using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WasmLib.Metadata;
using System.Text;

namespace WasmLib.Tests
{
	[TestClass]
	public class RoundTripTest : BaseTest
	{
		[TestMethod]
		public void TestRoundTrip()
		{
			foreach (var file in Directory.GetFiles(_filesDirectory, "*.wasm"))
			{
				TestContext.WriteLine("Testing {0}", file);
				var tmpFile = Path.GetTempFileName();

				ModuleMetadata moduleMetadata;
				using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
					using (var reader = new BinaryReader(stream, Encoding.ASCII))
						moduleMetadata = ModuleMetadata.Read(reader);

				using (var stream = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
				using (var writer = new BinaryWriter(stream, Encoding.ASCII))
					moduleMetadata.Write(writer);

				FileHelper.CompareFiles(file, tmpFile);
			}
		}
	}
}

