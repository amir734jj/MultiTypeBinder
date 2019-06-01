using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Interfaces;
using Core.Models;

namespace Core
{
    public class MultiTypeItem<TEnum> : IMultiTypeItem<TEnum> where TEnum : Enum
    {
        private readonly Dictionary<TEnum, BasicPropertyInfoUse> _table;

        public MultiTypeItem(Dictionary<TEnum, BasicPropertyInfoUse> table)
        {
            _table = table;
        }

        public object this[TEnum key]
        {
            get => _table[key].GetValue();
            set => _table[key].SetValue(value);
        }
    }

    public class MultiTypeBinder<TEnum> : IMultiTypeBinder<TEnum> where TEnum : Enum
    {
        private readonly Dictionary<Type, Dictionary<TEnum, BasicPropertyInfoBuild>> _basicTypeInfos;

        public MultiTypeBinder(Dictionary<Type, Dictionary<TEnum, BasicPropertyInfoBuild>> basicTypeInfos)
        {
            _basicTypeInfos = basicTypeInfos;
        }

        public IEnumerable<IMultiTypeItem<TEnum>> Map(IEnumerable<object> items)
        {
            return items.Select(x =>
            {
                var key = _basicTypeInfos.Keys.First(y => y.IsInstanceOfType(x));

                var value = _basicTypeInfos[key].ToDictionary(z => z.Key, z => new BasicPropertyInfoUse
                {
                    GetValue = () => z.Value.GetValue(x),
                    SetValue = a => z.Value.SetValue(x, a)
                });
                
                return new MultiTypeItem<TEnum>(value);
            });
        }
    }

    public class MultiTypeBinderBuilder<TEnum> : IMultiTypeBinderBuilder<TEnum> where TEnum : Enum
    {
        public readonly Dictionary<Type, Dictionary<TEnum, BasicPropertyInfoBuild>> BasicTypeInfos;

        public MultiTypeBinderBuilder()
        {
            BasicTypeInfos = new Dictionary<Type, Dictionary<TEnum, BasicPropertyInfoBuild>>();
        }
        
        public IMultiTypeBinderBuilder<TEnum> WithType<TClass>(
            Func<IBindTypeBuilder<TEnum, TClass>, IMultiTypeBinderBuilder<TEnum>> opt)
        {
            return opt(new BindTypeBuilder<TEnum, TClass>(this));
        }

        public IMultiTypeBinder<TEnum> Build()
        {
            return new MultiTypeBinder<TEnum>(BasicTypeInfos);
        }
    }

    public class BindTypeBuilder<TEnum, TClass> : IBindTypeBuilder<TEnum, TClass> where TEnum : Enum
    {
        public readonly Dictionary<TEnum, BasicPropertyInfoBuild> BasicPropertyInfos;

        private readonly MultiTypeBinderBuilder<TEnum> _multiTypeBinderBuilder;

        public BindTypeBuilder(MultiTypeBinderBuilder<TEnum> multiTypeBinderBuilder)
        {
            _multiTypeBinderBuilder = multiTypeBinderBuilder;
            BasicPropertyInfos = new Dictionary<TEnum, BasicPropertyInfoBuild>();
        }

        public IBindTypeBuilder<TEnum, TClass> WithProperty<TProperty>(Expression<Func<TClass, TProperty>> property,
            Func<IBindPropertyBuilder<TEnum, TClass, TProperty>, IBindTypeBuilder<TEnum, TClass>> opt)
        {
            return opt(new BindPropertyBuilder<TEnum, TClass, TProperty>(this));
        }

        public IMultiTypeBinderBuilder<TEnum> FinalizeType()
        {
            _multiTypeBinderBuilder.BasicTypeInfos[typeof(TClass)] = BasicPropertyInfos;

            return _multiTypeBinderBuilder;
        }
    }

    public class BindPropertyBuilder<TEnum, TClass, TProperty> : IBindPropertyBuilder<TEnum, TClass, TProperty>
        where TEnum : Enum
    {
        private readonly BindTypeBuilder<TEnum, TClass> _bindTypeBuilder;

        public BindPropertyBuilder(BindTypeBuilder<TEnum, TClass> bindTypeBuilder)
        {
            _bindTypeBuilder = bindTypeBuilder;
        }

        public IBindPropertyGetterBuilder<TEnum, TClass, TProperty> Bind(TEnum key)
        {
            return new BindPropertyGetterBuilder<TEnum, TClass, TProperty>(_bindTypeBuilder, key);
        }
    }

    public class
        BindPropertyGetterBuilder<TEnum, TClass, TProperty> : IBindPropertyGetterBuilder<TEnum, TClass, TProperty>
        where TEnum : Enum
    {
        private readonly BindTypeBuilder<TEnum, TClass> _bindTypeBuilder;

        private readonly TEnum _key;

        public BindPropertyGetterBuilder(BindTypeBuilder<TEnum, TClass> bindTypeBuilder, TEnum key)
        {
            _bindTypeBuilder = bindTypeBuilder;
            _key = key;
        }

        public IBindPropertySetterBuilder<TEnum, TClass, TProperty> WithGetter(Func<TClass, TProperty> getter)
        {
            return new BindPropertySetterBuilder<TEnum, TClass, TProperty>(_bindTypeBuilder, _key, getter);
        }
    }

    public class
        BindPropertySetterBuilder<TEnum, TClass, TProperty> : IBindPropertySetterBuilder<TEnum, TClass, TProperty>
        where TEnum : Enum
    {
        private readonly BindTypeBuilder<TEnum, TClass> _bindTypeBuilder;

        private readonly TEnum _key;

        private readonly Func<TClass, TProperty> _getter;

        public BindPropertySetterBuilder(BindTypeBuilder<TEnum, TClass> bindTypeBuilder, TEnum key,
            Func<TClass, TProperty> getter)
        {
            _bindTypeBuilder = bindTypeBuilder;
            _key = key;
            _getter = getter;
        }

        public IBindTypeBuilder<TEnum, TClass> WithSetter(Action<TClass, TProperty> setter)
        {
            _bindTypeBuilder.BasicPropertyInfos[_key] = new BasicPropertyInfoBuild
            {
                GetValue = instance =>
                {
                    switch (instance)
                    {
                        case TClass classInstance:
                            return _getter(classInstance);
                        default:
                            throw new Exception();
                    }
                },
                SetValue = (instance, value) =>
                {
                    switch (instance)
                    {
                        case TClass classInstance:
                            switch (value)
                            {
                                case TProperty propertyValue:
                                    setter(classInstance, propertyValue);
                                    break;
                                default:
                                    throw new Exception();
                            }

                            break;
                        default:
                            throw new Exception();
                    }
                }
            };

            return _bindTypeBuilder;
        }
    }
}