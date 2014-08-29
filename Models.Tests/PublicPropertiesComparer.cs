using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace DrunkAudible.Models.Tests
{
    public class PublicPropertiesComparer<T>  : IComparer
        where T : class
    {
        string[] _ignore;
        Type _type;

        public PublicPropertiesComparer(params string[] ignore)
        {
            _ignore = ignore;
            _type = typeof(T);
        }

        public int Compare (object to, object self)
        {
            var typedTo = to as T;
            var typedSelf = self as T;
            if (typedSelf != null && typedTo != null) {
                Collection<string> ignoreList = new Collection<string> (_ignore);
                foreach (System.Reflection.PropertyInfo pi in _type.GetProperties(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) {
                    if (!ignoreList.Contains (pi.Name)) {
                        object selfValue = _type.GetProperty (pi.Name).GetValue (typedSelf, null);
                        object toValue = _type.GetProperty (pi.Name).GetValue (typedTo, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals (toValue))) {
                            return 1;
                        }
                    }
                }
                return 0;
            }
            if (Object.Equals(typedSelf, typedTo)) {
                return 0;
            } else {
                return 1;
            }
        }
    }
}

