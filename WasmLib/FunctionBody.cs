using System.Collections.Generic;

namespace WasmLib
{
	public class FunctionBody
	{
		public List<Local> Locals { get; } = new List<Local>();
		public byte[] Ast { get; set; }
	}
}