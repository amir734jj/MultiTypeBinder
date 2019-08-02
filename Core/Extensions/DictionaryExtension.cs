using System.Collections.Generic;

namespace MultiTypeBinder.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        ///     Get dictionary value or else
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="elseValue"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetOrElse<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue elseValue)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : elseValue;
        }
    }
}