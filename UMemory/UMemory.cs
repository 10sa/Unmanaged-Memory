using UMemory.Models;
using System.Runtime.InteropServices;
using System;

namespace UMemory
{
    public static class UMemory
	{
		/// <summary>
		/// Allocate unmanaged memory areas.
		/// </summary>
		/// <param name="size">Size of memory area.</param>
		/// <returns>Allocated memory area. This object isn't garabge collectable.</returns>
		public static Memory Allocation(int size)
		{
			return Memory.Allocation(size);
		}

		public static void Memcpy(Memory src, Memory dst, int length)
		{
			Marshal.Copy(Marshal.PtrToStructure<byte[]>(src.Address), 0, src.Address, length);
		}

		#region WinAPI extern
		/// <summary>
		/// Retrieves a handle to the default heap of the calling process. This handle can then be used in subsequent calls to the heap methods.
		/// </summary>
		/// <returns>If the function succeeds, the return value is a handle to the calling process's heap.</returns>
		/// <remarks>https://msdn.microsoft.com/en-us/library/windows/desktop/aa366569(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint = "GetProcessHeap")]
		public extern static IntPtr GetProcessHeap();

		/// <summary>
		/// Allocates a block of memory from a heap. The allocated memory is not movable.
		/// </summary>
		/// <param name="handle">A handle to the heap from which the memory will be allocated. This handle is returned by the GetProcessHeap method.</param>
		/// <param name="flags">The heap allocation options. Specifying any of these values will override the corresponding value specified when the heap was created with HeapCreate. This parameter can be one or more of the following values.</param>
		/// <param name="size">The number of bytes to be allocated.</param>
		/// <returns>If the function succeeds, the return value is a pointer to the allocated memory block.</returns>
		/// <remarks>https://msdn.microsoft.com/en-us/library/windows/desktop/aa366597(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint = "HeapAlloc")]
		public extern static IntPtr HeapAlloc(IntPtr hHeap, HeapFlags dwFlags, uint dwBytes);

		/// <summary>
		/// Frees a memory block allocated from a heap by the HeapAlloc method.
		/// </summary>
		/// <param name="handle">A handle to the heap whose memory block is to be freed. This handle is returned by either the HeapAlloc method.</param>
		/// <param name="flags">The heap free options. Specifying the following value overrides the corresponding value specified in the flOptions parameter when the heap was created by using the HeapCreate method.</param>
		/// <param name="address">A pointer to the memory block to be freed. This pointer is returned by the MemoryAlloc method. If this pointer is NULL, the behavior is undefined.</param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		/// <remarks>https://msdn.microsoft.com/en-us/library/windows/desktop/aa366701(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint = "HeapFree")]
		public extern static IntPtr HeapFree(IntPtr hHeap, HeapFlags dwFlags, IntPtr lpMem);

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">A pointer to the starting address of the copied block's destination.</param>
		/// <param name="src">A pointer to the starting address of the block of memory to copy.</param>
		/// <param name="size">The size of the block of memory to copy, in bytes.</param>
		/// <returns>This function has no return value.</returns>
		/// <remarks>https://msdn.microsoft.com/ko-kr/library/windows/desktop/aa366535(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint ="CopyMemory")]
		public extern static IntPtr CopyMemory(IntPtr lpDestination, IntPtr lpSource, uint dwLength);

		/// <summary>
		/// Creates a private heap object that can be used by the calling process. The function reserves space in the virtual address space of the process and allocates physical storage for a specified initial portion of this block.
		/// </summary>
		/// <param name="flOptions">The heap allocation options. These options affect subsequent access to the new heap through calls to the heap functions. This parameter can be 0 or one or more of the following values.</param>
		/// <param name="dwInitialSize">The initial size of the heap, in bytes. This value determines the initial amount of memory that is committed for the heap. The value is rounded up to a multiple of the system page size. The value must be smaller than dwMaximumSize.</param>
		/// <param name="dwMaximumSize">The maximum size of the heap, in bytes. The HeapCreate function rounds dwMaximumSize up to a multiple of the system page size and then reserves a block of that size in the process's virtual address space for the heap. If allocation requests made by the HeapAlloc or HeapReAlloc functions exceed the size specified by dwInitialSize, the system commits additional pages of memory for the heap, up to the heap's maximum size.</param>
		/// <returns>If the function succeeds, the return value is a handle to the newly created heap.</returns>
		/// <remarks>https://msdn.microsoft.com/ko-kr/library/windows/desktop/aa366599(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint = "HeapCreate")]
		public extern static IntPtr CreateHeap(HeapFlags flOptions, uint dwInitialSize, uint dwMaximumSize);

		/// <summary>
		/// Returns the number of active heaps and retrieves handles to all of the active heaps for the calling process.
		/// </summary>
		/// <param name="dwNumberOfHeaps">The maximum number of heap handles that can be stored into the buffer pointed to by ProcessHeaps.</param>
		/// <param name="pProcessHeaps">A pointer to a buffer that receives an array of heap handles.</param>
		/// <returns>The return value is the number of handles to heaps that are active for the calling process.</returns>
		/// <remarks>https://msdn.microsoft.com/en-us/library/windows/desktop/aa366571(v=vs.85).aspx</remarks>
		[DllImport("Kernel32.dll", EntryPoint ="GetProcessHeaps")]
		public extern static uint GetProcessHeaps(uint dwNumberOfHeaps, IntPtr[] pProcessHeaps);  

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
		#endregion
	}
}
