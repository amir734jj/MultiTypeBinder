using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MultiTypeBinder.Interfaces
{
    public interface IMultiTypeItem<in TEnum> where TEnum : Enum
    {
        object this[TEnum key] { get; set; }
    }
    
    public interface IMultiTypeBinder<TEnum> where TEnum: Enum
    {
        List<IMultiTypeItem<TEnum>> Map(IEnumerable<object> items);
    }
    
    public interface  IMultiTypeBinderBuilder<TEnum> where TEnum: Enum
    {
        IMultiTypeBinderBuilder<TEnum> WithType<TClass>(Func<IBindTypeBuilder<TEnum, TClass>, IMultiTypeBinderBuilder<TEnum>> opt);

        IMultiTypeBinder<TEnum> Build();
    }

    public interface IBindTypeBuilder<TEnum, TClass> where TEnum : Enum
    {
        // ReSharper disable once UnusedParameter.Global
        IBindTypeBuilder<TEnum, TClass> WithProperty<TProperty>(Expression<Func<TClass, TProperty>> _, Func<IBindPropertyBuilder<TEnum, TClass, TProperty>, IVoid> opt);

        IMultiTypeBinderBuilder<TEnum> FinalizeType();
    }

    public interface IBindPropertyBuilder<in TEnum, out TClass, TProperty> where TEnum : Enum
    {
        IBindPropertyGetterBuilder<TClass, TProperty> Bind(TEnum key);
    }

    public interface IBindPropertyGetterBuilder<out TClass, TProperty>
    {
        IBindPropertySetterBuilder<TClass, TProperty> WithGetter(Func<TClass, TProperty> getter);
    }

    public interface IBindPropertySetterBuilder<out TClass, out TProperty>
    {
        IVoid WithSetter(Action<TClass, TProperty> setter);
    }

    public interface IVoid
    {
        
    }
}