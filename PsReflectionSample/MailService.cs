namespace PsReflectionSample;

public class MailService
{
	public void SendMail(string address, string subject)
	{
		Console.WriteLine($"Sending a warning email to address {address} with subject {subject}");
	}
}

public class SoundHornService
{
	public void SoundHorn(string volume)
	{
		Console.WriteLine($"Making noise with the volume turned up to {volume}");
	}
}