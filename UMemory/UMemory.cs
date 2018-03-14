using UMemory.Models;
using System.Runtime.InteropServices;

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

		public static void Memcpy(Memory src, Memory dst, int length)
		{
			Marshal.Copy(Marshal.PtrToStructure<byte[]>(src.Address), 0, src.Address, length);
		}
	}
}
