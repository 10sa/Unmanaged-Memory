using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMemory.Models;
using System.Runtime.InteropServices;
using System.Collections;

namespace UMemory
{
    public static class UMemory
	{
		/// <summary>
		/// Allocate unmanaged memory areas.
		/// </summary>
		/// <param name="size">Size of memory area.</param>
		/// <returns>Allocated memory area. This object isn't garabge collectable.</returns>
		public static Memory Allocation(int size)
		{
			return Memory.Allocation(size);
		}

		/// <summary>
		/// Allocate unmanaged memory areas.
		/// </summary>
		/// <param name="size">Size of memory area.</param>
		/// <param name="collectable">Whether the Memory object itself is garbage collectable.Even though this value is true, the memory area pointed to by the Memory instance is unmanaged memory.</param>
		/// <returns></returns>
		public static Memory Allocation(int size, bool collectable)
		{
			return Memory.Allocation(size, collectable);
		}

		public static void Memcpy(Memory src, Memory dst, int length)
		{
			Marshal.Copy(Marshal.PtrToStructure<byte[]>(src.Address), 0, src.Address, length);
		}
	}
}
