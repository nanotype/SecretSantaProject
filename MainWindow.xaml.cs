using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecretSantaProject
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ObservableCollection<Member> Members = new ObservableCollection<Member>();

		public MainWindow()
		{
			InitializeComponent();
			MemberList.ItemsSource = Members;
		}

		private void BT_Add_Member_Click(object sender, RoutedEventArgs e)
		{
			string prenom = NewUser_FirstName.Text;
			string nom = NewUser_LastName.Text;
			if(prenom != "")
			{
				Members.Add(new Member(prenom, nom).MAJ());
				NewUser_FirstName.Clear();
				NewUser_LastName.Clear();
			}
			else
			{
				MessageBox.Show("Erreur système couche 8 !!!");
				NewUser_FirstName.BorderBrush = new SolidColorBrush(Colors.Red);
			}
		}

		private void BT_RAZ_Members_Click(object sender, RoutedEventArgs e)
		{
			Members.Clear();
		}

		private void BT_Delete_Member_Click(object sender, RoutedEventArgs e)
		{
			if(MemberList.SelectedItem != null)
			{
				Members.Remove(MemberList.SelectedItem as Member);
			}
		}
	}
}
