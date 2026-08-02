namespace TUnit.Mixed_1._63._0;

public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;

    public double Divide(int a, int b) =>
        b == 0 ? throw new DivideByZeroException() : (double)a / b;
}
