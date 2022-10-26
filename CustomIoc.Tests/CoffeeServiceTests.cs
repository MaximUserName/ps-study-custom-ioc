using System.Net.Mime;
using System.Reflection;
using CustomIoc.Ioc;
using CustomIoc.Services;

namespace CustomIoc.Tests;

public class CoffeeServiceTests
{
	private IocContainer _container = new IocContainer();
	
	[SetUp]
	public void Setup()
	{
		_container = new IocContainer();
	}

	[Test]
	public void Ioc_ResolvesService()
	{
		_container.Register<ICoffeeService, CoffeeService>();
		_container.Register<IWaterService, TapWaterService>();
		_container.Register(typeof(IBeanService<>), typeof(ArabicaBeanService<>));
		
		Assert.DoesNotThrow(() => _container.Resolve<ICoffeeService>());
	}

	[Test]
	public void Ioc_ConcreteTypeNoCtorWithParamNotInDI_NotNull()
	{
		// Arrange
		_container.Register<ICoffeeService, CoffeeService>();
		_container.Register<IWaterService, TapWaterService>();
		
		// Act
		var service = _container.Resolve<Catimor>();
		
		// Assert
		Assert.That(service, Is.Not.Null);
	}
	
	[Test]
	public void Ioc_ConcreteTypeNoCtorWithParamInDI_NotNull()
	{
		// Arrange
		_container.Register<ICoffeeService, CoffeeService>();
		_container.Register<IWaterService, TapWaterService>();
		_container.Register<Catimor, Catimor>();
		
		// Act
		var service = _container.Resolve<Catimor>();
		
		// Assert
		Assert.That(service, Is.Not.Null);
	}

	[Test]
	public void Ioc_RegisteredDepCtorWithNoParams_NotNull()
	{
		// Arrange
		_container.Register<ICoffeeService, CoffeeService>();
		_container.Register<IWaterService, TapWaterService>();
		_container.Register<Catimor, Catimor>();

		// Act
		var service = _container.Resolve<IWaterService>();
		
		// Assert
		Assert.That(service, Is.Not.Null);
		Assert.That(service?.GetType(), Is.EqualTo(typeof(TapWaterService)));
	}

	[Test]
	public void Ioc_Registered_dep_ctor_one_arg_generic_interface_NotNull()
	{
		// Arrange
		_container.Register<ICoffeeService, CoffeeService>();
		_container.Register<IWaterService, TapWaterService>();
		_container.Register(typeof(IBeanService<>), typeof(ArabicaBeanService<>));

		// Act
		var coffeeService = _container.Resolve<ICoffeeService>();
		
		// Assert
		Assert.That(coffeeService, Is.Not.Null);
		Assert.That(coffeeService?.GetType(), Is.EqualTo(typeof(CoffeeService)));
	}
	
	class ClassWithNoCtorParams
	{
		
	}

	[Test]
	public void Reflection_CtorNoParams_HasOneCtor()
	{
		// Arrange
		var type = typeof(ClassWithNoCtorParams);
		
		// Act
		var ctors = type.GetConstructors().Select(e => e.ToString());

		// Assert
		Assert.That(ctors.Count(), Is.EqualTo(1));
	}
	
	[Test]
	public void Reflection_Instantiate_HasOneCtor()
	{
		// Arrange
		var type = typeof(ClassWithNoCtorParams);
		
		// Act
		var instance = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public, null,
			new object?[]{}, null);

		// Assert
		Assert.That(instance, Is.Not.Null);
	}

	[Test]
	public void Reflection_WithGenerics_InspectTypes()
	{
		var contractTypeOpened = typeof(IBeanService<>);
		var implementationTypeOpened = typeof(ArabicaBeanService<>);
		
		var contractTypeClosed = typeof(IBeanService<Catimor>);
		var implementationTypeClosed = typeof(ArabicaBeanService<Catimor>);
		
		// var write = TestContext.WriteLine;
		
		var underlyingType = contractTypeClosed.GetGenericTypeDefinition();
		var args = contractTypeClosed.GetGenericArguments();
		
		var constructedType = implementationTypeOpened.MakeGenericType(contractTypeClosed.GetGenericArguments());
		
		Assert.That(constructedType, Is.EqualTo(implementationTypeClosed));
	}

	[Test]
	public void Reflecton_Transform_Closed_Generic_to_opened_ReturnsOpenedGeneric()
	{
		// Arrange
		var closedGeneric = typeof(IBeanService<Catimor>);

		
		var openedGeneric = closedGeneric.GetGenericTypeDefinition();
		
		Assert.That(openedGeneric, Is.EqualTo(typeof(IBeanService<>)));
		// Act

		// Assert
	}
}