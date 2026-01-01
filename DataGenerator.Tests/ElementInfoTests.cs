using DataGenerator.Data;
using Xunit;

namespace DataGenerator.Tests;

public class ElementInfoTests
{
    [Fact]
    public void MAX_ELEMENTS_Is19()
    {
        // 18 types + None = 19 total
        Assert.Equal(19, ElementInfo.MAX_ELEMENTS);
    }

    [Fact]
    public void Element_None_HasValue0()
    {
        Assert.Equal(0, (int)ElementInfo.Element.None);
    }

    [Fact]
    public void Element_EnumContainsAll18Types()
    {
        var elementNames = Enum.GetNames(typeof(ElementInfo.Element));

        // Should have 19 values (None + 18 types)
        Assert.Equal(19, elementNames.Length);

        // Check some key types exist
        Assert.Contains("None", elementNames);
        Assert.Contains("Fire", elementNames);
        Assert.Contains("Water", elementNames);
        Assert.Contains("Grass", elementNames);
        Assert.Contains("Electric", elementNames);
        Assert.Contains("Psychic", elementNames);
        Assert.Contains("Dragon", elementNames);
        Assert.Contains("Fairy", elementNames);
    }

    [Theory]
    [InlineData(ElementInfo.Element.None, 0)]
    [InlineData(ElementInfo.Element.Bug, 1)]
    [InlineData(ElementInfo.Element.Dark, 2)]
    [InlineData(ElementInfo.Element.Dragon, 3)]
    [InlineData(ElementInfo.Element.Electric, 4)]
    [InlineData(ElementInfo.Element.Fairy, 5)]
    [InlineData(ElementInfo.Element.Fighting, 6)]
    [InlineData(ElementInfo.Element.Fire, 7)]
    [InlineData(ElementInfo.Element.Flying, 8)]
    [InlineData(ElementInfo.Element.Ghost, 9)]
    [InlineData(ElementInfo.Element.Grass, 10)]
    [InlineData(ElementInfo.Element.Ground, 11)]
    [InlineData(ElementInfo.Element.Ice, 12)]
    [InlineData(ElementInfo.Element.Normal, 13)]
    [InlineData(ElementInfo.Element.Poison, 14)]
    [InlineData(ElementInfo.Element.Psychic, 15)]
    [InlineData(ElementInfo.Element.Rock, 16)]
    [InlineData(ElementInfo.Element.Steel, 17)]
    [InlineData(ElementInfo.Element.Water, 18)]
    public void Element_HasCorrectNumericValue(ElementInfo.Element element, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)element);
    }

    [Fact]
    public void Element_MaxValueMatchesMAX_ELEMENTS_Minus1()
    {
        // The max enum value should be MAX_ELEMENTS - 1 (since we start at 0)
        var maxEnumValue = Enum.GetValues(typeof(ElementInfo.Element)).Cast<int>().Max();
        Assert.Equal(ElementInfo.MAX_ELEMENTS - 1, maxEnumValue);
    }

    [Fact]
    public void Element_ValuesAreContiguous()
    {
        // Verify all values from 0 to MAX_ELEMENTS-1 are defined
        var values = Enum.GetValues(typeof(ElementInfo.Element)).Cast<int>().OrderBy(x => x).ToArray();

        for (int i = 0; i < ElementInfo.MAX_ELEMENTS; i++)
        {
            Assert.Contains(i, values);
        }
    }
}
