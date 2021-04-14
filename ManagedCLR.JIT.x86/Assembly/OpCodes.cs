namespace ManagedCLR.JIT.x86.Assembly
{
	internal enum OpCodes : uint
	{
		AddRegister = 0x01,
		/// <summary>
		/// Pushes a register on top of the stack. The lower 3 bits of the opcode encode the register
		/// </summary>
		CompareRegisters = 0x39,
		PushRegister = 0x50,
		PushRbp = 0x55,
		/// <summary>
		/// Pops top of the stack into register. The lower 3 bits of the opcode encode the register
		/// </summary>
		PopRegister = 0x58,
		PopRbp = 0x5D,
		/// <summary>
		/// Pushes a immediate byte on top of the stack.
		/// </summary>
		PushImmediate8 = 0x6A,
		/// <summary>
		/// Pushes a immediate int on top of the stack.
		/// </summary>
		PushImmediate32 = 0x68,
		JumpGreater = 0x7F,
		/// <summary>
		/// Subbb
		/// </summary>
		AddImmediate = 0x81,
		/// <summary>
		/// Subbb
		/// </summary>
		SubtractImmediate = 0x81,
		/// <summary>
		/// 
		/// </summary>
		MoveToRegisterImmediate = 0xB8,
		/// <summary>
		/// Wow!
		/// </summary>
		MoveToRegister = 0x89,
		/// <summary>
		/// Return
		/// </summary>
		NearReturnPop = 0xC2,
		NearReturn = 0xC3,
		/// <summary>
		/// Calling
		/// </summary>
		JumpShort = 0xEB,
		CallNearRelative = 0xE8,
		JumpNearRelative = 0xE9,
		CallNearAbsolute = 0xFF,
		PushMemory = 0xFF,
	}
}
