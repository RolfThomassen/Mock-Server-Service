using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace station_mock.Net
{
	public class IPAddressResolver
	{
		public static IList<IPAddressItem> GetList()
		{
			var list = new List<IPAddressItem>();
			var hostName = Dns.GetHostName();
			var addressList = Dns.GetHostEntry(hostName).AddressList;

			foreach (var item in addressList)
			{
				list.Add(new IPAddressItem(item));
			}

			return list;
		}
	}

	public class IPAddressItem
	{
		private IPAddress _ipAddress;

		public IPAddressItem(IPAddress ipAddress)
		{
			_ipAddress = ipAddress;
		}

		public string Address
		{
			get { return _ipAddress.ToString(); }
		}

		public bool IsIPV4
		{
			get { return _ipAddress.AddressFamily == AddressFamily.InterNetwork; }
		}
	}
}