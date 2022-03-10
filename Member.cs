using System.ComponentModel;

namespace SecretSantaProject
{
	class Member : INotifyPropertyChanged
	{
		public string Name { get; set; }
		public string EncryptedName => MainWindow.Uncrypted ? Name : @"/!\ DONNEE CRYPTEE /!\";
		public Member Target { get; set; }

		/// <summary>
		/// constructeur avec élément par défaut
		/// </summary>
		/// <param name="firstName"></param>
		/// <param name="target"></param>
		public Member(string firstName, Member target = null)
		{
			Name = firstName;
			Target = target;
		}

		/// <summary>
		/// permet de préciser la valeur qui est mise à jour et de mettre à jour l'interface
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public Member MAJ(string element)
		{
			NotifyPropertyChanged(element);
			return this;
		}

		/// <summary>
		/// permet de mettre à jour l'interface rattaché aux données automatiquement
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string PropertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}
