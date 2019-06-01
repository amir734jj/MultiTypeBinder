using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IMultiTypeItem<in TEnum> where TEnum : Enum
    {
        object this[TEnum key] { get; set; }
    }
    
    public interface IMultiTypeBinder<in TEnum> where TEnum: Enum
    {
        IEnumerable<IMultiTypeItem<TEnum>> Map(IEnumerable<object> items);
    }
    
    public interface  IMultiTypeBinderBuilder<TEnum> where TEnum: Enum
    {
        IMultiTypeBinderBuilder<TEnum> WithType<TClass>(Func<IBindTypeBuilder<TEnum, TClass>, IMultiTypeBinderBuilder<TEnum>> opt);

        IMultiTypeBinder<TEnum> Build();
    }

    public interface IBindTypeBuilder<TEnum, TClass> where TEnum : Enum
    {
        IBindTypeBuilder<TEnum, TClass> WithProperty<TProperty>(Expression<Func<TClass, TProperty>> property, Func<IBindPropertyBuilder<TEnum, TClass, TProperty>, IBindTypeBuilder<TEnum, TClass>> opt);

        IMultiTypeBinderBuilder<TEnum> FinalizeType();
    }

    public interface IBindPropertyBuilder<TEnum, TClass, TProperty> where TEnum : Enum
    {
        IBindPropertyGetterBuilder<TEnum, TClass, TProperty> Bind(TEnum key);
    }

    public interface IBindPropertyGetterBuilder<TEnum, TClass, TProperty> where TEnum : Enum
    {
        IBindPropertySetterBuilder<TEnum, TClass, TProperty> WithGetter(Func<TClass, TProperty> getter);
    }

    public interface IBindPropertySetterBuilder<TEnum, TClass, out TProperty> where TEnum : Enum
    {
        IBindTypeBuilder<TEnum, TClass> WithSetter(Action<TClass, TProperty> setter);
    }
}