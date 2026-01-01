using DataGenerator;
using Xunit;

namespace DataGenerator.Tests;

public class GenPathTests
{
    [Fact]
    public void DATA_GEN_PATH_HasDefaultValue()
    {
        // Verify the default path is set
        Assert.NotNull(GenPath.DATA_GEN_PATH);
        Assert.Equal("DataAsset/", GenPath.DATA_GEN_PATH);
    }

    [Fact]
    public void TL_PATH_CombinesWithDataGenPath()
    {
        // Store original and test with default
        string original = GenPath.DATA_GEN_PATH;
        try
        {
            GenPath.DATA_GEN_PATH = "DataAsset/";
            Assert.Equal("DataAsset/String/", GenPath.TL_PATH);
        }
        finally
        {
            GenPath.DATA_GEN_PATH = original;
        }
    }

    [Fact]
    public void ITEM_PATH_CombinesWithDataGenPath()
    {
        string original = GenPath.DATA_GEN_PATH;
        try
        {
            GenPath.DATA_GEN_PATH = "DataAsset/";
            Assert.Equal("DataAsset/Item/", GenPath.ITEM_PATH);
        }
        finally
        {
            GenPath.DATA_GEN_PATH = original;
        }
    }

    [Fact]
    public void MONSTER_PATH_CombinesWithDataGenPath()
    {
        string original = GenPath.DATA_GEN_PATH;
        try
        {
            GenPath.DATA_GEN_PATH = "DataAsset/";
            Assert.Equal("DataAsset/Monster/", GenPath.MONSTER_PATH);
        }
        finally
        {
            GenPath.DATA_GEN_PATH = original;
        }
    }

    [Fact]
    public void ZONE_PATH_CombinesWithDataGenPath()
    {
        string original = GenPath.DATA_GEN_PATH;
        try
        {
            GenPath.DATA_GEN_PATH = "DataAsset/";
            Assert.Equal("DataAsset/Zone/", GenPath.ZONE_PATH);
        }
        finally
        {
            GenPath.DATA_GEN_PATH = original;
        }
    }

    [Fact]
    public void Paths_UpdateWhenDataGenPathChanges()
    {
        string original = GenPath.DATA_GEN_PATH;
        try
        {
            GenPath.DATA_GEN_PATH = "CustomPath/";

            Assert.Equal("CustomPath/String/", GenPath.TL_PATH);
            Assert.Equal("CustomPath/Item/", GenPath.ITEM_PATH);
            Assert.Equal("CustomPath/Monster/", GenPath.MONSTER_PATH);
            Assert.Equal("CustomPath/Zone/", GenPath.ZONE_PATH);
        }
        finally
        {
            GenPath.DATA_GEN_PATH = original;
        }
    }
}
