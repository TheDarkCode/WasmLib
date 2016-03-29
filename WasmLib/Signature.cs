using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WasmLib
{
	public class Signature : ICloneable, IEquatable<Signature>
	{
		public IList<ValueType> Parameters { get; private set; } = new List<ValueType>();
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

		public virtual Signature Clone()
		{
			return (this as ICloneable).Clone() as Signature;
		}

		object ICloneable.Clone()
		{
			var result = (Signature) MemberwiseClone();
			result.Parameters = new List<ValueType>(result.Parameters);
			return result;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Signature);
		}

		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			var hash = Parameters.Aggregate(13, (current, parameter) => current*7 + parameter.GetHashCode());

			// ReSharper disable once NonReadonlyMemberInGetHashCode
			hash = hash * 7 + ReturnType.GetHashCode();
			return hash;
		}

		public bool Equals(Signature other)
		{
			if (ReturnType != other?.ReturnType)
				return false;

			if (Parameters.Count != other.Parameters.Count)
				return false;

			return !Parameters.Where((t, i) => t != other.Parameters[i]).Any();
		}
	}
}
