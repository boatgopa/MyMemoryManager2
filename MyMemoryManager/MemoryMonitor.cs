using System;
using System.Collections.Generic;
using System.Text;

namespace MyMemoryManager {
    internal class MemoryMonitor {
        private MemoryManager memory_manager;
        private int memory_size;
        private int alloc_type = 0; // 1이면 first fit, 2면 best fit.

        // 할당된 메모리의 이름, 주소, 크기
        private List<(string name, int address, int size)> allocs
            = new List<(string name, int address, int size)>();

        public MemoryMonitor(int memory_size) {
            this.memory_size = memory_size;
            memory_manager = new MemoryManager(memory_size);
        }

        public void Run() {

            while (true) {
                Console.WriteLine("메모리 할당 방식을 선택해주세요.");
                Console.WriteLine("1 : First-Fit");
                Console.WriteLine("2 : Best-Fit");
                Console.Write("선택: ");

                string input = Console.ReadLine();

                if (input == "1") {
                    alloc_type = 1;
                    break;
                } 
                
                else if (input == "2") {
                    alloc_type = 2;
                    break;
                } 
                
                else {
                    Console.WriteLine("\n잘못된 입력입니다.\n");
                }
            }

            PrintMemory();

            while (true) {
                Console.WriteLine("할당하려면 1, 해제하려면 2, 종료하려면 0을 입력하세요. ");
                Console.Write("입력: ");

                string command = Console.ReadLine();

                if (command == "1") {
                    Alloc();
                } 
                
                else if (command == "2") {
                    Free();
                } 
                
                else if (command == "0") {
                    Console.WriteLine("\n프로그램을 종료합니다.");
                    break;
                } 
                
                else {
                    Console.WriteLine("\n잘못된 입력입니다.\n");
                }
            }
        }

        // 메모리 할당
        private void Alloc() {
            Console.WriteLine();
            Console.WriteLine("할당할 이름과 크기를 입력해주세요. (형식: a, 30)");
            Console.Write("입력: ");

            string input = Console.ReadLine();

            string[] data = input.Split(',');

            if (data.Length != 2 || !int.TryParse(data[1].Trim(), out int size)) {
                Console.WriteLine("\n입력 형식이 잘못되었습니다.\n");
                return;
            }

            string name = data[0].Trim();

            // 이름이 중복이면
            for (int i = 0; i < allocs.Count; i++) {
                if (allocs[i].name == name) {
                    Console.WriteLine("\n이미 사용 중인 이름입니다.\n");
                    return;
                }
            }

            int address;

            if (alloc_type == 1) {
                address = memory_manager.Myalloc(size);
            } 
            
            else {
                address = memory_manager.MyallocB(size);
            }

            // 메모리가 부족하면
            if (address == -1) {
                Console.WriteLine("\n메모리가 부족합니다.\n");
                return;
            }

            allocs.Add((name, address, size));

            PrintMemory();

            Console.WriteLine("\n메모리 할당이 완료되었습니다.\n");
        }

        // 메모리 해제
        private void Free() {

            Console.WriteLine();
            Console.WriteLine("해제할 메모리의 이름을 입력해주세요.");
            Console.Write("입력: ");

            string name = Console.ReadLine()?.Trim();

            for (int i = 0; i < allocs.Count; i++) {
                if (allocs[i].name == name) {
                    memory_manager.Myfree(allocs[i].address);
                    allocs.RemoveAt(i);

                    PrintMemory();
                    Console.WriteLine("\n메모리 해제가 완료되었습니다.\n");
                    return;
                }
            }

            Console.WriteLine("\n존재하지 않는 이름입니다.\n");
        }

        // 메모리 상태 출력
        private void PrintMemory() {

            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("                현재 메모리 상태");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            allocs.Sort((a, b) => a.address.CompareTo(b.address));

            int current = 0;

            for (int i = 0; i < allocs.Count; i++) {
                int address = allocs[i].address;
                int size = allocs[i].size;

                int front = address - 8;
                int back = address + size;

                // 앞쪽의 빈 공간
                int space = front - current;

                if (space >= 8) {
                    PaintMemory("빈 공간", space - 8, '|');
                }

                // 뒤에 남은 공간
                int remain;

                if (i + 1 < allocs.Count) {
                    remain = allocs[i + 1].address - 8 - back;
                } else {
                    remain = memory_size - back;
                }

                // 남은 용량이 헤더를 만들 수 없거나 딱 맞을때에 합치기
                if (remain > 0 && remain <= 8) {
                    size += remain;
                    back += remain;
                }

                PaintMemory(allocs[i].name, size, '█');

                current = back;
            }

            // 마지막 빈 공간
            int last_space = memory_size - current;

            if (last_space > 8) {
                PaintMemory("빈 공간", last_space - 8, '|');
            }

            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine();
        }

        // 메모리 그리기
        private void PaintMemory(string name, int size, char painting) {
            if (name == "빈 공간") {
                Console.WriteLine("[빈 공간 : " + size + " bytes]");
            } 
            
            else {
                Console.Write("[");

                for (int i = 0; i < 2; i++) {
                    Console.Write(painting);
                }

                Console.Write(" 헤더 : 8 bytes ");

                for (int i = 0; i < 2; i++) {
                    Console.Write(painting);
                }

                Console.Write("] [");

                int count = size / (memory_size / 20);

                if (count < 1) {
                    count = 1;
                }

                for (int i = 0; i < count; i++) {
                    Console.Write(painting);
                }

                Console.Write(" " + name + " : " + size + " bytes ");

                for (int i = 0; i < count; i++) {
                    Console.Write(painting);
                }

                Console.WriteLine("]");
            }
        }
    }
}
