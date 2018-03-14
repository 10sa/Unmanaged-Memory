using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		/// Return allocated memory area address. This address isn't Memory insteance address.
		/// </summary>
		public IntPtr Address { get; private set; }

		/// <summary>
		/// Return this object address. This address isn't request memory address.
		/// </summary>
		public IntPtr ObjectAddress { get; private set; }

		internal static Memory Allocation(int size)
		{
			IntPtr memoryObject = Marshal.AllocHGlobal(size + Marshal.SizeOf(typeof(Memory)));
			Memory memory = (Memory)Marshal.PtrToStructure(memoryObject, typeof(Memory));
			memory.Address = IntPtr.Add(memoryObject, Marshal.SizeOf(typeof(Memory)));
			memory.ObjectAddress = memoryObject;
			memory.Size = size;

			return memory;
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

		public override int GetHashCode()
		{
			return Address.ToInt32();
		}

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
