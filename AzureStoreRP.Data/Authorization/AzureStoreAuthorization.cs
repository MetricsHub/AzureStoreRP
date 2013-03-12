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

namespace AzureStoreRP.Data.Authorization
{
    using AzureStoreRP.Data.Providers.Configuration;
    using log4net;
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public static class AzureStoreAuthorization
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureStoreAuthorization));

        #endregion

        #region GetSsoToken

        public static string GetSsoToken(string subscriptionId, string cloudServiceName, string resourceType, string resourceName, Guid tenantId, DateTime timestamp)
        {
            var key = Encoding.ASCII.GetBytes(ConfigurationDataProvider.AzureStoreSingleSignOnSecret);

            using (var hasher = new HMACSHA256(key))
            {
                var token = String.Format(
                    "{0}:{1}:{2}:{3}:{4}:{5}",
                    subscriptionId.ToLower(),
                    cloudServiceName.ToLower(),
                    resourceType.ToLower(),
                    resourceName.ToLower(),
                    tenantId.ToString(),
                    timestamp.Ticks.ToString()
                );

                return BitConverter.ToString(hasher.ComputeHash(Encoding.ASCII.GetBytes(token))).Replace("-", "").ToLower();
            }
        }

        #endregion

        #region AuthorizeRequest

        public static bool AuthorizeRequest(X509Certificate2 clientCertificate)
        {
            if (ConfigurationDataProvider.AzureStoreRequestAuthorization)
            {
                if (clientCertificate == null || (
                    // GTERdfeExtensibilityClientProd.cer, will ‎expire on Friday, ‎May ‎17, 2013
                    !clientCertificate.Thumbprint.Equals("C318C397979CB8BE103AA11346E4D82878C8A2B1", StringComparison.OrdinalIgnoreCase) &&

                    // BaltimoreRdfeExtensibilityClientProd.cer, will ‎expire on ‎Saturday, ‎February ‎14, ‎2015
                    !clientCertificate.Thumbprint.Equals("‎F2693F8487AB975A28C19610A672E59DDCF873F2", StringComparison.OrdinalIgnoreCase) &&

                    // BaltimoreRdfeExtensibilityClientStage.cer, will ‎expire on ‎Saturday, ‎February ‎14, ‎2015
                    !clientCertificate.Thumbprint.Equals("‎19D02B07DEC22C0998BB266A7DA5BA8B4D42A0A6", StringComparison.OrdinalIgnoreCase)
                ))
                {
                    Logger.ErrorFormat(
                        format  : "Unauthorized access to Azure Store integration endpoints: {0}, {1}", 
                        arg0    : clientCertificate != null ? clientCertificate.Subject     : "<null>",
                        arg1    : clientCertificate != null ? clientCertificate.Thumbprint  : "<null>"
                    );

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
