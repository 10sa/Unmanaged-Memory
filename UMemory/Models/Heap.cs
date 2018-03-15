using System;
using UMemory.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMemory.Models
{
	public sealed class Heap
	{
		public static implicit operator Heap(IntPtr handle)
		{
			return handle;
		}

		public static implicit operator IntPtr(Heap heap)
		{
			return heap;
		}

		/// <summary>
		/// Creates a private heap object that can be used by the calling process. The function reserves space in the virtual address space of the process and allocates physical storage for a specified initial portion of this block.
		/// </summary>
		/// <param name="flags">The heap allocation options. These options affect subsequent access to the new heap through calls to the heap functions. This parameter can be 0 or one or more of the following values.</param>
		/// <param name="initSize">The initial size of the heap, in bytes. This value determines the initial amount of memory that is committed for the heap. The value is rounded up to a multiple of the system page size. The value must be smaller than dwMaximumSize.</param>
		/// <param name="maxSize">The maximum size of the heap, in bytes. The HeapCreate function rounds dwMaximumSize up to a multiple of the system page size and then reserves a block of that size in the process's virtual address space for the heap. If allocation requests made by the HeapAlloc or HeapReAlloc functions exceed the size specified by dwInitialSize, the system commits additional pages of memory for the heap, up to the heap's maximum size.</param>
		/// <returns>If the function succeeds, the return value is a handle to the newly created heap.</returns>
		public static Heap CreateHeap(HeapFlags flags, uint initSize, uint maxSize) => MemAPIs.CreateHeap(flags, initSize, maxSize);

		/// <summary>
		/// Creates a private heap object that can be used by the calling process. The function reserves space in the virtual address space of the process and allocates physical storage for a specified initial portion of this block.
		/// </summary>
		/// <param name="initSize">The initial size of the heap, in bytes. This value determines the initial amount of memory that is committed for the heap. The value is rounded up to a multiple of the system page size. The value must be smaller than dwMaximumSize.</param>
		/// <param name="maxSize">The maximum size of the heap, in bytes. The HeapCreate function rounds dwMaximumSize up to a multiple of the system page size and then reserves a block of that size in the process's virtual address space for the heap. If allocation requests made by the HeapAlloc or HeapReAlloc functions exceed the size specified by dwInitialSize, the system commits additional pages of memory for the heap, up to the heap's maximum size.</param>
		/// <returns>If the function succeeds, the return value is a handle to the newly created heap.</returns>
		public static Heap CreateHeap(uint initSize, uint maxSize) => MemAPIs.CreateHeap(HeapFlags.HEAP_NONE, initSize, maxSize);

		/// <summary>
		/// Creates a private heap object that can be used by the calling process. The function reserves space in the virtual address space of the process and allocates physical storage for a specified initial portion of this block.
		/// </summary>
		/// <returns>If the function succeeds, the return value is a handle to the newly created heap.</returns>
		public static Heap CreateHeap() => MemAPIs.CreateHeap(HeapFlags.HEAP_NONE, 0, 1024);
	}
}
