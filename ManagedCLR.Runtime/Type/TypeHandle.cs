using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ManagedCLR.IL;
using ManagedCLR.IL.Methods;
using ManagedCLR.JIT;
using ManagedCLR.Runtime.Assembly.Loader;
using ManagedCLR.Runtime.Assembly.Parser;
using ManagedCLR.Runtime.Type.Loader;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Type
{
	public sealed class TypeHandle
	{
		private readonly TypeLoader typeLoader;

		private ConcurrentDictionary<string, TypeMethodHandle> methods;

		public TypeHandle(TypeLoader typeLoader)
		{
			this.typeLoader = typeLoader;

			this.methods = new ConcurrentDictionary<string, TypeMethodHandle>();
		}

		internal TypeMethodHandle LoadMethod(BaseJIT jit, AssemblyLoader loader, MetadataReader reader, MethodDefinition method, MethodBodyBlock methodBody)
		{
			string methodName = reader.GetString(method.Name);

			ILMethodReader ilReader = new();
			ilReader.Read(methodBody);

			ILMethodDefinition il = new(ilReader.instructions)
			{
				ILOffsets = ilReader.ilOffset.ToImmutableDictionary(),
				ArgumentsCount = method.GetParameters().Count,

				MaxStack = methodBody.MaxStack,

				Blocks = new Dictionary<int, BasicBlock>(ilReader.blocks)
			};

			TypeMethodHandle handle = new(jit, this.typeLoader.GetNextSlot(), il)
			{
				Loader = loader
			};

			this.typeLoader.RegisterMethodHandle(handle);

			//TODO: Support different signatures
			return this.methods.GetOrAdd(methodName, handle);
		}
	}
}
