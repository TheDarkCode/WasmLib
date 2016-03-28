namespace WasmLib.Debug
{
	internal class Program
	{
		private static void Main()
		{
			var module = Module.Read(@"..\..\..\WasmLib.Tests\Files\AngryBots.wasm");
		}
	}
}
