using Newtonsoft.Json.Serialization;

namespace SmartRoadSense {

    internal class LowercaseContractResolver: DefaultContractResolver {

        protected override string ResolvePropertyName(string propertyName) {
            return propertyName.ToLower();
        }

    }

}
