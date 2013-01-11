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

namespace AzureStoreRP.Data.Providers.Configuration
{
    using Microsoft.WindowsAzure.ServiceRuntime;
    using System;

    public static class ConfigurationDataProvider
    {
        public static bool AzureStoreRequestAuthorization
        {
            get { return Boolean.Parse(RoleEnvironment.GetConfigurationSettingValue("AzureStoreRP.Configuration.AzureStoreRequestAuthorization")); }
        }

        public static string AzureStoreSingleSignOnSecret
        {
            get { return RoleEnvironment.GetConfigurationSettingValue("AzureStoreRP.Configuration.AzureStoreSingleSignOnSecret"); }
        }

        public static string AccountDataProviderConnectionString
        {
            get { return RoleEnvironment.GetConfigurationSettingValue("AzureStoreRP.Configuration.AccountDataProviderConnectionString"); }
        }
    }
}
