using System.Reflection.Metadata;

namespace ManagedCLR.IL.Extensions
{
	internal static class BlobReaderExtension
	{
		public static ILOpCode DecodeILOpCode(this ref BlobReader blob)
		{
			byte opCodeByte = blob.ReadByte();

			return (ILOpCode)(opCodeByte == 0xFE ? 0xFE00 + blob.ReadByte() : opCodeByte);
		}
	}
}
