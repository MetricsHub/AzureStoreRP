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
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    public class AccountDataContext : DbContext
    {
        public AccountDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<AzureStoreSubscription>    AzureStoreSubscriptions { get; set; }
        public DbSet<AzureStoreResource>        AzureStoreResources     { get; set; }
    }

    [Table("AzureStoreSubscriptions")]
    public class AzureStoreSubscription
    {
        public AzureStoreSubscription()
        {
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid         id_AzureStoreSubscriptionId { get; set; }
        public Guid         id_TenantId                 { get; set; }
        public string       nvc_SubscriptionId          { get; set; }
        public string       nvc_EntityState             { get; set; }
        public string       nvc_ResourceType            { get; set; }
        public string       nvc_Email                   { get; set; }
        public string       nvc_OptIn                   { get; set; }
        public string       nvc_RawData                 { get; set; }
        public DateTime     dt_CreatedTime              { get; set; }
        public DateTime     dt_ChangedTime              { get; set; }
        public DateTime?    dt_DeletedTime              { get; set; }
    }

    [Table("AzureStoreResources")]
    public class AzureStoreResource
    {
        public AzureStoreResource()
        {
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid         id_AzureStoreResourceId     { get; set; }
        public Guid         id_TenantId                 { get; set; }
        public string       nvc_SubscriptionId          { get; set; }
        public string       nvc_CloudServiceName        { get; set; }
        public string       nvc_ResourceType            { get; set; }
        public string       nvc_ResourceName            { get; set; }
        public string       nvc_ETag                    { get; set; }
        public string       nvc_Region                  { get; set; }
        public string       nvc_Plan                    { get; set; }
        public string       nvc_PromotionCode           { get; set; }
        public string       nvc_RawData                 { get; set; }
        public DateTime     dt_CreatedTime              { get; set; }
        public DateTime     dt_ChangedTime              { get; set; }
        public DateTime?    dt_DeletedTime              { get; set; }
    }
}

