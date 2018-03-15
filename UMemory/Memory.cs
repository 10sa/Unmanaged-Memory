using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace UMemory
{
	/// <summary>
	/// Implement Unmanaged memory. This class is sealed class.
	/// </summary>
	public sealed class Memory : IDisposable, ICloneable, IEnumerable
	{
		/// <summary>
		/// Allocated memory size.
		/// </summary>
		public uint Size { get; private set; }

		/// <summary>
		/// Allocated memory area address. This address isn't Memory instance address.
		/// </summary>
		public IntPtr Address { get; private set; }

		/// <summary>
		/// A handle to the heap from which the memory allocated.
		/// </summary>
		public Heap HeapHandle { get; private set; }

		/// <summary>
		/// Whether or not to free the unmanaged memory area that the object points to when the Memory object is free.
		/// </summary>
		public bool IsFreeOnDispose { get; set; }

		private Memory() { }

		/// <summary>
		/// Allocates a block of memory from a heap. The allocated memory is not movable.
		/// </summary>
		/// <param name="size">The number of bytes to be allocated.</param>
		/// <param name="heapHandle">A handle to the heap from which the memory will be allocated.</param>
		/// <returns>If the function succeeds, Memory.Address member not null.</returns>
		public static Memory Allocation(uint size, Heap heapHandle)
		{
			IntPtr memoryObject = MemAPIs.HeapAlloc(MemAPIs.GetProcessHeap(), HeapFlags.HEAP_NONE, size);
			Memory memory = new Memory();
			memory.Address = memoryObject;
			memory.Size = size;
			memory.HeapHandle = heapHandle;
			memory.IsFreeOnDispose = false;

			return memory;
		}

		/// <summary>
		/// Allocates memory for the Heap Handle returned by the GetProcessHeap() method. The allocated memory is not movable.
		/// </summary>
		/// <param name="size">The number of bytes to be allocated.</param>
		/// <returns>If the function succeeds, Memory.Address member not null.</returns>
		public static Memory Allocation(uint size) => Allocation(size, MemAPIs.GetProcessHeap());

		/// <summary>
		/// Return object memory area.
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes()
		{
			byte[] memoryArea = new byte[Size];
			for (int i = 0; i < this.Size; i++)
				memoryArea[i] = this[i];

			return memoryArea;
		}

		/// <summary>
		/// Access allocated memory area.
		/// </summary>
		/// <param name="offset">Memory offset.</param>
		/// <returns>Allocated memory area value.</returns>
		public byte this[int offset]
		{
			get
			{
				if (offset >= Size)
					throw new FieldAccessException();
				else
					return Marshal.ReadByte(Address, offset);
			}

			set
			{
				if (value >= Size)
					throw new FieldAccessException();
				else
					Marshal.WriteByte(Address, offset, value);
			}
		}

		/// <summary>
		/// Returns This memory instance pointed memory area address.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Address.ToString();

		/// <summary>
		/// Clone this memory object. this method will be allocate new memory area.
		/// </summary>
		/// <returns>New allocated memory object.</returns>
		public object Clone()
		{
			Memory memory = Allocation(Size);
			MemAPIs.CopyMemory(Address, memory.Address, Size);

			return memory;
		}

		public void Dispose()
		{
			if (IsFreeOnDispose)
				Free();
		}

		/// <summary>
		/// Release pointed heap memory.
		/// </summary>
		public void Free()
		{
			MemAPIs.HeapFree(HeapHandle, HeapFlags.HEAP_NONE, Address);
			this.Address = IntPtr.Zero;
		}

		/// <summary>
		/// Returns Enumerator.
		/// </summary>
		/// <returns>Memory area enumerator.</returns>
		public IEnumerator GetEnumerator() => new MemoryByte(Address, Size);

		public override int GetHashCode()
		{
			var hashCode = 1195157887;
			hashCode = hashCode * -1521134295 + Size.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(Address);
			hashCode = hashCode * -1521134295 + IsFreeOnDispose.GetHashCode();
			return hashCode;
		}

		private class MemoryByte : IEnumerator
		{
			object IEnumerator.Current => GetCurrent();

			public byte GetCurrent()
			{
				if (offset < this.size)
					return Marshal.ReadByte(ptr, offset);
				else
					throw new FieldAccessException();
			}

			private int offset;
			private IntPtr ptr;
			private uint size;

			private MemoryByte() { }
			public MemoryByte(IntPtr address, uint size)
			{
				this.offset = -1;
				this.size = size;
				this.ptr = address;
			}

			public bool MoveNext()
			{
				if (offset < size - 1)
				{
					offset++;
					return true;
				}
				else
					return false;
			}

			public void Reset() => this.offset = 0;
		}
	}
}
