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
    public class ResourceInput : IExtensibleDataObject
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public CloudServiceSettings CloudServiceSettings { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string ETag { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public XmlNode[] IntrinsicSettings { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string Plan { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string PromotionCode { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string SchemaVersion { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string Type { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
