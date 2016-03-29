using System.Collections.Generic;
using System.Linq;

namespace WasmLib.Debug
{
	internal class Program
	{
		private static void Main()
		{
			var module = Module.Read(@"..\..\..\WasmLib.Tests\Files\AngryBots.wasm");
			var functions = module.Functions.Where(f => f.Name != null);
		}
	}
}
