using System;
using System.Runtime.InteropServices;

namespace UMemory.Models
{
	/// <summary>
	/// Implement Unmanaged memory. This class is sealed class.
	/// </summary>
	public sealed class Memory : IDisposable, ICloneable, IComparable<Memory>
	{
		private Memory() { }

		/// <summary>
		/// Return allocated memory size.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Return allocated memory area address. This address isn't Memory instance address.
		/// </summary>
		public IntPtr Address { get; private set; }

		/// <summary>
		/// Whether this memory instance is allocated.If this is true, then this instance is a garbage collector instance. Also, if this value is true, memory area that has been Allocated at the time of disposal is also free.
		/// </summary>
		public bool IsAllocated { get; private set; }

		/// <summary>
		/// Whether or not to free the unmanaged memory area that the object points to when the Memory object is free.
		/// </summary>
		public bool IsFreeOnDispose { get; set; } = false;

		/// <summary>
		/// Return this object address. This address isn't request result memory area.
		/// </summary>
		public IntPtr ObjectAddress
		{
			get
			{
				if (ObjectAddress == null)
					ObjectAddress = GCHandle.ToIntPtr(GCHandle.Alloc(this));
				
				return ObjectAddress;
			}

			private set
			{
				ObjectAddress = value;
			}
		}

		internal static Memory Allocation(int size)
		{
			Memory memory;
			IntPtr memoryObject = Marshal.AllocHGlobal(size + Marshal.SizeOf(typeof(Memory)));
			memory = (Memory)Marshal.PtrToStructure(memoryObject, typeof(Memory));
			memory.Address = IntPtr.Add(memoryObject, Marshal.SizeOf(typeof(Memory)));
			memory.ObjectAddress = memoryObject;
			memory.Size = size;
			memory.IsAllocated = true;

			return memory;
		}

		internal static Memory Allocation(int size, bool collectable)
		{
			if (collectable)
			{
				IntPtr memoryObject = Marshal.AllocHGlobal(size);
				return new Memory(size, memoryObject);
			}
			else
				return Allocation(size);
		}

		private Memory(int size, IntPtr memoryAddress)
		{
			this.Address = memoryAddress;
			this.IsAllocated = false;
		}

		/// <summary>
		/// Cast allocated memory area.
		/// </summary>
		/// <param name="dstType">Casting type.</param>
		/// <returns>Castingable object.</returns>
		public CastType Cast<CastType>()
		{
			if (Marshal.SizeOf(typeof(CastType)) != Size)
				return Marshal.PtrToStructure<CastType>(Address);
			else
				throw new InvalidCastException("Memory Size mismatched!");
		}

		/// <summary>
		/// Cast allocated memory area.
		/// </summary>
		/// <param name="ignoreOversize">If true, ignore oversize.</param>
		/// <returns></returns>
		public CastType Cast<CastType>(bool ignoreOversize)
		{
			try
			{
				return Cast<CastType>();
			}
			catch (InvalidCastException)
			{
				if (Marshal.SizeOf(typeof(CastType)) <= Size && ignoreOversize)
					return Marshal.PtrToStructure<CastType>(Address);
				else
					throw;
			}
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
				return Marshal.PtrToStructure<byte[]>(Address)[offset];
			}

			set
			{
				Marshal.PtrToStructure<byte[]>(Address)[offset] = value;
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
			if (!IsAllocated && IsFreeOnDispose)
				Marshal.FreeHGlobal(Address);
			else if (IsAllocated)
				Marshal.FreeHGlobal(ObjectAddress);
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
