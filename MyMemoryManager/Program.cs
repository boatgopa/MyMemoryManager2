using System;

namespace MyMemoryManager {
    internal class Program {
        static void Main(string[] args) {

            int memory_size;

            while (true) {
                Console.Write("초기 메모리 크기를 입력해주세요: ");

                if (int.TryParse(Console.ReadLine(), out memory_size) && memory_size > 8) {
                    break;
                }

                Console.WriteLine("\n잘못된 메모리 크기입니다. 8보다 큰 정수를 입력해주세요.\n");
            }

            Console.WriteLine("\n메모리 관리자를 시작합니다.\n");

            MemoryMonitor monitor = new MemoryMonitor(memory_size);

            monitor.Run();
        }
    }
}
