namespace PsReflectionSample;

public class NetworkMonitorSettings
{
	public string WarningService { get; set; } = String.Empty;
	public string MethodToExecute { get; set; } = String.Empty;

	public Dictionary<string, object> PropertyBag { get; set; } = 
		new Dictionary<string, object>(comparer: StringComparer.OrdinalIgnoreCase);
}