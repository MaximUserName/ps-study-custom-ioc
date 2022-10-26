using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace PsReflectionSample;

public class NetworkMonitor
{
	private static NetworkMonitorSettings _networkMonitorSettings = new NetworkMonitorSettings();

	private static Type? _warningServiceType;
	private static MethodInfo _warningServiceMethod;
	private static List<object> _warningServiceParameters = new List<object>();
	private static object _warningService;
	
	public static void BootstrapFromConfiguration()
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();
		configuration.Bind("NetworkMonitorSettings", _networkMonitorSettings);
		Console.WriteLine(_networkMonitorSettings.MethodToExecute);
		Console.WriteLine(_networkMonitorSettings.WarningService);

		_warningServiceType = Assembly.GetExecutingAssembly().GetType(_networkMonitorSettings.WarningService);

		if (_warningServiceType == null)
		{
			throw new Exception("Configuration is invalid. Warning service not found.");
		}
		
		_warningServiceMethod = _warningServiceType.GetMethod(_networkMonitorSettings.MethodToExecute);
		
		if (_warningServiceMethod == null)
		{
			throw new Exception("Configuration is invalid. Warning method not found.");
		}
		
		foreach (var keyValuePair in _networkMonitorSettings.PropertyBag)
		{
			try
			{
				var parameters = _warningServiceMethod.GetParameters().ToList();
				var parameterType = _warningServiceMethod.GetParameters()
					.FirstOrDefault(e => e.Name == keyValuePair.Key.ToLowerInvariant())
					?.ParameterType;
				if (parameterType != null)
				{
					var typedValue = Convert.ChangeType(keyValuePair.Value, parameterType);
					_warningServiceParameters.Add(typedValue);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			Console.WriteLine($"Key: {keyValuePair.Key}. Value: {keyValuePair.Value}");
		}
	}

	public static void Warn()
	{
		if(_warningService == null)
		{
			_warningService = Activator.CreateInstance(_warningServiceType);
		}
		_warningServiceMethod.Invoke(_warningService, _warningServiceParameters.ToArray());
	}
}