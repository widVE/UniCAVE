using TMPro;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{

	[SerializeField]
	private TMP_Text text;

	/// <summary>
	/// Sets the text of textmesh pro text field
	/// </summary>
	/// <param name="text">the text to set</param>
	public void SetText(string text)
	{
		this.text?.SetText(text);
	}
}