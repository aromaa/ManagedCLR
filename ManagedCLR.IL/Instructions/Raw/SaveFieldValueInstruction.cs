using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct SaveFieldValueInstruction : ILInstruction
	{
		public FieldDefinitionHandle Target { get; internal init; }

		public static SaveFieldValueInstruction Handle(FieldDefinitionHandle handle) => new()
		{
			Target = handle
		};

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static SaveFieldValueInstruction Handle(int rowNumber) => SaveFieldValueInstruction.Handle(MetadataTokens.FieldDefinitionHandle(rowNumber));
	}
}
