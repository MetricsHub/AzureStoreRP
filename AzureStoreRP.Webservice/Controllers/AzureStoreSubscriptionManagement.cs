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

namespace AzureStoreRP.Webservice.Controllers
{
    using AzureStoreRP.Data.Authorization;
    using AzureStoreRP.Data.Contracts;
    using AzureStoreRP.Data.Extensions;
    using AzureStoreRP.Data.Providers.Account;
    using log4net;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class AzureStoreSubscriptionManagementController : ApiController
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureStoreSubscriptionManagementController));

        #endregion

        #region HandleSubscriptionEvent

        //
        // GET /subscriptions/{subscriptionId}/events
        //
        [HttpPost]
        public HttpResponseMessage HandleSubscriptionEvent(string subscriptionId)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            EntityEvent entityEvent = null;
            try
            {
                entityEvent = Request.Content.ReadAsAsync<EntityEvent>().Result;

                using (var provider = AccountDataProvider.Instance)
                {
                    var resourceType        = entityEvent.GetProperty("ResourceType");
                    var email               = entityEvent.GetProperty("EMail");
                    var optIn               = entityEvent.GetProperty("OptIn");

                    var subscription = provider.GetAzureStoreSubscriptionBySubscriptionId(subscriptionId);
                    if (subscription == null)
                    {
                        if (entityEvent.EntityState != EntityState.Registered)
                        {
                            Logger.ErrorFormat("HandleSubscriptionEvent: Event '{1}' received for unknown Azure store subscription {0}. Raw data:\n{2}\n", subscriptionId, entityEvent.EntityState, entityEvent.AsJson());
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }

                        if (String.IsNullOrEmpty(email))
                        {
                            Logger.ErrorFormat("HandleSubscriptionEvent: Failed to register new Azure store subscription {0}. Subscription doesn't have associated email. Raw data:\n{1}\n", subscriptionId, entityEvent.AsJson());
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }

                        Guid tenantId = provider.FindOrCreateTenantForUser(
                            userEmail               : email,
                            tenantName              : String.Format("Tenant for {0} {1} (via azure store)", email, subscriptionId)
                        );

                        subscription = new AzureStoreSubscription
                        {
                            nvc_SubscriptionId      = subscriptionId,
                            id_TenantId             = tenantId
                        };
                    }

                    subscription.nvc_EntityState    = entityEvent.EntityState.ToString();
                    subscription.nvc_ResourceType   = resourceType ?? subscription.nvc_ResourceType;
                    subscription.nvc_Email          = email ?? subscription.nvc_Email;
                    subscription.nvc_OptIn          = optIn ?? subscription.nvc_OptIn;
                    subscription.nvc_RawData        = entityEvent.AsJson();

                    provider.CreateOrUpdateAzureStoreSubscription(subscription);

                    Logger.InfoFormat("HandleSubscriptionEvent: Azure store subscription {0} updated. Raw data:\n{1}\n", subscriptionId, entityEvent.AsJson());
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalException(ex) || ex is HttpResponseException)
                {
                    throw;
                }

                Logger.Error(
                    message     : String.Format(
                        "HandleSubscriptionEvent: Unable to update Azure store subscription {0}. Raw data:\n{1}\n",
                        subscriptionId,
                        entityEvent != null ? entityEvent.AsJson() : "<null>"
                    ),
                    exception   : ex
                );

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        #endregion
    }
}
