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

namespace AzureStoreRP.Webservice.Configuration
{
    using System.Web.Http;

    public static class RegisterWebApiRoutes
    {
        public static void Register(HttpConfiguration config)
        {
            // Azure Store expect xml instead of json, so we need to set XmlFormatter as default
            var xmlFormatter = config.Formatters.XmlFormatter;
            config.Formatters.Remove(xmlFormatter);
            config.Formatters.Insert(0, xmlFormatter);

            config.Routes.MapHttpRoute(
                name            : "AzureStoreSubscriptionManagement",
                routeTemplate   : "api/azurestore/subscriptions/{subscriptionId}/events",
                defaults        : new { controller = "AzureStoreSubscriptionManagement", action = "HandleSubscriptionEvent" }
            );

            config.Routes.MapHttpRoute(
                name            : "AzureStoreResourceManagementForCloudServiceName",
                routeTemplate   : "api/azurestore/subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}",
                defaults        : new { controller = "AzureStoreResourceManagement" }
            );

            config.Routes.MapHttpRoute(
                name            : "AzureStoreResourceManagementForSingleSignOn",
                routeTemplate   : "api/azurestore/subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceType}/{resourceName}/SsoToken",
                defaults        : new { controller = "AzureStoreResourceManagement", action = "SingleSignOn" }
            );

            config.Routes.MapHttpRoute(
                name            : "AzureStoreResourceManagementForResourceName",
                routeTemplate   : "api/azurestore/subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceType}/{resourceName}",
                defaults        : new { controller = "AzureStoreResourceManagement" }
            );
        }
    }
}