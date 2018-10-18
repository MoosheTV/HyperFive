using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperfive.Server.SharedModels
{
	public class SessionDataModel
	{
		public int NetId { get; set; }
		public string Name { get; set; }
		public Dictionary<string, dynamic> SharedData { get; set; }
		public Dictionary<string, dynamic> ProtectedData { get; set; }
	}
}
