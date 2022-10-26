using System.Reflection;
using System.Runtime.InteropServices;

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

public class IocContainer
{
	private Dictionary<Type, Type> _map = new Dictionary<Type, Type>();

	public void Register<TContract, TImplementation>() where TImplementation : class, TContract
	{
		this.Register(typeof(TContract), typeof(TImplementation));
	}
	
	public void Register(Type contract, Type implementation)
	{
		if(_map.ContainsKey(contract))
		{
			throw new Exception($"Registration for {contract.FullName} already exists.");
		}
		
		_map[contract] = implementation;
	}
	
	private object? ResolveInternal(Type contractType)
	{
		var implementation = TryFindImplementation(contractType);
		var ctorParameters = new List<object?>();
		
		foreach (var parameterInfo in implementation.GetCtorWithMaxParametersOrDefault().GetParameters())
		{
			ctorParameters.Add(ResolveInternal(parameterInfo.ParameterType));
		}

		var service = Activator.CreateInstance(
			type: implementation, 
			bindingAttr: BindingFlags.Instance | BindingFlags.Public,
			binder: null,
			args: ctorParameters.ToArray(),
			culture: null);
		
		return service;
	}

	private Type TryFindImplementation(Type contractType)
	{
		if(_map.TryGetValue(contractType, out var implementation))
		{
			return implementation;
		}
		
		if(contractType.IsClass && !contractType.IsAbstract)
		{
			return contractType;	
		}
		
		throw new Exception($"Implementation type was not found for {contractType.FullName}");
	}

	public TContract? Resolve<TContract>() //where TContract : class
	{
		return (TContract?)ResolveInternal(typeof(TContract));
		// var implementation = _map[typeof(TContract)];
		if(_map.TryGetValue(typeof(TContract), out var implementation))
		{
			var selectedConstructor = implementation.GetCtorWithMaxParametersOrDefault();
			if(!selectedConstructor.GetParameters().Any())
			{
				return (TContract?)Activator.CreateInstance(implementation);
			}
			
			var paramsList = new List<object?>();
			foreach (var parameterInfo in selectedConstructor.GetParameters())
			{
				if(parameterInfo.ParameterType.GetConstructors().Length == 0)
				{
					paramsList.Add(Activator.CreateInstance(parameterInfo.ParameterType));
				}
				
				paramsList.Add(ResolveInternal(parameterInfo.ParameterType));
			}
			
			// var paramsList = selectedConstructor.GetParameters().Select(p => 
			// 		Activator.CreateInstance(p.ParameterType))
			// 	.ToArray();
			
			var ins = Activator.CreateInstance(implementation, BindingFlags.Instance | BindingFlags.Public, 
				binder: null,
				args: paramsList.ToArray(),
				null);
			return (TContract?)ins;
			// var ctor = implementation.GetParameterLessConstructor();
			// if(ctor != null)
			// {
			// 	return (TContract?)Activator.CreateInstance(implementation);
			// }
			return (TContract?)Activator.CreateInstance(implementation);
			//throw new NotImplementedException("In map exists not implemented");
			//return this.Create<TContract>(implementation);
		}
		
		if(typeof(TContract).IsClass && !typeof(TContract).IsAbstract)
		{
			return (TContract?)Activator.CreateInstance(typeof(TContract));
		}
		
		return (TContract)Activator.CreateInstance(typeof(TContract))!;
	}
	
	public TContract Create<TContract>(Type implementationType) where TContract 
		: class
	{
		var resolvedService = Activator.CreateInstance(implementationType);
		return resolvedService as TContract;
		
		var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		var selectedConstructor = constructors.MaxBy(e => e.GetParameters().Length);
		var _ = selectedConstructor ?? throw new Exception($"No public constructor was found for {implementationType.FullName}");

		var service = CreateInternal(selectedConstructor.GetParameters(), implementationType) as TContract;
		return service;
		// return default(TContract);
	}

	private object CreateInternal(ParameterInfo[] constructorParameters, Type implementationType)
	{
		// end of recursion
		if(constructorParameters == null || constructorParameters.Length == 0)
		{
			var type = _map[implementationType];
			//return CreateInternal(type.g)
			return Activator.CreateInstance(implementationType);
		}
		
		var resolvedConstructorArguments = ResolveParameters(constructorParameters);
		
		return Activator.CreateInstance(implementationType, BindingFlags.Instance | BindingFlags.Public, null,
			resolvedConstructorArguments, null);
	}
	
	private ConstructorInfo FindPublicConstructorWithMaxParameters(Type implementationType)
	{
		var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		var selectedConstructor = constructors.MaxBy(e => e.GetParameters().Length);
		var _ = selectedConstructor ?? throw new Exception($"No public constructor was found for {implementationType.FullName}");
		return selectedConstructor;
	}

	private object?[]? ResolveParameters(ParameterInfo[] constructorParameters)
	{
		var result = new List<object?>();
		foreach (var parameter in constructorParameters)
		{
			var constructors = parameter.ParameterType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			var selectedConstructor = constructors.MaxBy(e => e.GetParameters().Length);
			var _ = selectedConstructor ?? throw new Exception($"No public constructor was found for {parameter.ParameterType.FullName}");
			var dependency = CreateInternal(selectedConstructor.GetParameters(), parameter.ParameterType);
		}
		return result.ToArray();
	}
}