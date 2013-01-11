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

namespace AzureStoreRP.Website.Controllers
{
    using AzureStoreRP.Data.Authorization;
    using AzureStoreRP.Data.Extensions;
    using AzureStoreRP.Data.Providers.Account;
    using log4net;
    using System;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AzureStoreController : Controller
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureStoreController));

        #endregion

        [AllowAnonymous]
        public ActionResult SingleSignOn(string subid, string cloudServiceName, string resourceType, string resourceName, long timestamp, string token)
        {
            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName) || String.IsNullOrEmpty(resourceType) || String.IsNullOrEmpty(resourceName))
                    {
                        return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
                    }

                    var resource = provider.GetAzureStoreResource(
                        subscriptionId      : subid,
                        cloudServiceName    : cloudServiceName,
                        resourceType        : resourceType,
                        resourceName        : resourceName
                    );

                    if (resource == null || timestamp.FromTicks() < DateTime.UtcNow.AddMinutes(-10))
                    {
                        return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);
                    }

                    if (token != AzureStoreAuthorization.GetSsoToken(subid, cloudServiceName, resourceType, resourceName, resource.id_TenantId, timestamp.FromTicks()))
                    {
                        return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);
                    }

                    FormsAuthentication.SetAuthCookie("azurestore." + resource.id_TenantId, false);

                    return this.RedirectToAction(
                        actionName      : "Index",
                        controllerName  : "Home"
                    );
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    message: String.Format(
                        "Single sign on failed for resource {1} and subscription {0}.",
                        subid,
                        resourceName
                    ),
                    exception: ex
                );

                throw;
            }
        }
    }
}