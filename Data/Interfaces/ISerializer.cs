using System;
namespace DataProcessor.Data.Interfaces
{
	public interface ISerializer<T>
	{
		string Serialize(in IEnumerable<T> obj);
	}
}

