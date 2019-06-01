using System;

namespace Core.Models
{
    public class BasicPropertyInfoBuild
    {
        public Func<object, object> GetValue { get; set; }
        
        public Action<object, object> SetValue { get; set; }
    }
}