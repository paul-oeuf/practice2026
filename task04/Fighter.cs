namespace task04;

public class Fighter : ISpaceship
{
    public int Speed => 100;

    public int FirePower => 50;


    public void MoveForward()
    {
        Console.WriteLine("Fighter moves forward");
    }


    public void Rotate(int angle)
    {
        Console.WriteLine($"Fighter rotates {angle} degrees");
    }


    public void Fire()
    {
        Console.WriteLine("Fighter fires photon missile");
    }
}