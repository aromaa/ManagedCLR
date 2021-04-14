using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ManagedCLR.IL.Instructions.Raw
{
	public readonly struct CallInstruction : ILInstruction
	{
		public MethodDefinitionHandle Target { get; internal init; }
		
		public static CallInstruction Handle(MethodDefinitionHandle handle) => new()
		{
			Target = handle
		};

		public void Consume(ILConsumer reader)
		{
			reader.Consume(this);
		}

		public static CallInstruction Handle(int rowNumber) => CallInstruction.Handle(MetadataTokens.MethodDefinitionHandle(rowNumber));
	}
}
