using Newtonsoft.Json;
using System.Reflection;
using Xunit;

namespace Kickoffa.API.TestUtils.JsonProperty
{
	public static class JsonPropertiesValidator
	{
		public static PropertyInfo AssertPropertyName(this PropertyInfo property, string expectedPropertyName)
		{
			var jsonPropertyAttribute = GetJsonPropertyAttribute(property);
			Assert.Equal(expectedPropertyName, jsonPropertyAttribute?.PropertyName);

			return property;
		}

		public static PropertyInfo AssertRequired(this PropertyInfo property, Required expectedRequired)
		{
			var jsonPropertyAttribute = GetJsonPropertyAttribute(property);
			Assert.Equal(expectedRequired, jsonPropertyAttribute?.Required);

			return property;
		}

		private static JsonPropertyAttribute? GetJsonPropertyAttribute(PropertyInfo property)
		{
			return property?.GetCustomAttribute<JsonPropertyAttribute>();
		}
	}
}