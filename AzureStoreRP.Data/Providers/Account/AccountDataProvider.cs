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

namespace AzureStoreRP.Data.Providers.Account
{
    using AzureStoreRP.Data.Providers.Configuration;
    using log4net;
    using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
    using Microsoft.Practices.TransientFaultHandling;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;

    public class AccountDataProvider : IDisposable
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountDataProvider));

        #endregion

        #region RetryPolicy

        private static readonly Microsoft.Practices.TransientFaultHandling.RetryPolicy RetryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(
            retryStrategy : new Incremental(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
        );

        #endregion

        #region Constructor

        public static readonly AccountDataProvider Instance = new AccountDataProvider();

        static AccountDataProvider()
        {
            RetryPolicy.Retrying += (sender, e) =>
            {
                Logger.Error(
                    message     : String.Format("SQL Azure transient error detected. This is {0} attempt. Retrying...", e.CurrentRetryCount),
                    exception   : e.LastException
                );
            };

            Database.SetInitializer<AccountDataContext>(new CreateDatabaseIfNotExists<AccountDataContext>());
        }

        #endregion

        #region FindOrCreateTenantForUser

        public Guid FindOrCreateTenantForUser(string userEmail, string tenantName)
        {
            return Guid.NewGuid();
        }

        #endregion

        #region AzureStoreSubscriptions

        public AzureStoreSubscription GetAzureStoreSubscriptionBySubscriptionId(string subscriptionId)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                return AccountDataProvider.RetryPolicy.ExecuteAction<AzureStoreSubscription>(() =>
                {
                    return context.AzureStoreSubscriptions.FirstOrDefault(subscription => subscription.nvc_SubscriptionId == subscriptionId);
                });
            }
        }

        public AzureStoreSubscription[] GetAzureStoreSubscriptionsByEmail(string email)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                return AccountDataProvider.RetryPolicy.ExecuteAction<AzureStoreSubscription[]>(() =>
                {
                    return context.AzureStoreSubscriptions.Where(subscription => subscription.nvc_Email == email).ToArray();
                });
            }
        }

        public void CreateOrUpdateAzureStoreSubscription(AzureStoreSubscription subscription)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                if (subscription.id_AzureStoreSubscriptionId == Guid.Empty)
                {
                    context.Entry(subscription).State = EntityState.Added;
                    subscription.dt_CreatedTime = DateTime.UtcNow;
                    subscription.dt_ChangedTime = DateTime.UtcNow;
                }
                else
                {
                    context.Entry(subscription).State = EntityState.Modified;
                    subscription.dt_ChangedTime = DateTime.UtcNow;
                }

                AccountDataProvider.RetryPolicy.ExecuteAction(() =>
                {
                    context.SaveChanges();
                });
            }    
        }

        #endregion

        #region AzureStoreResources

        public AzureStoreResource GetAzureStoreResource(string subscriptionId, string cloudServiceName, string resourceType, string resourceName)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                return AccountDataProvider.RetryPolicy.ExecuteAction<AzureStoreResource>(() =>
                {
                    return context.AzureStoreResources.FirstOrDefault(subscription => 
                        subscription.nvc_SubscriptionId     == subscriptionId   &&
                        subscription.nvc_CloudServiceName   == cloudServiceName &&
                        subscription.nvc_ResourceType       == resourceType     &&
                        subscription.nvc_ResourceName       == resourceName
                    );
                });
            }
        }

        public AzureStoreResource[] GetAzureStoreResources(string subscriptionId, string cloudServiceName)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                return AccountDataProvider.RetryPolicy.ExecuteAction<AzureStoreResource[]>(() =>
                {
                    return context.AzureStoreResources.Where(subscription => 
                        subscription.nvc_SubscriptionId     == subscriptionId   &&
                        subscription.nvc_CloudServiceName   == cloudServiceName
                    ).ToArray();
                });
            }
        }

        public void CreateOrUpdateAzureStoreResource(AzureStoreResource resource)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                if (resource.id_AzureStoreResourceId == Guid.Empty)
                {
                    context.Entry(resource).State = EntityState.Added;
                    resource.dt_CreatedTime = DateTime.UtcNow;
                    resource.dt_ChangedTime = DateTime.UtcNow;
                }
                else
                {
                    context.Entry(resource).State = EntityState.Modified;
                    resource.dt_ChangedTime = DateTime.UtcNow;
                }

                AccountDataProvider.RetryPolicy.ExecuteAction(() =>
                {
                    context.SaveChanges();
                });
            }   
        }

        public void DeleteAzureStoreResource(AzureStoreResource resource)
        {
            using (AccountDataContext context = new AccountDataContext(ConfigurationDataProvider.AccountDataProviderConnectionString))
            {
                context.AzureStoreResources.Attach(resource);
                context.AzureStoreResources.Remove(resource);

                AccountDataProvider.RetryPolicy.ExecuteAction(() =>
                {
                    context.SaveChanges();
                });
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}