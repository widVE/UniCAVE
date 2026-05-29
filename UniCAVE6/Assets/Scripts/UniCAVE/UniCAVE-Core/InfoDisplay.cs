using TMPro;
using UnityEngine;

namespace UniCAVE
{
	public class InfoDisplay : MonoBehaviour
	{

		[SerializeField]
		private TMP_Text textField;

		/// <summary>
		/// Sets the text of textmesh pro text field
		/// </summary>
		/// <param name="newText">the text to set</param>
		public void SetText(string newText)
		{
			if(textField) textField.SetText(newText);
		}
	}
}