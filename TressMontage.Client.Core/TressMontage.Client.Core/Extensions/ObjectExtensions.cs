using System;
namespace TressMontage.Client.Core.Extensions
{
public static class ObjectExtensions
    {
        /// <summary>
        /// Throws ArgumentNullException if parameter is null.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName">Parameter name used for exception message.</param>
        public static T ThrowIfParameterIsNull<T>(this T parameter, string parameterName) where T : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return parameter;
        }

        /// <summary>
        /// Throws NullReferenceException if object is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        public static T ThrowIfObjectIsNull<T>(this T obj, string objectName, string message = null)
        {
            if (obj == null)
            {
                string msg = string.Empty;

                if (!string.IsNullOrWhiteSpace(message))
                {
                    msg = " " + message;
                }

                throw new NullReferenceException(objectName + " is null." + msg);
            }
            return obj;
        }
    }
}
