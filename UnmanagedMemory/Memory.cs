using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace UnmanagedMemory
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
			IntPtr memoryObject = MemAPIs.HeapAlloc(heapHandle, HeapFlags.HEAP_NONE, size);
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
				if (offset >= Size)
					throw new FieldAccessException();
				else
					Marshal.WriteByte(Address, offset, value);
			}
		}

		/// <summary>
		/// Writes the Byte values to the memory address. Throws an ArgumentOutOfRange exception if the size field is smaller than the size of the byte values ​​to write to the address.
		/// </summary>
		/// <param name="bytes">A bytes to write to the memory address.</param>
		/// <param name="offset">The offset of the memory address.</param>
		/// <param name="length">The length of the byte to write to the memory address.</param>
		public void WriteBytes(byte[] bytes, int offset, int length)
		{
			if (bytes.Length + offset <= Size)
				Marshal.Copy(bytes, offset, Address, length);
			else
				throw new FieldAccessException();
		}

		/// <summary>
		/// Writes the Byte values to the memory address. Throws an FieldAccessException exception if the size field is smaller than the size of the byte values ​​to write to the address.
		/// </summary>
		/// <param name="bytes">A bytes to write to the memory address.</param>
		/// <param name="length">The length of the byte to write to the memory address.</param>
		/// <exception cref="FieldAccessException"></exception>
		public void WriteBytes(byte[] bytes, int length) => WriteBytes(bytes, 0, length);

		/// <summary>
		/// Writes the Byte values to the memory address.Throws an FieldAccessException exception if the size field is smaller than the size of the byte values ​​to write to the address.
		/// </summary>
		/// <param name="bytes">A bytes to write to the memory address.</param>
		public void WriteBytes(byte[] bytes) => WriteBytes(bytes, 0, bytes.Length);

		/// <summary>
		/// Write the long value to memory address.
		/// </summary>
		/// <param name="value">The value to be written.</param>
		/// <param name="offset">The offset of the memory address.</param>
		public void WriteInt64(long value, int offset) => Marshal.WriteInt64(Address, offset, value);

		/// <summary>
		/// Write the int value to memory address.
		/// </summary>
		/// <param name="value">The value to be written.</param>
		/// <param name="offset">The offset of the memory address.</param>
		public void WriteInt32(int value, int offset) => Marshal.WriteInt32(Address, offset, value);

		/// <summary>
		/// Write the short value to memory address.
		/// </summary>
		/// <param name="value">The value to be written.</param>
		/// <param name="offset">The offset of the memory address.</param>
		public void WriteInt16(short value, int offset) => Marshal.WriteInt16(Address, offset, value);

		/// <summary>
		/// Read the long value from memory address.
		/// </summary>
		/// <param name="offset">The offset of the memory address.</param>
		/// <returns>Readed value.</returns>
		public long ReadInt64(int offset) => Marshal.ReadInt64(Address, offset);

		/// <summary>
		/// Read the int value from memory address.
		/// </summary>
		/// <param name="offset">The offset of the memory address.</param>
		/// <returns>eaded value.</returns>
		public int ReadInt32(int offset) => Marshal.ReadInt32(Address, offset);

		/// <summary>
		/// Read the short value from memory address.
		/// </summary>
		/// <param name="offset">The offset of the memory address.</param>
		/// <returns>eaded value.</returns>
		public short ReadInt16(int offset) => Marshal.ReadInt16(Address, offset);

		/// <summary>
		/// Write Marshalable object to memory address.
		/// </summary>
		/// <param name="dst">The Marshalable object to write.</param>
		public void WriteObject(object dst)
		{
			WriteObject(dst, false);
		}

		/// <summary>
		/// Write Marshalable object to memory address.
		/// </summary>
		/// <param name="dst">The Marshalable object to write.</param>
		/// <param name="deleteOld">If true, remove existing data.</param>
		public void WriteObject(object dst, bool deleteOld)
		{
			if (Marshal.SizeOf(dst) <= Size)
				Marshal.StructureToPtr(dst, Address, deleteOld);
			else
				throw new FieldAccessException();
		}

		/// <summary>
		/// Read Marshalable object from memory address.
		/// </summary>
		/// <typeparam name="CastType">The Marshalable type to read.</typeparam>
		/// <returns>The object read from the memory address.</returns>
		public CastType ReadObject<CastType>()
		{
			if (Marshal.SizeOf(typeof(CastType)) <= Size)
				return Marshal.PtrToStructure<CastType>(this.Address);
			else
				throw new FieldAccessException();
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

		/// <summary>
		/// If the IsFreeOnDispose value is true, releases the memory area it points to.
		/// </summary>
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
				offset = -1;
				this.size = size;
				ptr = address;
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

			public void Reset() => offset = 0;
		}
	}
}
