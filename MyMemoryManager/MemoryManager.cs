using System;
using System.Collections.Generic;
using System.Text;

namespace MyMemoryManager {
    internal class MemoryManager {
        private int[] memory;
        int header = 0;

        public MemoryManager(int memory_size) {
            memory = new int[memory_size];

            memory[0] = memory_size - 8;

            memory[1] = -1;
        }

        public int Myalloc(int request_size) {
            int address = -1;
            int prev = -1;
            int ptr = header;

            while (ptr != -1) {
                // 헤더를 만들 여유 공간이 넘치면
                if (memory[ptr] > request_size + 8) {
                    address = ptr + 8;

                    int new_ptr = ptr + request_size + 8;

                    memory[new_ptr] = memory[ptr] - request_size - 8;
                    memory[new_ptr + 1] = memory[ptr + 1];

                    if (prev == -1) {
                        header = new_ptr;
                    } else {
                        memory[prev + 1] = new_ptr;
                    }

                    memory[ptr] = request_size;
                    break;
                }

                // 헤더를 만들 여유 공간을 포함한 공간과 같거나 적으면
                else if (memory[ptr] >= request_size) {
                    // 전체 다 할당.
                    address = ptr + 8;

                    if (prev == -1) {
                        header = memory[ptr + 1];
                    } else {
                        memory[prev + 1] = memory[ptr + 1];
                    }

                    break;
                }

                // 요구 공간보다 여유 공간이 부족한 경우, 다음 블록으로 이동 시도.
                else {
                    prev = ptr;
                    ptr = memory[ptr + 1];
                }
            }

            return address; // 할당 성공 시 주소 반환, 할당 실패 시 -1 반환.
        }

        public int MyallocB(int request_size) {
            int address = -1;

            int best_prev = -1;
            int best_ptr = -1;

            int min_space = int.MaxValue;

            int prev = -1;
            int ptr = header;

            // 가장 적합한 빈 공간 찾기
            while (ptr != -1) {

                if (memory[ptr] >= request_size) {
                    int remain = memory[ptr] - request_size;

                    // 남은 공간이 현재까지의 최소 공간보다 작으면 갱신
                    if (remain < min_space) {
                        min_space = remain;
                        best_ptr = ptr;
                        best_prev = prev;
                    }
                }

                prev = ptr;
                ptr = memory[ptr + 1];
            }

            // 적합한 공간이 존재하면 할당
            if (best_ptr != -1) {
                address = best_ptr + 8;

                // 헤더를 만들 여유 공간이 있으면 분할
                if (memory[best_ptr] > request_size + 8) {

                    int new_ptr = best_ptr + request_size + 8;

                    memory[new_ptr] = memory[best_ptr] - request_size - 8;
                    memory[new_ptr + 1] = memory[best_ptr + 1];

                    if (best_prev == -1) {
                        header = new_ptr;
                    } 
                    
                    else {
                        memory[best_prev + 1] = new_ptr;
                    }

                    memory[best_ptr] = request_size;
                }

                // 전체 공간을 할당
                else {
                    if (best_prev == -1) {
                        header = memory[best_ptr + 1];
                    } 
                    
                    else {
                        memory[best_prev + 1] = memory[best_ptr + 1];
                    }
                }
            }

            return address;
        }

        public void Myfree(int start_address) {

            ///////////////////////// 메모리 해제 로직
            int ptr = start_address - 8;

            memory[ptr + 1] = header;

            header = ptr;
            ///////////////////////// 여기까지

            int left_space = -1; //왼쪽 빈공간 주소
            int right_space = -1; //오른쪽 빈공간 주소

            int prev = ptr; // 이전 헤더 위치
            int right_prev = -1; // 오른쪽 헤더 위치


            int current = header; // 현재 헤더 위치

            while (current != -1) {
                if (current != ptr) {
                    //오른쪽부터
                    if (current == ptr + memory[ptr] + 8) {
                        right_space = current;
                        right_prev = prev;
                    }

                    //이번엔 왼쪽
                    if (ptr == current + memory[current] + 8) {
                        left_space = current;
                    }
                }

                prev = current;
                current = memory[current + 1];
            }
            // 왼쪽 오른쪽을 다 구했으므로 병합

            if (right_space != -1) {

                // 오른쪽 공간 크기 + 헤더 크기를 현재 공간 크기에 더하기
                memory[ptr] += memory[right_space] + 8;
                // 오른쪽 공간의 이전 주소를 연결
                memory[right_prev + 1] = memory[right_space + 1];
            }

            if (left_space != -1) {

                // 현재 공간 크기 + 헤더 크기를 왼쪽 공간 크기에 더하기
                memory[left_space] += memory[ptr] + 8;
                // 기존 헤더 부분이 왼쪽 공간에 합쳐졌으므로, 헤더가 기존 헤더의 다음 주소를 가리키도록 변경
                header = memory[ptr + 1];
            }
        }
    }

}
