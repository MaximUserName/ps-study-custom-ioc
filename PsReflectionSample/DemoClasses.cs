using static System.Console;	
namespace PsReflectionSample;

public interface ITalk
{
	void Talk(string sentence);
}

class Alien : ITalk
{
	public void Talk(string sentence)
	{
		WriteLine($"Alien talking {sentence}");
	}
}

public class EmployeeMarkerAttribute : Attribute
{
	
}

public class Person : ITalk
{
	public Person()
	{
		WriteLine("A person is being created.");
	}

	public Person(string? name)
	{
		WriteLine($"A person with name {name} is being created");
		this.Name = name;
	}
	
	private Person(string? name, int age)
	{
		WriteLine($"A person with name {name} and age {age} is being created using private constructor");
		this.Name = name;
		this.age = age;
	}
	
	public string Name { get; set; }
	public int age { get; set; }
	private string _aprivateField = "initial private field value";
	
	public void Talk(string sentence)
	{
		WriteLine($"Talking sentence {sentence}");
	}
	
	protected void Yell(string sentence)
	{
		WriteLine($"YELLING! {sentence}");
	}

	public override string ToString()
	{
		return $"{Name} {age} {_aprivateField}";
	}
	
}

[EmployeeMarker]
class Employee : Person
{
	public string Company { get; set; }
	public void Talk(string sentence)
	{
	}
}