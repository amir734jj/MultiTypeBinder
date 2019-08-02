using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MultiTypeBinder.Tests
{
    public enum Key
    {
        Name, RandomKey
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
        public void Test__Get()
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
                .Map(new List<object> {a, b});

            // Act
            var v1 = multiTypeItems.First()[Key.Name];
            var v2 = multiTypeItems.Last()[Key.Name];

            // Assert
            Assert.Equal(2, multiTypeItems.Count);
            Assert.Equal("A", v1);
            Assert.Equal("B", v2);
        }

        [Fact]
        public void Test__Set()
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
                .Map(new List<object> {a, b});

            // Act
            multiTypeItems.First()[Key.Name] = "updated A";
            multiTypeItems.Last()[Key.Name] = "updated B";

            var v1 = multiTypeItems.First()[Key.Name];
            var v2 = multiTypeItems.Last()[Key.Name];

            // Assert
            Assert.Equal(2, multiTypeItems.Count);
            Assert.Equal("updated A", v1);
            Assert.Equal("updated B", v2);
        }

        [Fact]
        public void Test__Get_Fail()
        {
            // Arrange
            var source = new EntityB {Name = "A"};
            
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
                .Map(new List<object> {source});

            // Act, Assert
            Assert.Throws<Exception>(() => multiTypeItems.First()[Key.RandomKey]);
        }

        [Fact]
        public void Test__Set_Fail()
        {
            // Arrange
            var source = new EntityA {Name = "A"};

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
                .Map(new List<object> {source});

            // Act, Assert
            Assert.Throws<InvalidCastException>(() => multiTypeItems.First()[Key.Name] = 123);
        }
    }
}