using System;

namespace Socket.Shared
{
	public class POCSocketException : Exception
	{
		public POCSocketException(string message) : base(message)
		{

		}

		public POCSocketException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}