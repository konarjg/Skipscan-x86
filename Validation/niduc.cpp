#include <iostream>
#include <fstream>
#include <cstdlib>
using namespace std;

int main()
{
	ofstream plik("test.s", ofstream::out|ofstream::trunc);
	plik<<"#vim: syntax=gas"<<endl;
	plik<<".globl _start"<<endl;
	plik<<".data"<<endl;
	plik<<"bajty: .byte 0x65, 0x32, 0x2c"<<endl;
	plik<<endl;
	plik<<".text"<<endl;
	plik<<"_start:"<<endl;
	plik<<"\tmov $bajty, %eax"<<endl;
	plik<<"\tmov $1, %eax"<<endl;
	plik<<"\tmov $0, %ebx"<<endl;
	plik<<"\tint $0x80"<<endl;
	plik.close();
	
	system("as -o test.o test.s");
	system("ld -o test test.o");
	system("gdb -x cmd.txt test");

	return 0;
}
