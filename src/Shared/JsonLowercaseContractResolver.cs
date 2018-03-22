using System;
using Newtonsoft.Json.Serialization;

namespace SmartRoadSense.Shared {

    internal class LowercaseContractResolver: DefaultContractResolver {

        protected override string ResolvePropertyName(string propertyName) {
            return propertyName.ToLower();
        }

    }

}
