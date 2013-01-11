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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class AzureStoreResourceManagementController : ApiController
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureStoreResourceManagementController));

        #endregion

        #region CreateOrUpdateResource

        //
        // PUT /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceName}
        //
        [HttpPut]
        public ResourceOutput CreateOrUpdateResource(string subscriptionId, string cloudServiceName, string resourceType, string resourceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            ResourceInput resourceInput = null;
            try
            {
                resourceInput = Request.Content.ReadAsAsync<ResourceInput>().Result;

                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName) || String.IsNullOrEmpty(resourceType) || String.IsNullOrEmpty(resourceName) || (resourceInput == null))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var eTag            = resourceInput.ETag;
                    var region          = resourceInput.CloudServiceSettings.GeoRegion;
                    var plan            = resourceInput.Plan;
                    var promotionCode   = resourceInput.PromotionCode;

                    var resource = provider.GetAzureStoreResource(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName,
                        resourceType        : resourceType,
                        resourceName        : resourceName
                    );

                    if (resource == null)
                    {
                        var subscription = provider.GetAzureStoreSubscriptionBySubscriptionId(subscriptionId);
                        if (subscription == null)
                        {
                            Logger.ErrorFormat("CreateOrUpdateResource: Unable to find Azure Store resource {1} for subscription {0}. Raw data:\n{2}\n", subscriptionId, resourceName, resourceInput.AsJson());
                            throw new HttpResponseException(HttpStatusCode.BadRequest);
                        }

                        resource = new AzureStoreResource
                        {
                            id_TenantId             = subscription.id_TenantId,
                            nvc_SubscriptionId      = subscriptionId,
                            nvc_CloudServiceName    = cloudServiceName,
                            nvc_ResourceType        = resourceType,
                            nvc_ResourceName        = resourceName,
                        };
                    }

                    if (!eTag.Equals(resource.nvc_ETag, StringComparison.OrdinalIgnoreCase))
                    {
                        resource.nvc_ETag           = eTag;
                        resource.nvc_Region         = region ?? resource.nvc_Region;
                        resource.nvc_Plan           = plan ?? resource.nvc_Plan;
                        resource.nvc_PromotionCode  = promotionCode ?? resource.nvc_PromotionCode;
                        resource.nvc_RawData        = resourceInput.AsJson();

                        provider.CreateOrUpdateAzureStoreResource(resource);
                    }

                    Logger.InfoFormat("CreateOrUpdateResource: Azure Store resource {1} for subscription {0} create or updated. Raw data:\n{2}\n", subscriptionId, resourceName, resourceInput.AsJson());
                    return this.MapAzureStoreResourceToResourceOutput(resource, expandOutputItems: true);
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
                        "CreateOrUpdateResource: Unable to provision or update resource {1} for subscription {0}. Raw data:\n{2}\n",
                        subscriptionId,
                        resourceName,
                        resourceInput != null ? resourceInput.AsJson() : "<null>"
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region GetResource

        //
        // GET /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceName}
        //
        [HttpGet]
        public ResourceOutput GetResource(string subscriptionId, string cloudServiceName, string resourceType, string resourceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName) || String.IsNullOrEmpty(resourceType) || String.IsNullOrEmpty(resourceName))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var resource = provider.GetAzureStoreResource(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName,
                        resourceType        : resourceType,
                        resourceName        : resourceName
                    );

                    if (resource == null)
                    {
                        Logger.ErrorFormat("GetResource: Unable to find Azure Store resource {1} for subscription {0}.", subscriptionId, resourceName);
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    return this.MapAzureStoreResourceToResourceOutput(resource, expandOutputItems: true);
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
                        "GetResource: Unable to find Azure Store resource {1} for subscription {0}.",
                        subscriptionId,
                        resourceName
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region DeleteResource

        //
        // DELETE /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceName}
        //
        [HttpDelete]
        public HttpResponseMessage DeleteResource(string subscriptionId, string cloudServiceName, string resourceType, string resourceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName) || String.IsNullOrEmpty(resourceType) || String.IsNullOrEmpty(resourceName))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var resource = provider.GetAzureStoreResource(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName,
                        resourceType        : resourceType,
                        resourceName        : resourceName
                    );

                    if (resource == null)
                    {
                        Logger.ErrorFormat("DeleteResource: Unable to find Azure Store resource {1} for subscription {0}.", subscriptionId, resourceName);
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    provider.DeleteAzureStoreResource(resource);

                    Logger.InfoFormat("DeleteResource: Azure Store resource {1} for subscription {0} deleted.", subscriptionId, resourceName);
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
                        "DeleteResource: Unable to delete Azure Store resource {1} for subscription {0}.",
                        subscriptionId,
                        resourceName
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region GetAllResourcesInCloudService

        //
        // GET /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}
        //
        [HttpGet]
        public CloudServiceOutput GetAllResourcesInCloudService(string subscriptionId, string cloudServiceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var resources = provider.GetAzureStoreResources(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName
                    );

                    if (resources == null || !resources.Any())
                    {
                        Logger.ErrorFormat("GetAllResourcesInCloudService: Unable to find Azure Store resources in cloud service {1} for subscription {0}.", subscriptionId, cloudServiceName);
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    return new CloudServiceOutput
                    {
                        GeoRegion   = resources.First().nvc_Region,
                        Resources   = new ResourceOutputList(resources.Select(resource => this.MapAzureStoreResourceToResourceOutput(resource, expandOutputItems: false)))
                    };
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
                        "GetAllResourcesInCloudService: Unable to find Azure Store resources in cloud service {1} for subscription {0}.",
                        subscriptionId,
                        cloudServiceName
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region DeleteCloudService

        //
        // DELETE /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}
        //
        [HttpDelete]
        public HttpResponseMessage DeleteCloudService(string subscriptionId, string cloudServiceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var resources = provider.GetAzureStoreResources(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName
                    );

                    if (resources == null || !resources.Any())
                    {
                        Logger.ErrorFormat("DeleteCloudService: Unable to find Azure Store resources in cloud service {1} for subscription {0}.", subscriptionId, cloudServiceName);
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    foreach (var resource in resources)
                    {
                        provider.DeleteAzureStoreResource(resource);
                    }

                    Logger.InfoFormat("DeleteCloudService: Azure Store resources in cloud service {1} for subscription {0} deleted.", subscriptionId, cloudServiceName);
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
                        "DeleteCloudService: Unable to find Azure Store resources in cloud service {1} for subscription {0}.",
                        subscriptionId,
                        cloudServiceName
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region SingleSignOn

        //
        // GET /subscriptions/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceType}/{resourceName}/SsoToken
        //
        [HttpGet][HttpPost]
        public SsoToken SingleSignOn(string subscriptionId, string cloudServiceName, string resourceType, string resourceName)
        {
            if (!AzureStoreAuthorization.AuthorizeRequest(this.Request.GetClientCertificate()))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            try
            {
                using (var provider = AccountDataProvider.Instance)
                {
                    if (String.IsNullOrEmpty(cloudServiceName) || String.IsNullOrEmpty(resourceType) || String.IsNullOrEmpty(resourceName))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    
                    var resource = provider.GetAzureStoreResource(
                        subscriptionId      : subscriptionId,
                        cloudServiceName    : cloudServiceName,
                        resourceType        : resourceType,
                        resourceName        : resourceName
                    );

                    if (resource == null)
                    {
                        Logger.ErrorFormat("SingleSignOn: Unable to find Azure Store resource {1} for subscription {0}.", subscriptionId, resourceName);
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    var timestamp = DateTime.UtcNow;

                    return new SsoToken
                    {
                        TimeStamp   = timestamp.Ticks.ToString(),
                        Token       = AzureStoreAuthorization.GetSsoToken(subscriptionId, cloudServiceName, resourceType, resourceName, resource.id_TenantId, timestamp)
                    };
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
                        "SingleSignOn: Unable to find Azure Store resource {1} for subscription {0}.",
                        subscriptionId,
                        resourceName
                    ),
                    exception   : ex
                );

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Private functions

        private ResourceOutput MapAzureStoreResourceToResourceOutput(AzureStoreResource resource, bool expandOutputItems)
        {
            return new ResourceOutput
            {
                ETag            = resource.nvc_ETag,
                Type            = resource.nvc_ResourceType,
                Name            = resource.nvc_ResourceName,
                Plan            = resource.nvc_Plan,
                State           = ResourceState.Started.ToString(),

                OutputItems     = expandOutputItems ? this.GetOutputItemsForAzureStoreResource(resource) : null,
                UsageMeters     = new UsageMeterList(this.GetUsageForAzureStoreResource(resource)),

                CloudServiceSettings = new CloudServiceSettings
                {
                    GeoRegion   = resource.nvc_Region
                },

                OperationStatus = new OperationStatus()
                {
                    Result      = OperationResult.Succeeded,
                    Error       = new Error
                    {
                        HttpCode    = (int)HttpStatusCode.OK,
                        Message     = "OK" 
                    },
                },
            };
        }

        private OutputItemList GetOutputItemsForAzureStoreResource(AzureStoreResource resource)
        {
            using (var provider = AccountDataProvider.Instance)
            {
                return new OutputItemList(
                    new OutputItem[]
                    {
                         new OutputItem() { Key = "ApplicationToken"    , Value = Utils.GenerateSecurityToken()   },
                         new OutputItem() { Key = "ApplicationSecret"   , Value = Utils.GenerateSecurityToken()   },
                    }
                );
            }
        }

        private UsageMeter GetUsageForAzureStoreResource(AzureStoreResource resource)
        {
            return new UsageMeter()
            {
                Name        = "Servers",
                Unit        = "generic",
                Used        = this.GetUsedServersForAzureStoreResource(resource),
                Included    = this.GetIncludedServersForAzureStoreResource(resource),
            };
        }

        private string GetUsedServersForAzureStoreResource(AzureStoreResource resource)
        {
            return "42";
        }

        private string GetIncludedServersForAzureStoreResource(AzureStoreResource resource)
        {
            if (resource.nvc_Plan.Equals("free"         , StringComparison.OrdinalIgnoreCase)) return "100";
            if (resource.nvc_Plan.Equals("advanced10"   , StringComparison.OrdinalIgnoreCase)) return "10";
            if (resource.nvc_Plan.Equals("advanced30"   , StringComparison.OrdinalIgnoreCase)) return "30";
            if (resource.nvc_Plan.Equals("advanced80"   , StringComparison.OrdinalIgnoreCase)) return "80";
                       
            Logger.ErrorFormat("GetIncludedServersForAzureStoreResource: Unknown resource plan {0} encountered.", resource.nvc_Plan);
            return "100";
        }

        #endregion
    }
}
