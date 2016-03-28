using System.Text;

namespace WasmLib
{
	public class Function
	{
		public bool IsExported { get; set; }
		public bool IsImported { get; set; }
		public Signature Signature { get; set; }

		public string Name { get; set; }
		public bool HasName => IsExported || IsImported && !string.IsNullOrEmpty(Name);

		public string ModuleName { get; set; }
		public bool HasModuleName => IsImported && !string.IsNullOrEmpty(ModuleName);

		public FunctionBody Body { get; set; }
		public bool HasBody => !IsImported && Body != null;

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (HasModuleName)
			{
				sb.Append(ModuleName);
				sb.Append(".");
			}

			sb.Append(HasName ? Name : "?");
			sb.Append(Signature);

			return sb.ToString();
		}
	}
}
