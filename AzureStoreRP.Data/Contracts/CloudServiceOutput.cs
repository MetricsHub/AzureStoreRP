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

namespace AzureStoreRP.Data.Contracts
{
    using System.Runtime.Serialization;

    [DataContract(Name = "CloudService", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class CloudServiceOutput : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public string GeoRegion { get; set; }

        [DataMember(Order = 2)]
        public ResourceOutputList Resources { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
