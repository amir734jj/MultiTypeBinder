using System;

namespace Core.Models
{
    public class BasicPropertyInfoUse
    {
        public Func<object> GetValue { get; set; }
        
        public Action<object> SetValue { get; set; }
    }
}