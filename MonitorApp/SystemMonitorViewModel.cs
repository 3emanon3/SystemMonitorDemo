using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace MonitorApp
{
    public class SystemMonitorViewModel : INotifyPropertyChanged
    {
        private float _cpuUsage;
        private float _memoryUsage;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memoryCounter;
        private DispatcherTimer _timer;

        public float CpuUsage
        {
            get => _cpuUsage;
            set
            {
                _cpuUsage = value;
                OnPropertyChanged(nameof(CpuUsage));
            }
        }

        public float MemoryUsage
        {
            get => _memoryUsage;
            set
            {
                _memoryUsage = value;
                OnPropertyChanged(nameof(MemoryUsage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SystemMonitorViewModel()
        {
            // 初始化性能计数器
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

            // 创建定时器，每秒更新一次
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateData;
            _timer.Start();
        }

        private void UpdateData(object sender, EventArgs e)
        {
            CpuUsage = _cpuCounter.NextValue();
            MemoryUsage = _memoryCounter.NextValue();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}