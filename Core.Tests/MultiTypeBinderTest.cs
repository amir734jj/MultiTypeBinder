using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Xunit;

namespace Core.Tests
{
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
        public void Test_Basic_Get()
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
            var v1 = multiTypeItems.FirstOrDefault()[Key.Name];
            var v2 = multiTypeItems.LastOrDefault()[Key.Name];

            // Assert
            Assert.Equal(2, multiTypeItems.Count());
            Assert.Equal("A", v1);
            Assert.Equal("B", v2);
        }
        
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
}