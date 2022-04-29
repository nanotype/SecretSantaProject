using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaProject
{
	class LiteMember
	{
		public string FirstName { get; set; }
		public string Mail { get; set; }

		public LiteMember(string name, string mail = "")
		{
			FirstName = name;
			Mail = mail;
		}
	}
}
