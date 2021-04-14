using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using ManagedCLR.IL;
using ManagedCLR.Runtime.Assembly.Parser;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Assembly.Loader
{
	public sealed class AssemblyLoader
	{
		private readonly PEReader peReader;
		private readonly MetadataReader metadata;

		public AssemblyLoader(string path)
		{
			this.peReader = new PEReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
			this.metadata = this.peReader.GetMetadataReader();
		}

		public MethodDefinitionHandle EntryMethod => MetadataTokens.MethodDefinitionHandle(this.peReader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress);

		public TypeMethodHandle ReadMethod(MethodDefinitionHandle methodHandle)
		{
			MethodDefinition methodDefinition = this.metadata.GetMethodDefinition(methodHandle);

			MethodBodyBlock methodBody = this.peReader.GetMethodBody(methodDefinition.RelativeVirtualAddress);

			ILMethodReader reader = new();
			reader.Read(methodBody);

			return new TypeMethodHandle()
			{
				Loader = this,
				IL = new(reader.instructions)
				{
					ILOffsets = reader.ilOffset.ToImmutableDictionary(),
					ArgumentsCount = methodDefinition.GetParameters().Count,
					Blocks = new Dictionary<int, BasicBlock>(reader.blocks)
				}
			};
		}
	}
}
