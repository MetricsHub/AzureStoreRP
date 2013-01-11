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

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Cis.DevExp.Services.Rdfe.ServiceManagement")]
    public class EntityEvent : IExtensibleDataObject
    {
        [DataMember(EmitDefaultValue = false, IsRequired = true, Order = 0)]
        public string EventId { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string ListenerId { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string EntityType { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public EntityState EntityState { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public EntityId EntityId { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public string OperationId { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public bool IsAsync { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public EntityPropertyList Properties { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
