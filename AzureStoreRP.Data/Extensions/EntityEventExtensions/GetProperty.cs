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
    using AzureStoreRP.Data.Contracts;
    using System;
    using System.Linq;

    public static partial class EntityEventExtensions
    {
        public static string GetProperty(this EntityEvent subscriptionEvent, string name)
        {
            return subscriptionEvent.Properties.CoalesceEnumerable().Where(
                property => property.PropertyName.Equals(name, StringComparison.OrdinalIgnoreCase)
            ).Select(
                property => property.PropertyValue
            ).FirstOrDefault();
        }
    }
}
