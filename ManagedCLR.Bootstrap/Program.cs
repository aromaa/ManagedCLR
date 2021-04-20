using ManagedCLR.JIT.x86;
using System;
using AppDomain = ManagedCLR.Runtime.Domains.AppDomain;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using ManagedCLR.Runtime.Type.Method;

namespace ManagedCLR.Bootstrap
{
	public static class Program
	{
		private static async Task Main(string[] args)
		{
			RootCommand rootCommand = Program.GenerateCommands();
			rootCommand.Handler = CommandHandler.Create((FileInfo? mainEntryFile) =>
			{
				if (mainEntryFile is null)
				{
					Console.WriteLine("Uh, you didn't specify a file to be run..");

					return;
				}

				AppDomain appDomain = new();
				appDomain.Load(mainEntryFile);

				Program.RunMethod(appDomain, appDomain.GetMethod(new X86JIT()));
			});

			await rootCommand.InvokeAsync(args);
		}

		private static RootCommand GenerateCommands()
		{
			return new(description: "Loads up IL applications and boots them up using ManagedCLR.")
			{
				new Option<FileInfo>(aliases: new[]
				{
					"--main-entry-file",
					"-f"
				}, description: "The file that contains the applications main entry.")
			};
		}

		private static unsafe void RunMethod(AppDomain appDomain, TypeMethodHandle entryMethod)
		{
			delegate* unmanaged<int> entry = (delegate* unmanaged<int>)entryMethod.EntryPointer;

			Console.WriteLine("ManagedCLR (JIT): " + entry());
		}
    }
}
