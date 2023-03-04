using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public TMPro.TMP_Text activePlayerTxt;

	public void setActivePlayerTxt(int currTurn)
	{
		SetText(activePlayerTxt, "Active Player: " + currTurn);
	}

	private void SetText(TMPro.TMP_Text txt, string newText)
	{
		txt.text = newText;
	}

	private void AppendText(TMPro.TMP_Text txt, string newText)
	{
		txt.text += newText;
	}
}
