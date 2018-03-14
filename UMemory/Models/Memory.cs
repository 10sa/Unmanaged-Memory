using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UMemory.Models
{
	/// <summary>
	/// Implement Unmanaged memory area.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Memory : IDisposable, ICloneable
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
		public object Cast(Type dstType)
		{
			if (Marshal.SizeOf(dstType) != Size)
				return Marshal.PtrToStructure(Address, dstType);
			else
				throw new InvalidCastException("Memory Size mismatched!");
		}

		/// <summary>
		/// Cast allocated memory area.
		/// </summary>
		/// <param name="dstType">Casting type.</param>
		/// <param name="ignoreOversize">If true, ignore oversize.</param>
		/// <returns></returns>
		public object Cast(Type dstType, bool ignoreOversize)
		{
			try
			{
				return Cast(dstType);
			}
			catch (InvalidCastException)
			{
				if (Marshal.SizeOf(dstType) <= Size && ignoreOversize)
					return Marshal.PtrToStructure(Address, dstType);
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
			return Allocation(this.Size);
		}

		public void Dispose()
		{
			Marshal.FreeHGlobal(Address);
		}
	}
}
