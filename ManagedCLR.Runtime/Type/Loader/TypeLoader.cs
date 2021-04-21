using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Threading;
using ManagedCLR.Runtime.Type.Method;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;

namespace ManagedCLR.Runtime.Type.Loader
{
	public sealed class TypeLoader : ISignatureTypeProvider<TypeHandle, object>
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

			int size = 0;

			Dictionary<string, int> offsets = new();
			foreach (FieldDefinitionHandle fieldHandle in type.GetFields())
			{
				FieldDefinition field = reader.GetFieldDefinition(fieldHandle);

				TypeHandle fieldType = field.DecodeSignature(this, null!);

				offsets[reader.GetString(field.Name)] = size;

				size += fieldType.Size;
			}

			return this.types.GetOrAdd($"{namespaceName}.{typeName}", new TypeHandle(this)
			{
				Size = size,

				Offsets = offsets,
			});
		}

		public TypeHandle LoadType(string assembly, string type)
		{
			return this.LoadType(AppDomain.Instance.GetAssembly(assembly).metadata, AppDomain.Instance.GetAssembly(assembly).GetType(type));
		}

		public TypeHandle GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode switch
		{
			PrimitiveTypeCode.Int32 => new TypeHandle(this)
			{
				Size = 4
			},

			_ => throw new NotImplementedException($"Primitive not implemented: {typeCode}")
		};

		public TypeHandle GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return this.LoadType(reader, reader.GetTypeDefinition(handle));
		}

		public TypeHandle GetSZArrayType(TypeHandle elementType) => throw new NotImplementedException();
		public TypeHandle GetArrayType(TypeHandle elementType, ArrayShape shape) => throw new NotImplementedException();
		public TypeHandle GetByReferenceType(TypeHandle elementType) => throw new NotImplementedException();
		public TypeHandle GetGenericInstantiation(TypeHandle genericType, ImmutableArray<TypeHandle> typeArguments) => throw new NotImplementedException();
		public TypeHandle GetPointerType(TypeHandle elementType) => throw new NotImplementedException();
		public TypeHandle GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => throw new NotImplementedException();
		public TypeHandle GetFunctionPointerType(MethodSignature<TypeHandle> signature) => throw new NotImplementedException();
		public TypeHandle GetGenericMethodParameter(object genericContext, int index) => throw new NotImplementedException();
		public TypeHandle GetGenericTypeParameter(object genericContext, int index) => throw new NotImplementedException();
		public TypeHandle GetModifiedType(TypeHandle modifier, TypeHandle unmodifiedType, bool isRequired) => throw new NotImplementedException();
		public TypeHandle GetPinnedType(TypeHandle elementType) => throw new NotImplementedException();
		public TypeHandle GetTypeFromSpecification(MetadataReader reader, object genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => throw new NotImplementedException();
	}
}
