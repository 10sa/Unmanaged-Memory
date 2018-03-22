# Unmanaged Memory
This library provides memory that can not be collected by the CLR gc and Memory heap allocation.


Memory areas allocated using this library are not collected by gc and must be released by the programmer. **A memory leak occurs when all references are disconnected without being released.**


This library also uses WinAPI. **It doesn't work on non-Windows platforms.**


For the WinAPI used in this library, please refer to the "List of Used WinAPIs" below.

# Examples
```
/// Allocation Unmanaged memory
Memory memroy = Memory.Allocation(4); // 4Bytes Memory allocation.
```

```
/// Access Memory
byte data = memory[0];
memory[0] = 2;

byte[] datas = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF};
memory.WriteBytes(datas, 0, datas.Length);
```

```
/// Release Memory
memory.Free();
```

```
/// Since VS2017, C++ Version Memory release
delete memory; // memory : Address (in C#, IntPtr type variable.)
```

```
/// Dispose Free
using (Memory memory = Memory.Allocation(4))
{
  memory.IsFreeOnDispose = true; // This code will be make automatic memory release.
  ...
}
```

```
/// Allocation Memory heap and Use.
Heap memoryHeap = Heap.CreateHeap();
Memory memory = Memory.Allocation(memoryHeap, 8); // 8Bytes Memory allocation on "memoryHeap" Heap.
```

```
/// Destory Memory heap
memoryHeap.Destory();
```

# List of Used WinAPIs
   ## List of Data types
  - HeapFlags : A 32-bit unsigned integer, Bit Flags.
  - IntPtr : Variable. (x32, 4Byte / x64, 8Byte)
  
   ## IntPtr GetProcessHeap()
  - Dll : Kernel32.dll
  - Symbol : GetProcessHeap
  - MSDN Link : https://msdn.microsoft.com/en-us/library/windows/desktop/aa366569(v=vs.85).aspx
  
   ## uint GetProcessHeaps(uint, IntPtr[])
  - Dll : Kernel32.dll
  - Symbol : GetProcessHeaps
  - MSDN Link : https://msdn.microsoft.com/en-us/library/windows/desktop/aa366571(v=vs.85).aspx
  
   ## IntPtr HeapAlloc(IntPtr, HeapFlags, uint)
  - Dll : Kernel32.dll
  - Symbol : HeapAlloc
  - MSDN Link : https://msdn.microsoft.com/en-us/library/windows/desktop/aa366597(v=vs.85).aspx
    
   ## IntPtr HeapFree(IntPtr, HeapFlags, IntPtr);
  - Dll : Kernel32.dll
  - Symbol : HeapFree
  - MSDN Link : https://msdn.microsoft.com/en-us/library/windows/desktop/aa366701(v=vs.85).aspx

   ## IntPtr CopyMemory(IntPtr, IntPtr, uint)
  - Dll : Kernel32.dll
  - Symbol : CopyMemory
  - MSDN Link : https://msdn.microsoft.com/ko-kr/library/windows/desktop/aa366535(v=vs.85).aspx

   ## IntPtr CreateHeap(HeapFlags, uint, uint)
  - Dll : Kernel32.dll
  - Symbol : CreateHeap
  - MSDN Link : https://msdn.microsoft.com/ko-kr/library/windows/desktop/aa366599(v=vs.85).aspx
