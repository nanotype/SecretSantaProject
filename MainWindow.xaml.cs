using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
			DG_MemberList.ItemsSource = Members;
		}

		private void BT_Add_Member_Click(object sender, RoutedEventArgs e)
		{
			string prenom = NewUser_FirstName.Text;
			if(prenom != "")
			{
				Members.Add(new Member(prenom).MAJ("Member"));
				NewUser_FirstName.Clear();
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
			if(DG_MemberList.SelectedItem != null)
			{
				Members.Remove(DG_MemberList.SelectedItem as Member);
			}
		}

		private void Rollback()
		{
			foreach (Member member in Members)
			{
				member.Target = null;
				member.MAJ("Target");
			}
		}

		private void BT_Generate_SecretSanta_Click(object sender, RoutedEventArgs e)
		{
			if(Members.Count > 1)
			{
				Random random = new Random();

				// affectation
				ArrayList memberToDispatch = new ArrayList(Members);
				foreach (Member member in Members)
				{
					int randomMemberIndex = random.Next(0, memberToDispatch.Count);
					Member randomMember = memberToDispatch[randomMemberIndex] as Member;
					member.Target = randomMember;
					memberToDispatch.RemoveAt(randomMemberIndex);
					member.MAJ("Target");
				}

				// Vérification
				foreach (Member member in Members)
				{
					//Correction
					if (member.Target.Equals(member))// Si un participant est son propre père noel
					{
						int indexNewTarget;
						int indexMemberInError = Members.IndexOf(member);
						
						// Recherche index valide
						do
						{
							indexNewTarget = random.Next(0, Members.Count);
						} while (indexMemberInError == indexNewTarget);

						// Echange des pères noel
						member.Target = Members[indexNewTarget].Target;
						member.MAJ("Target");

						Members[indexNewTarget].Target = member;
						Members[indexNewTarget].MAJ("Target");
					}
				}
			}
		}

		private void BT_Rollback_Members_Click(object sender, RoutedEventArgs e)
		{
			Rollback();
		}

		private void Menu_ExportAs_txt_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
