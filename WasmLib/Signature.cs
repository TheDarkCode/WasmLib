using System.Collections.Generic;
using System.Text;

namespace WasmLib
{
	public class Signature
	{
		public IList<ValueType> Parameters { get; } = new List<ValueType>();
		public ValueType ReturnType { get; set; } = ValueType.None;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i > 0)
					sb.Append(",");
				sb.Append(Parameters[i]);
			}
			sb.Append(")");

			if (ReturnType != ValueType.None)
			{
				sb.Append(":");
				sb.Append(ReturnType);
			}

			return sb.ToString();
		}
	}
}
