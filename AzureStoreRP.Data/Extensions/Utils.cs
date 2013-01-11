/*
* Copyright 2013 MetricsHub, Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

namespace AzureStoreRP.Data.Extensions
{
    using log4net;
    using System;
    using System.Security;
    using System.Security.Cryptography;
    using System.Threading;

    public static class Utils
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Utils));

        #endregion

        #region IsFatalException

        public static bool IsFatalException(Exception ex)
        {
            return
                ex is TypeInitializationException   ||
                ex is AppDomainUnloadedException    ||
                ex is ThreadInterruptedException    ||
                ex is AccessViolationException      ||
                ex is InvalidProgramException       ||
                ex is BadImageFormatException       ||
                ex is StackOverflowException        ||
                ex is ThreadAbortException          ||
                ex is OutOfMemoryException          ||
                ex is SecurityException
             ;
        }

        #endregion

        #region GenerateSecurityToken

        public static string GenerateSecurityToken()
        {
            byte[] seed = new byte[64];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(seed);

                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    return Convert.ToBase64String(md5.ComputeHash(seed));
                }
            }
        }

        #endregion
    }
}
