// See https://aka.ms/new-console-template for more information

using CustomIoc.Ioc;
using CustomIoc.Services;

var container = new IocContainer();

var coffeeService = container.Resolve<ICoffeeService>();

Console.WriteLine("Hello, World!");