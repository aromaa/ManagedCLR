using System;
using System.Collections.Generic;
using System.IO;
using ManagedCLR.Runtime.Assembly.Loader;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Runtime.Domains
{
	public sealed class AppDomain
	{
		private readonly Dictionary<string, AssemblyLoader> assemblies;

		public AppDomain()
		{
			this.assemblies = new Dictionary<string, AssemblyLoader>();
		}

		public void Load(FileInfo file) => this.Load(file.FullName);

		public void Load(string path)
		{
			AssemblyLoader loader = new(path);

			this.assemblies.Add(path, loader);
		}

		public TypeMethodHandle GetMethod()
		{
			foreach (AssemblyLoader loader in this.assemblies.Values)
			{
				return loader.ReadMethod(loader.EntryMethod);
			}

			throw new EntryPointNotFoundException();
		}
	}
}
