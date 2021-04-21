using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct InitObjectInstruction : ILInstruction
	{
		public TypeDefinitionHandle Target { get; internal init; }

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static InitObjectInstruction Handle(TypeDefinitionHandle handle) => new()
		{
			Target = handle
		};

		public static InitObjectInstruction Handle(int rowNumber) => InitObjectInstruction.Handle(MetadataTokens.TypeDefinitionHandle(rowNumber));
	}
}
