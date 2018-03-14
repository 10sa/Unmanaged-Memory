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
	public class Memory : IDisposable
	{
		private Memory() { }

		public int Size { get; private set; }

		public IntPtr Address { get; private set; }
		public byte[] Bytes { get; set; }

		internal static Memory Allocation(int size)
		{
			IntPtr memoryObject = Marshal.AllocHGlobal(size + Marshal.SizeOf(typeof(Memory)));
			
			return (Memory)Marshal.PtrToStructure(memoryObject, typeof(Memory));
		}

		/// <summary>
		/// Dispose unmanaged memory area.
		/// </summary>
		public void Delete()
		{
			Marshal.FreeHGlobal(Address);
			Address = IntPtr.Zero;
		}

		public bool Deleted { get
			{
				return Address == IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			Delete();
		}
	}
}
