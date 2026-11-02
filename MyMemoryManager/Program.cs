using System;

namespace MyMemoryManager {
    internal class Program {
        static void Main(string[] args) {
            MemoryMonitor monitor = new MemoryMonitor();

            monitor.Run();
        }
    }
}
