using System;

namespace UnmanagedMemory
{
	public sealed class Heap
	{
		private Heap() { }

		private Heap(IntPtr handle) => Handle = handle;

		/// <summary>
		/// Heap handle.
		/// </summary>
		public IntPtr Handle { get; private set; }

		/// <summary>
		/// Destroys the this heap object. It decommits and releases all the pages of a private heap object, and it invalidates the handle to the heap.
		/// </summary>
		public void Destory()
		{
			MemAPIs.HeapDestory(Handle);
			Handle = IntPtr.Zero;
		}

		public static implicit operator Heap(IntPtr handle) => new Heap(handle);

		public static implicit operator IntPtr(Heap heap) => heap.Handle;

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
	}
}
