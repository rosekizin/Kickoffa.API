using System.Reflection;

namespace Kickoffa.API.TestUtils.InternalMember
{
	public static class InternaMemberAccessor
	{
		public static void SetPropertyWithPrivateSet<T>(this T obj, string propertyName, object? value)
		{
			var type = obj!.GetType();
			PropertyInfo? property = null;

			while (type != null)
			{
				property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null) break;
				type = type.BaseType;
			}

			var setMethod = property!.GetSetMethod(true); // true = inclui private set
			setMethod!.Invoke(obj, [value]);
		}

		public static void SetPrivatePropertyBackingField<T>(this T obj, string propertyName, object? value)
		{
			var type = obj!.GetType();			
			var fieldName = $"<{propertyName}>k__BackingField";

			FieldInfo? field = null;

			while (type != null)
			{
				field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null) break;

				type = type.BaseType;
			}
			
			field!.SetValue(obj, value);
		}

		public static R GetPrivateFieldValue<T, R>(this T obj, string propertyName)
		{
			var type = obj!.GetType();			
			var fieldName = propertyName;

			FieldInfo? field = null;

			while (type != null)
			{
				field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null) break;

				type = type.BaseType;
			}
			
			return (R)field!.GetValue(obj)!;
		}
	}
}