# MultiTypeBinder

This library is similar to my [object-binder](https://github.com/amir734jj/object-binder) but supports multiple types for binding.

[![pipeline status](https://gitlab.com/hesamian/MultiTypeBinder/badges/master/pipeline.svg)](https://gitlab.com/hesamian/MultiTypeBinder/commits/master)


```csharp
public enum Key
{
    Name
}

public class EntityA
{
    public string Name { get; set; }
}

public class EntityB
{
    public string Name { get; set; }
}

public class MultiTypeBinderTest
{    
    [Fact]
    public void Test_Basic_Set()
    {
        // Arrange
        var a = new EntityA {Name = "A"};
        var b = new EntityB {Name = "B"};
        
        var multiTypeItems = new MultiTypeBinderBuilder<Key>()
            .WithType<EntityA>(opt1 => opt1
                .WithProperty(x => x.Name, opt2 => opt2
                    .Bind(Key.Name)
                    .WithGetter(x => x.Name)
                    .WithSetter((x, y) => x.Name = y))
                .FinalizeType())
            .WithType<EntityB>(opt1 => opt1
                .WithProperty(x => x.Name, opt2 => opt2
                    .Bind(Key.Name)
                    .WithGetter(x => x.Name)
                    .WithSetter((x, y) => x.Name = y))
                .FinalizeType())
            .Build()
            .Map(new List<object> { a, b });

        // Act
        multiTypeItems.FirstOrDefault()[Key.Name] = "updated A";
        multiTypeItems.LastOrDefault()[Key.Name] = "updated B";
        
        var v1 = multiTypeItems.FirstOrDefault()[Key.Name];
        var v2 = multiTypeItems.LastOrDefault()[Key.Name];

        // Assert
        Assert.Equal(2, multiTypeItems.Count());
        Assert.Equal("updated A", v1);
        Assert.Equal("updated B", v2);
    }
}
 ```
