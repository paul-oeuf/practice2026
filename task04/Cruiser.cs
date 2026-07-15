namespace task04;

public class Cruiser : ISpaceship
{
    public int Speed => 50;

    public int FirePower => 100;


    public void MoveForward()
    {
        Console.WriteLine("Cruiser moves forward");
    }


    public void Rotate(int angle)
    {
        Console.WriteLine($"Cruiser rotates {angle} degrees");
    }


    public void Fire()
    {
        Console.WriteLine("Cruiser fires powerful photon missile");
    }
}