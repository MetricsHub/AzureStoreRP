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
    using System.Xml;

    [DataContract(Name = "Resource", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceOutput : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public CloudServiceSettings CloudServiceSettings { get; set; }

        [DataMember(Order = 2)]
        public string ETag { get; set; }

        [DataMember(Order = 3)]
        public XmlNode[] IntrinsicSettings { get; set; }

        [DataMember(Order = 4)]
        public string Name { get; set; }

        [DataMember(Order = 5)]
        public OperationStatus OperationStatus { get; set; }

        [DataMember(Order = 6)]
        public OutputItemList OutputItems { get; set; }

        [DataMember(Order = 7)]
        public string Plan { get; set; }

        [DataMember(Order = 8)]
        public string PromotionCode { get; set; }

        [DataMember(Order = 9)]
        public string SchemaVersion { get; set; }

        [DataMember(Order = 10)]
        public string State { get; set; }

        [DataMember(Order = 11)]
        public string SubState { get; set; }

        [DataMember(Order = 12)]
        public string Type { get; set; }

        [DataMember(Order = 13)]
        public UsageMeterList UsageMeters { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
