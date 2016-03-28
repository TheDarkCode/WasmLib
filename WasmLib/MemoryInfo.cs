namespace WasmLib
{
	public class MemoryInfo
	{
		public uint MinMemoryPages { get; set; }
		public uint MaxMemoryPages { get; set; }
		public bool IsExported { get; set; }
	}
}