using System;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Threading;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Type.Loader
{
	public sealed class TypeLoader
	{
		private ConcurrentDictionary<string, TypeHandle> types;

		//Workaround for now because we can't pin pointers in .NET 5 (Support for pinned references is however enabled in master (.NET 6) but not exposed in the API)
		private ConcurrentDictionary<uint, TypeMethodHandle> slots;
		private uint nextSlot;

		public TypeLoader()
		{
			this.types = new ConcurrentDictionary<string, TypeHandle>();

			this.slots = new ConcurrentDictionary<uint, TypeMethodHandle>();
		}

		public uint GetNextSlot() => Interlocked.Increment(ref this.nextSlot);

		public TypeMethodHandle GetSlot(uint slot) => this.slots[slot];

		internal void RegisterMethodHandle(TypeMethodHandle handle)
		{
			this.slots[handle.ClrData.Slot] = handle;
		}

		public TypeHandle LoadType(MetadataReader reader, TypeDefinition type)
		{
			string namespaceName = reader.GetString(type.Namespace);
			string typeName = reader.GetString(type.Name);

			return this.types.GetOrAdd($"{namespaceName}.{typeName}", new TypeHandle(this));
		}
	}
}
