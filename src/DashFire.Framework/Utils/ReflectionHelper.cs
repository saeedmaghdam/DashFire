using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DashFire.Utils
{
    /// <summary>
    /// Reflection helper class.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Sets an object's property.
        /// </summary>
        /// <param name="inputObject">Object instance.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyVal">Property value.</param>
        public static void SetPropertyValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        /// <summary>
        /// Returns caller class name.
        /// </summary>
        /// <returns></returns>
        public static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }

        /// <summary>
        /// Get all parent classes.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassHierarchy(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
