using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{

    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();

        Assert.Equal(50, cruiser.Speed);

        Assert.Equal(100, cruiser.FirePower);
    }


    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();

        Assert.Equal(100, fighter.Speed);

        Assert.Equal(50, fighter.FirePower);
    }


    [Fact]
    public void Ships_ShouldImplementInterface()
    {
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();


        Assert.IsAssignableFrom<ISpaceship>(cruiser);

        Assert.IsAssignableFrom<ISpaceship>(fighter);
    }


    [Fact]
    public void ShipMethods_ShouldExecute()
    {
        ISpaceship ship = new Fighter();


        ship.MoveForward();

        ship.Rotate(90);

        ship.Fire();
    }
}