using ManagedCLR.IL.Methods;
using ManagedCLR.Runtime.Assembly.Loader;

namespace ManagedCLR.Runtime.Type.Method
{
	public sealed class TypeMethodHandle
	{
		public AssemblyLoader Loader { get; init; }
		public ILMethodDefinition IL { get; init; }
	}
}
