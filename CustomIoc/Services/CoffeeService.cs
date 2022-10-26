namespace CustomIoc.Services;

public interface ICoffeeService
{
}

public class CoffeeService : ICoffeeService
{
	public CoffeeService(IWaterService waterService
		//, IBeanService<Catimor> beanService
		)
	{
		
	}
}

public class Catimor
{
}

public interface IBeanService<T>
{
}

class ArabicaBeanService<T> : IBeanService<T>
{
}

public interface IWaterService	
{
}

public class TapWaterService : IWaterService
{
}