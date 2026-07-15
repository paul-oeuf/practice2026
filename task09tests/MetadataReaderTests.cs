using task09;

namespace task09tests;

public class MetadataReaderTests
{
    [Fact]
    public void MetadataReader_ShouldReadAssembly()
    {
        string dllPath =
            typeof(MetadataReader)
            .Assembly
            .Location;


        var reader = new MetadataReader();


        string result =
            reader.ReadMetadata(dllPath);


        Assert.Contains(
            "Library:",
            result);
    }



    [Fact]
    public void MetadataReader_ShouldThrowForMissingFile()
    {
        var reader = new MetadataReader();


        Assert.Throws<FileNotFoundException>(
            () =>
            reader.ReadMetadata(
                "unknown.dll"));
    }



    [Fact]
    public void MetadataReader_ShouldReadProperties()
    {
        string dllPath =
            typeof(MetadataReader)
            .Assembly
            .Location;


        var reader = new MetadataReader();


        string result =
            reader.ReadMetadata(dllPath);


        Assert.Contains(
            "Properties:",
            result);
    }
}