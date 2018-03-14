using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMemory.Models;

namespace UMemory
{
    public static class UMemory
	{
		/// <summary>
		/// Allocation Unmanaged memory area.
		/// </summary>
		/// <param name="size">Size of memory area.</param>
		/// <returns>Allocated memory area. This object isn't garabge collectable.</returns>
		public static Memory Allocation(int size)
		{
			return Memory.Allocation(size);
		}

		public static void Memcpy(Memory src, Memory dst, int length)
		{
			Array.Copy(src.Bytes, dst.Bytes, length);
		}
	}
}
