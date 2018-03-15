using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace UMemory.Models
{
	/// <summary>
	/// Implement Unmanaged memory. This class is sealed class.
	/// </summary>
	public sealed class Memory : IDisposable, ICloneable, IComparable<Memory>, IEnumerable
	{
		/// <summary>
		/// Allocated memory size.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Allocated memory area address. This address isn't Memory instance address.
		/// </summary>
		public IntPtr Address { get; private set; }

		/// <summary>
		/// Whether or not to free the unmanaged memory area that the object points to when the Memory object is free.
		/// </summary>
		public bool IsFreeOnDispose { get; set; }

		private Memory() { }

		internal static Memory Allocation(int size)
		{
			IntPtr memoryObject = Marshal.AllocHGlobal(size);
			Memory memory = new Memory();
			memory.Address = memoryObject;
			memory.Size = size;
			memory.IsFreeOnDispose = false;

			return memory;
		}

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
			Memory memory = Allocation(this.Size);
			UMemory.Memcpy(this, memory, this.Size);

			return memory;
		}

		public void Dispose()
		{
			if (this.IsFreeOnDispose)
				Free();
		}

		/// <summary>
		/// Release pointed heap memory.
		/// </summary>
		public void Free()
		{
			Marshal.FreeHGlobal(Address);
		}

		public int CompareTo(Memory other)
		{
			if (Equals(other))
				return 0;
			else if (Address.ToInt32() > other.Address.ToInt32())
				return 1;
			else
				return -1;
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
			private int size;

			private MemoryByte() { }
			public MemoryByte(IntPtr address, int size)
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
