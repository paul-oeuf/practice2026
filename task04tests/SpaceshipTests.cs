using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectProperties()
    {
        // Arrange
        ISpaceship cruiser = new Cruiser();

        // Assert
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectProperties()
    {
        // Arrange
        ISpaceship fighter = new Fighter();

        // Assert
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(50, fighter.FirePower);
    }

    [Fact]
    public void Cruiser_ShouldImplementInterface()
    {
        var cruiser = new Cruiser();

        Assert.IsAssignableFrom<ISpaceship>(cruiser);
    }

    [Fact]
    public void Fighter_ShouldImplementInterface()
    {
        var fighter = new Fighter();

        Assert.IsAssignableFrom<ISpaceship>(fighter);
    }

    [Fact]
    public void Cruiser_Methods_ShouldNotThrowExceptions()
    {
        // Arrange
        ISpaceship cruiser = new Cruiser();

        // Act
        var exception = Record.Exception(() =>
        {
            cruiser.MoveForward();
            cruiser.Rotate(90);
            cruiser.Fire();
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Fighter_Methods_ShouldNotThrowExceptions()
    {
        // Arrange
        ISpaceship fighter = new Fighter();

        // Act
        var exception = Record.Exception(() =>
        {
            fighter.MoveForward();
            fighter.Rotate(180);
            fighter.Fire();
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void DifferentShips_ShouldHaveDifferentCharacteristics()
    {
        // Arrange
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();

        // Assert
        Assert.NotEqual(cruiser.Speed, fighter.Speed);
        Assert.NotEqual(cruiser.FirePower, fighter.FirePower);
    }
}