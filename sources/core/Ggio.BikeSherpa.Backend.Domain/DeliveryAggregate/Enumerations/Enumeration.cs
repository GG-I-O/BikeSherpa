using System.Reflection;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public abstract class Enumeration : IComparable
{
	public int Id { get; }
	public string Name { get; }

	protected Enumeration(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public override string ToString() => Name;

	public int CompareTo(object? other) => other is Enumeration e ? Id.CompareTo(e.Id) : -1;

	public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
		typeof(T)
			.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
			.Where(f => f.FieldType == typeof(T))
			.Select(f => (T)f.GetValue(null)!);

	public static T FromId<T>(int id) where T : Enumeration =>
		GetAll<T>().FirstOrDefault(x => x.Id == id)
		?? throw new ArgumentException($"Invalid id '{id}' for {typeof(T).Name}");

	public static T FromName<T>(string name) where T : Enumeration =>
		GetAll<T>().FirstOrDefault(x => x.Name == name)
		?? throw new ArgumentException($"Invalid name '{name}' for {typeof(T).Name}");
}
