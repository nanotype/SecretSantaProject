using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;

namespace SecretSantaProject
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static bool Uncrypted { get; set; } = false;
		public bool SecretSantaGenerated { get; set; } = false;

		private ObservableCollection<Member> Members = new ObservableCollection<Member>();

		public MainWindow()
		{
			InitializeComponent();
			DG_MemberList.ItemsSource = Members;
		}

		private void BT_Add_Member_Click(object sender, RoutedEventArgs e)
		{
			string name = NewUser_Name.Text;
			if (name != "")
			{
				Members.Add(new Member(name));
				NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Silver);
				NewUser_Name.Clear();
			}
			else
			{
				MessageBox.Show("Erreur système couche 8 !!!");
				NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Red);
			}
		}

		private void BT_RAZ_Members_Click(object sender, RoutedEventArgs e)
		{
			Members.Clear();
		}

		private void BT_Delete_Member_Click(object sender, RoutedEventArgs e)
		{
			if (DG_MemberList.SelectedItem != null)
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
			SecretSantaGenerated = false;
		}

		private void GenerateSecretSanta()
		{
			if (Members.Count > 1)
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
			SecretSantaGenerated = true;
		}

		private void BT_Generate_SecretSanta_Click(object sender, RoutedEventArgs e)
		{
			GenerateSecretSanta();
		}

		private void BT_Rollback_Members_Click(object sender, RoutedEventArgs e)
		{
			Rollback();
		}

		private void Menu_ExportAs_txt_Click(object sender, RoutedEventArgs e)
		{
			SetupGeneration(GenerationType.txt);
		}

		private void SetupGeneration(GenerationType generationType)
		{
			var folderSelection = new System.Windows.Forms.FolderBrowserDialog();
			folderSelection.ShowDialog();
			if (folderSelection.SelectedPath != "")// Si on a sélectionné un chemin de destination
			{
				if (!SecretSantaGenerated)
					GenerateSecretSanta();
				Directory.CreateDirectory(folderSelection.SelectedPath);
				Generation(generationType, folderSelection.SelectedPath);
			}
		}

		private void Generation(GenerationType generationType, string generationPath)
		{
			switch (generationType)
			{
				case GenerationType.txt:
					GenerationTXT(generationPath);
					break;
			}
		}

		private void GenerationTXT(string generationPath)
		{
			foreach (Member member in Members)
			{
				string fileGenerated = $"{generationPath}{member.Name}.txt";
				File.WriteAllText(fileGenerated, member.Target.Name);
			}
		}

		private void MemberList_Encryption_Click(object sender, RoutedEventArgs e)
		{
			foreach (Member member in Members)
			{
				member.MAJ("Target");
			}
		}

		private void NewUser_FirstName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					string name = NewUser_Name.Text;
					if (name != "")
					{
						Members.Add(new Member(name));
						NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Silver);
						NewUser_Name.Clear();
					}
					break;

				case Key.Escape:
					NewUser_Name.Clear();
					NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Silver);
					break;
			}
		}

		private void DG_MemberList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				// récupération des données de la DataGrid
				Member memberToUpdate = e.EditingElement.DataContext as Member;
				string newMemberName = (e.EditingElement as TextBox).Text;

				//si on a un nom défférent par rapport aux données d'origine
				if (memberToUpdate.Name != newMemberName)
				{
					int updatedMemberIndex = Members.IndexOf(memberToUpdate);//récupération de l'index du membre dans la liste
					Members[updatedMemberIndex].Name = newMemberName;//mise à jour du nom
					Members[updatedMemberIndex].MAJ("EncryptedName");//mise à jour interface
				}
			}
		}

		private string PointedFile()
		{
			string targetPath = "C:/";

			return targetPath;
		}

		private bool Save(string targetPath)
		{
			try
			{
				XDocument xmlDocument = new XDocument();
				foreach (Member member in Members)
				{
					xmlDocument.Root.Add(member);
				}
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "Xml file (*.xml)|*.txt"
				};
				if (saveFileDialog.ShowDialog() == true)
				{
					xmlDocument.Save(targetPath);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private bool Load()
		{
			try
			{

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private void MainMenu_Save_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MainMenu_Load_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
