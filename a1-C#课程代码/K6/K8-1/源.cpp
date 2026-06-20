#include<iostream>
#include"student.h"
//double Add(double a, double b)
//{
//	return a + b;
//}


int main()
{
	double x = 50;
	double y = 100;
	std::cout<< "Hello,world!";
	student* pStu = new student();
	double result = pStu->Add(x, y);
	std::cout << x << "+" << y << "=" << result;
	pStu->SayHello();

	return 0;
}