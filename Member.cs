using System.ComponentModel;

namespace SecretSantaProject
{
	class Member : INotifyPropertyChanged
	{
		public string Name { get; set; }
		public Member Target { get; set; }

		public Member(string firstName, Member target = null)
		{
			Name = firstName;
			Target = target;
		}

		public Member MAJ(string element)
		{
			NotifyPropertyChanged(element);
			return this;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string PropertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}
