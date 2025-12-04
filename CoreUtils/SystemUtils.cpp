// SystemUtils.cpp
// C++ DLL for system utilities - demonstrates C++/C# interoperability

#include <Windows.h>

/// <summary>
/// Returns the system uptime in seconds since last boot
/// This function is exported to be called from C# via P/Invoke
/// </summary>
/// <returns>System uptime in seconds (64-bit integer)</returns>
extern "C" __declspec(dllexport) long long GetSystemUptimeSeconds()
{
    // GetTickCount64() returns milliseconds since boot
    // Convert to seconds by dividing by 1000
    return GetTickCount64() / 1000;
}