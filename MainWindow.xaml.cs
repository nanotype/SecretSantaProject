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
using SecretSantaProject.Module;
using System.Collections.Generic;

namespace SecretSantaProject
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public bool ProjectFileLinked { get; set; } = SaveAndLoadSystem.FileProject != null ? SaveAndLoadSystem.FileProject.Exists : false;

		/// <summary>
		/// Défini l'affichage de la données en clair ou non
		/// </summary>
		public static bool Uncrypted { get; set; } = false;

		/// <summary>
		/// Défini si le secret santa à été généré
		/// </summary>
		private bool SecretSantaGenerated { get; set; } = false;

		/// <summary>
		/// Liste des membres participant au Secret Santa
		/// </summary>
		private ObservableCollection<Member> Members = new ObservableCollection<Member>();

		/// <summary>
		/// Constructeur de la fenetre principale
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			// Liaison entre le tableau et la liste des membres
			DG_MemberList.ItemsSource = Members;
		}

		/// <summary>
		/// Lance l'action d'ajout d'un nouveau membre
		/// Rattachée au bouton BT_Add_Member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BT_Add_Member_Click(object sender, RoutedEventArgs e)
		{
			string name = NewUser_Name.Text;
			
			// Si le champ est conforme
			if (!string.IsNullOrWhiteSpace(name))
			{
				// Ajout membre
				Members.Add(new Member(name));
				NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Silver);
				NewUser_Name.Clear();
			}
			else
			{
				NewUser_Name.BorderBrush = new SolidColorBrush(Colors.Red);
			}
		}

		/// <summary>
		/// Lance l'action de remise à zéro des données
		/// Rattachée au bouton BT_RAZ_Members
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BT_RAZ_Members_Click(object sender, RoutedEventArgs e)
		{
			Members.Clear();
		}

		/// <summary>
		/// Lance l'action de suppression d'un utilisateur
		/// Rattachée au bouton BT_Delete_Member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BT_Delete_Member_Click(object sender, RoutedEventArgs e)
		{
			if (DG_MemberList.SelectedItems != null)
			{
				int nbMemberDeleted = 0;
				while (DG_MemberList.SelectedItems.Count > 0)
				{
					Members.Remove(DG_MemberList.SelectedItem as Member);
					nbMemberDeleted++;
				}
				Console.WriteLine($"Members deleted : {nbMemberDeleted}");
			}
		}

		/// <summary>
		/// Permet de remettre à zéro les affectations des utilisateurs
		/// </summary>
		private void Rollback()
		{
			foreach (Member member in Members)
			{
				member.Target = null;
				member.MAJ("Target");
			}
			SecretSantaGenerated = false;
		}

		/// <summary>
		/// Permet de générer le secret santa
		/// </summary>
		private void GenerateSecretSanta()
		{
			// si pas assez de membre, on ne lance aucun traitement
			if (Members.Count > 1)
			{
				Random random = new Random();

				#region Affectation
				// copie de la liste des membres dans une liste de travail
				ArrayList memberToDispatch = new ArrayList(Members);

				foreach (Member member in Members)
				{
					// récupération aléatoire d'un membre à affecter au membre actif
					int randomMemberIndex = random.Next(0, memberToDispatch.Count);
					Member randomMember = memberToDispatch[randomMemberIndex] as Member;

					// Affectation du secret santa
					member.Target = randomMember;

					//suppression du membre affecté de la liste des membre disponible
					memberToDispatch.RemoveAt(randomMemberIndex);

					member.MAJ("Target");
				}
				#endregion

				#region Vérification
				foreach (Member member in Members)
				{
					// Si un participant est son propre secret santa
					if (member.Target.Equals(member))
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
						Members[indexNewTarget].Target = member;

						// Mise à jour interface
						member.MAJ("Target");
						Members[indexNewTarget].MAJ("Target");
					}
				}
				#endregion

				SecretSantaGenerated = true;
			}
			else
			{
				MessageBox.Show("Erreur système couche 8 !!!");
			}
		}

		/// <summary>
		/// Lance l'action de génération du Secret Santa
		/// Rattachée au bouton BT_Generate_SecretSanta
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BT_Generate_SecretSanta_Click(object sender, RoutedEventArgs e)
		{
			GenerateSecretSanta();
		}

		/// <summary>
		/// Lance l'action de mise à zéro des affectations
		/// Rattachée au bouton BT_Rollback_Members
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BT_Rollback_Members_Click(object sender, RoutedEventArgs e)
		{
			Rollback();
		}

		/// <summary>
		/// Lance l'export du secret santa généré
		/// Rattachée au bouton Menu_ExportAs_txt
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Menu_ExportAs_txt_Click(object sender, RoutedEventArgs e)
		{
			SetupGeneration(GenerationType.txt);
		}

		/// <summary>
		/// Première étape de l'export
		/// Permet de choisir l'emplacement un export du résultat du secret santa
		/// Le type de fichier de sortie est géré en fonction du paramètre d'entré
		/// </summary>
		/// <param name="generationType">Element de l'énum <see cref="GenerationType"/></param>
		private void SetupGeneration(GenerationType generationType)
		{
			var folderSelection = new System.Windows.Forms.FolderBrowserDialog();
			folderSelection.ShowDialog();
			if (folderSelection.SelectedPath != "")// Si on a sélectionné un chemin de destination
			{
				if (!SecretSantaGenerated)
				{
					GenerateSecretSanta();
				}

				Directory.CreateDirectory(folderSelection.SelectedPath);
				Generation(generationType, folderSelection.SelectedPath);
			}
		}

		/// <summary>
		/// Seconde etape de l'export
		/// Permet de choisir quelle fonction lancer en fonction du type d'export lancé
		/// </summary>
		/// <param name="generationType">Element de l'énum <see cref="GenerationType"/></param>
		/// <param name="generationPath">Chemin du dossier cible pour la sauvegarde</param>
		private void Generation(GenerationType generationType, string generationPath)
		{
			switch (generationType)
			{
				case GenerationType.txt:
					GenerationTXT(generationPath);
					break;
			}
		}

		/// <summary>
		/// Dernière étape de l'export
		/// Permet de réaliser l'export txt
		/// </summary>
		/// <param name="generationPath">Chemin du dossier cible pour la sauvegarde</param>
		private void GenerationTXT(string generationPath)
		{
			foreach (Member member in Members)
			{
				string fileGenerated = $"{generationPath}/{member.Name}.txt";
				File.WriteAllText(fileGenerated, member.Target.Name);
			}
		}

		/// <summary>
		/// Lance l'action de cryptage/décryptage de la colonne des affectations
		/// Rattachée au bouton MemberList_Encryption
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MemberList_Encryption_Click(object sender, RoutedEventArgs e)
		{
			foreach (Member member in Members)
			{
				member.MAJ("Target");
			}
		}

		/// <summary>
		/// Lance l'action de gestion des raccourci clavier du champs textuel de l'ajout de nouveau utilisateur
		/// Rattachée au champs NewUser_FirstName
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewUser_FirstName_KeyDown(object sender, KeyEventArgs e)
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

		/// <summary>
		/// Lance l'action de gestion de fin d'édition de cellule
		/// Rattachée à l'évènement de fin d'édition de cellule de la datagrid DG_MemberList
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Permet de pointer un fichier précis
		/// </summary>
		/// <returns></returns>
		private string PointedFile()
		{
			string targetPath = "C:/";

			return targetPath;
		}

		/// <summary>
		/// Permet de sauvegarder le secret santa dans un fichier xml
		/// </summary>
		/// <param name="targetPath"></param>
		/// <returns></returns>
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

		/// <summary>
		/// lance l'action de sauvegarde du secret santa
		/// rattaché au clic sur le menu MainMenu_Save
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainMenu_Save_Click(object sender, RoutedEventArgs e)
		{
			SaveAndLoadSystem.Save(new List<Member>(Members));
		}

		/// <summary>
		/// lance l'action de chargement du secret santa
		/// rattaché au clic sur le menu MainMenu_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainMenu_Load_Click(object sender, RoutedEventArgs e)
		{
			List<Member> loadingMembers = SaveAndLoadSystem.Load();
			if(loadingMembers != null)
			{
				Members.Clear();
				foreach (Member member in loadingMembers)
				{
					Members.Add(member);
				}
			}
		}

		private void MainMenu_Quit_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MainMenu_SaveAs_Click(object sender, RoutedEventArgs e)
		{
			SaveAndLoadSystem.SaveAs(new List<Member>(Members));
		}
	}
}
