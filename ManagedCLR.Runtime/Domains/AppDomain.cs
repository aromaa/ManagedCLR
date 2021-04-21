using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagedCLR.JIT;
using ManagedCLR.Runtime.Assembly.Loader;
using ManagedCLR.Runtime.Type.Loader;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Domains
{
	public sealed class AppDomain
	{
		public static AppDomain Instance;

		public TypeLoader TypeLoader { get; }

		private readonly Dictionary<string, AssemblyLoader> assemblies;

		public AppDomain()
		{
			AppDomain.Instance = this;

			this.TypeLoader = new TypeLoader();

			this.assemblies = new Dictionary<string, AssemblyLoader>();
			this.assemblies["System.Private.CoreLib"] = new AssemblyLoader(this.TypeLoader, typeof(int).Assembly.Location);
		}

		public void Load(FileInfo file) => this.Load(file.FullName);

		public void Load(string path)
		{
			AssemblyLoader loader = new(this.TypeLoader, path);

			this.assemblies.Add(path, loader);
		}

		public TypeMethodHandle GetMethod(BaseJIT jit)
		{
			foreach (AssemblyLoader loader in this.assemblies.Values.Skip(1))
			{
				return loader.ReadMethod(jit, loader.EntryMethod);
			}

			throw new EntryPointNotFoundException();
		}

		public AssemblyLoader GetAssembly(string path) => this.assemblies[path];
	}
}
