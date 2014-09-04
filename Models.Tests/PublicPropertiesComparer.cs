// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;

namespace DrunkAudible.Models.Tests
{
    public class PublicPropertiesComparer<T> : IComparer
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
                var ignoreList = new Collection<string> (_ignore);
                foreach (PropertyInfo propertyInfo in _type.GetProperties(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) {
                    if (!ignoreList.Contains (propertyInfo.Name)) {
                        var selfValue = _type.GetProperty (propertyInfo.Name).GetValue (typedSelf, null);
                        var toValue = _type.GetProperty (propertyInfo.Name).GetValue (typedTo, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals (toValue))) {
                            throw new AssertionException (propertyInfo.Name + "is not equal.");
                        }
                    }
                }
                return 0;
            }
            if (Object.Equals(typedSelf, typedTo)) {
                return 0;
            } else {
                throw new AssertionException ("One reference is null but the other is not.");
            }
        }
    }
}

