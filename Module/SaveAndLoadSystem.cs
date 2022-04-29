using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SecretSantaProject.Module
{
	class SaveAndLoadSystem
	{
		public static FileInfo FileProject { get; set; }

		/// <summary>
		/// Permet de sauvegarder le projet secret santa dans le fichier xml courant
		/// </summary>
		/// <returns></returns>
		public static bool Save(List<Member> members)
		{
			if (FileProject.Exists)
			{
				File.WriteAllText(FileProject.FullName, JsonConvert.SerializeObject(PrepareSavingObjects(members)));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Permet de sauvegarder le projet secret santa dans un fichier xml
		/// </summary>
		/// <param name="members">la liste des membre à sauvegarder</param>
		/// <returns></returns>
		public static bool SaveAs(List<Member> members)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Json file (*.json)|*.json"
			};

			if (DialogResult.OK.Equals(saveFileDialog.ShowDialog()))
			{
				List<LiteMember> liteMembers = PrepareSavingObjects(members);
				File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(liteMembers));
				FileProject = new FileInfo(saveFileDialog.FileName);

				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="members"></param>
		/// <returns></returns>
		public static List<LiteMember> PrepareSavingObjects(List<Member> members)
		{
			List<LiteMember> memberToSave = new List<LiteMember>();

			foreach (Member member in members)
			{
				memberToSave.Add(new LiteMember(member.Name, member.Mail));
			}

			return memberToSave;
		}

		/// <summary>
		/// permet de charger un secret santa depuis un xml
		/// </summary>
		/// <returns>la liste de membre chargé, null si vide</returns>
		public static List<Member> Load()
		{
			List<Member> members = new List<Member>();

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Json file (*.json)|*.json"
			};

			switch (openFileDialog.ShowDialog())
			{
				case DialogResult.OK:
					FileProject = new FileInfo(openFileDialog.FileName);
					List<LiteMember> importedMembers = JsonConvert.DeserializeObject<List<LiteMember>>(File.ReadAllText(FileProject.FullName));
					foreach (LiteMember liteMember in importedMembers)
					{
						members.Add(new Member(liteMember.FirstName, liteMember.Mail));
					}
					break;

				case DialogResult.Cancel:
					members = null;
					break;
			}
			return members;
		}
	}
}
