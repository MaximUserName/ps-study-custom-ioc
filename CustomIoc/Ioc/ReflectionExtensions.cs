using System.Reflection;

namespace CustomIoc.Ioc;

public static class ReflectionExtensions
{
	public static bool HasParameterLessConstructor(this Type type)
	{
		var constructor = type
			.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
			.SingleOrDefault(e => e.GetParameters().Length == 0);
		return constructor != null;
	}
	
	public static ConstructorInfo? GetParameterLessConstructor(this Type type)
	{
		if(type.HasParameterLessConstructor())
			return type
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
				.Single(e => e.GetParameters().Length == 0);
		return null;
	}
	
	public static ConstructorInfo GetCtorWithMaxParametersOrDefault(this Type type)
	{
		var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		var selectedConstructor = constructors.MaxBy(e => e.GetParameters().Length);
		var _ = selectedConstructor ?? throw new Exception($"No public constructor was found for {type.FullName}");	
		return selectedConstructor;
	}
}