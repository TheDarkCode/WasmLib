using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WasmLib.Tests
{
	public abstract class BaseTest
	{
		private TestContext _testContextInstance;
		protected string _filesDirectory;

		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
				_filesDirectory = Path.Combine(_testContextInstance.TestDir, @"..\..");
				_filesDirectory = Path.Combine(_filesDirectory, @"WasmLib.Tests\Files");
				_filesDirectory = Path.GetFullPath(_filesDirectory);
			}
		}
	}
}
