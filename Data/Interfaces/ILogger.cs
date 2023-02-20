using System;
using DataProcessor.Data.Enums;
using DataProcessor.Data.Logs;

namespace DataProcessor.Data.Interfaces
{
	public interface ILogger
	{
		void Log(string details, LogType type);
		MetaLog MetaLog { get; }
	}
}

