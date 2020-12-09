using System;
using System.Collections.Generic;
using System.Management;

namespace LenovoController.Providers
{
    public static class WmiProvider
    {
        public static int ExecuteGamezone(string methodName, string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            return Execute("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);
        }

        private static int Execute(string queryString, string methodName, string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var objectQuery = new ObjectQuery(queryString);
            using (var enumerator = new ManagementObjectSearcher(scope, objectQuery).Get().GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception("No results in query");
                var mo = (ManagementObject) enumerator.Current;
                var methodParamsObject = mo.GetMethodParameters(methodName);
                if (methodParams != null)
                {
                    foreach (var pair in methodParams)
                        methodParamsObject[pair.Key] = pair.Value;
                }

                return Convert.ToInt32(
                    mo.InvokeMethod(methodName, methodParamsObject, null)?.Properties[resultPropertyName].Value);
            }
        }
    }
}