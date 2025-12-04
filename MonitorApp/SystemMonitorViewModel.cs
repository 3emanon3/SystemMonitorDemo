using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace MonitorApp
{
    /// <summary>
    /// ViewModel for system resource monitoring
    /// Implements INotifyPropertyChanged for data binding
    /// </summary>
    public class SystemMonitorViewModel : INotifyPropertyChanged
    {
        private float _cpuUsage;
        private float _memoryUsage;
        private string _uptime;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memoryCounter;
        private DispatcherTimer _timer;

        // Import C++ DLL function
        [DllImport("CoreUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern long GetSystemUptimeSeconds();

        /// <summary>
        /// CPU usage percentage (0-100)
        /// </summary>
        public float CpuUsage
        {
            get => _cpuUsage;
            set
            {
                _cpuUsage = value;
                OnPropertyChanged(nameof(CpuUsage));
            }
        }

        /// <summary>
        /// Memory usage percentage (0-100)
        /// </summary>
        public float MemoryUsage
        {
            get => _memoryUsage;
            set
            {
                _memoryUsage = value;
                OnPropertyChanged(nameof(MemoryUsage));
            }
        }

        /// <summary>
        /// System uptime string (formatted as "X hours Y minutes")
        /// </summary>
        public string Uptime
        {
            get => _uptime;
            set
            {
                _uptime = value;
                OnPropertyChanged(nameof(Uptime));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor: Initializes performance counters and timer
        /// </summary>
        public SystemMonitorViewModel()
        {
            // Initialize performance counters for CPU and memory
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

            // Create timer to update data every second
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateData;
            _timer.Start();

            // Initial update
            UpdateData(null, null);
        }

        /// <summary>
        /// Updates CPU, memory, and uptime data
        /// Called every second by the timer
        /// </summary>
        private async void UpdateData(object sender, EventArgs e)
        {
            var result = await Task.Run(() =>
            {
                float cpu = _cpuCounter.NextValue();
                float mem = _memoryCounter.NextValue();
                
                string uptimeStr = "";
                try
                {
                    long seconds = GetSystemUptimeSeconds();
                    TimeSpan ts = TimeSpan.FromSeconds(seconds);
                    uptimeStr = $"{ts.TotalHours} hours {ts.Minutes} minutes";
                }catch (Exception ex)
                {
                    uptimeStr = $"Error: {ex.Message}";
                }

                return new { Cpu = cpu, Mem = mem, Uptime = uptimeStr };
            });
        }

        /// <summary>
        /// Notifies UI of property changes
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}