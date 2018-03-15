using System;

namespace UMemory
{
	[Flags]
	public enum HeapFlags : uint
	{
		/// <summary>
		/// No flags.
		/// </summary>
		HEAP_NONE = 0x00000000,

		/// <summary>
		/// All memory blocks that are allocated from this heap allow code execution, if the hardware enforces data execution prevention. Use this flag heap in applications that run code from the heap. If HEAP_CREATE_ENABLE_EXECUTE is not specified and an application attempts to run code from a protected page, the application receives an exception with the status code STATUS_ACCESS_VIOLATION.
		/// </summary>
		HEAP_NO_SERIALIZE = 0x00000001,

		/// <summary>
		/// The system raises an exception to indicate failure (for example, an out-of-memory condition) for calls to HeapAlloc and HeapReAlloc instead of returning NULL.
		/// </summary>
		HEAP_GENERATE_EXCEPTIONS = 0x00000004,

		/// <summary>
		/// The allocated memory will be initialized to zero. Otherwise, the memory is not initialized to zero.
		/// </summary>
		HEAP_ZERO_MEMORY = 0x00000008,

		/// <summary>
		/// Serialized access is not used when the heap functions access this heap. This option applies to all subsequent heap function calls. Alternatively, you can specify this option on individual heap function calls.
		/// </summary>
		HEAP_CREATE_ENABLE_EXECUTE = 0x00040000
	}
}
