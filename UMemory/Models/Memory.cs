using System;
using System.Runtime.InteropServices;

namespace UMemory.Models
{
	/// <summary>
	/// Implement Unmanaged memory. This class is sealed class.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack =1)]
	public sealed class Memory : IDisposable, ICloneable, IComparable<Memory>
	{
		private Memory() { }

		/// <summary>
		/// Return allocated memory size.
		/// </summary>
		public int Size { get { return _size; } private set { _size = value; } }
		private int _size = 0;

		/// <summary>
		/// Return allocated memory area address. This address isn't Memory instance address.
		/// </summary>
		public IntPtr Address { get { return _address; } private set { _address = value; } }
		private IntPtr _address = IntPtr.Zero;

		/// <summary>
		/// Whether or not to free the unmanaged memory area that the object points to when the Memory object is free.
		/// </summary>
		public bool IsFreeOnDispose { get { return _isFreeOnDispose; } set { _isFreeOnDispose = value; } }
		private bool _isFreeOnDispose = false;

		internal static Memory Allocation(int size)
		{
			IntPtr memoryObject = Marshal.AllocHGlobal(size);
			Memory memory = (Memory)Marshal.PtrToStructure(memoryObject, typeof(Memory));
			memory.Address = IntPtr.Add(memoryObject, Marshal.SizeOf(typeof(Memory)));
			memory.Size = size;
			memory.IsFreeOnDispose = false;

			return memory;
		}

		private Memory(int size, IntPtr memoryAddress)
		{
			this.Address = memoryAddress;
		}

		/// <summary>
		/// If target object address equal, true. otherwise false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(obj, GCHandleType.Normal)) == Address;
		}

		/// <summary>
		/// Return object memory area.
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes()
		{
			return Marshal.PtrToStructure<byte[]>(Address);
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

		public override int GetHashCode()
		{
			return Address.ToInt32();
		}

		/// <summary>
		/// Returns This memory instance pointed memory area address.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Address.ToString();
		}

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
			Marshal.FreeHGlobal(Address);
			Console.WriteLine("TEST!");
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
	}
}
