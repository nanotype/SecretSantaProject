using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaProject
{
	class Member : INotifyPropertyChanged
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CompleteName 
		{
			get { return FirstName + " " + LastName; }
		}
		public Member Target { get; set; }

		public Member(string firstName, string lastName, Member target = null)
		{
			FirstName = firstName;
			LastName = lastName;
			Target = target;
		}

		public Member MAJ()
		{
			NotifyPropertyChanged("Member");
			return this;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string PropertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}
