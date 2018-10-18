using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperFive
{
	public class ClientAccessor
	{
		protected Client Client { get; }

		protected ClientAccessor( Client client ) {
			Client = client;
		}
	}
}
