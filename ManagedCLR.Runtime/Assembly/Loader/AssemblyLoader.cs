using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using ManagedCLR.IL;
using ManagedCLR.JIT;
using ManagedCLR.Runtime.Assembly.Parser;
using ManagedCLR.Runtime.Type;
using ManagedCLR.Runtime.Type.Loader;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Assembly.Loader
{
	public sealed class AssemblyLoader
	{
		private readonly TypeLoader typeLoader;

		private readonly PEReader peReader;
		public readonly MetadataReader metadata;

		public AssemblyLoader(TypeLoader typeLoader, string path)
		{
			this.typeLoader = typeLoader;

			this.peReader = new PEReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
			this.metadata = this.peReader.GetMetadataReader();
		}

		public MethodDefinitionHandle EntryMethod => MetadataTokens.MethodDefinitionHandle(this.peReader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress);

		public TypeMethodHandle ReadMethod(BaseJIT jit, MethodDefinitionHandle methodHandle)
		{
			MethodDefinition methodDefinition = this.metadata.GetMethodDefinition(methodHandle);
			TypeDefinition typeDefinition = this.metadata.GetTypeDefinition(methodDefinition.GetDeclaringType());
			
			TypeHandle typeHandle = this.typeLoader.LoadType(this.metadata, typeDefinition);
			MethodBodyBlock methodBody = this.peReader.GetMethodBody(methodDefinition.RelativeVirtualAddress);

			return typeHandle.LoadMethod(jit, this, this.metadata, methodHandle, methodBody);
		}

		public TypeDefinition GetType(string name)
		{
			foreach (TypeDefinitionHandle typeHandle in this.metadata.TypeDefinitions)
			{
				TypeDefinition type = this.metadata.GetTypeDefinition(typeHandle);

				string namespaceName = this.metadata.GetString(type.Namespace);
				string typeName = this.metadata.GetString(type.Name);

				if ($"{namespaceName}.{typeName}" == name)
				{
					return type;
				}
			}

			throw new KeyNotFoundException();
		}
	}
}
