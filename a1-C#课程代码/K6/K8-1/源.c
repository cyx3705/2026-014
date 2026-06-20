#include<stdio.h>

//Function fun
double Add(double a, double b)
{
	return a + b;
}

int main()
{
	printf("Hello world!");
	double x = 30;
	double y = 50;
	double result = Add(x, y);
	printf("%f+%f=%f", x, y, result);
	return 0;
	

}