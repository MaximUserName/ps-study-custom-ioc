using static System.Console;
using System.Reflection;
using PsReflectionSample;

//NetworkMonitorExample();







// CodeFromModule3();

void CodeFromModule3()
{
	var assemblyName = "PsReflectionSample";
	var typeName = "PsReflectionSample.Person";

	var person6 = Activator.CreateInstance(
			assemblyName: assemblyName,
			typeName: typeName,
			ignoreCase: true,
			bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
			binder: null,
			args: new object[] { "Some", 32 },
			culture: null,
			activationAttributes: null)
		?.Unwrap();

	WriteLine(person6);

	var propInfo = person6?.GetType().GetProperty(nameof(Person.Name));

	WriteLine(propInfo?.GetValue(person6));

	var privateFieldInfo = person6.GetType().GetField("_aprivateField", BindingFlags.Instance | BindingFlags.NonPublic);
	var value = privateFieldInfo.GetValue(person6);
	WriteLine(value);
}

void NetworkMonitorExample()
{
	NetworkMonitor.BootstrapFromConfiguration();

	Console.WriteLine("Error occured. Warn service executed.");
	NetworkMonitor.Warn();
}