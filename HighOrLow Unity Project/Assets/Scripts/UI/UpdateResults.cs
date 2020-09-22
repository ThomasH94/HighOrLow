using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class will update the text results to indicate to the player
/// Why the won or lost
/// </summary>
namespace HighOrLow
{
	public class UpdateResults : MonoBehaviour 
	{
		public TextMeshProUGUI resultsTextValue;
		int score;

		public void UpdateResultsText(string newValue)
		{
			resultsTextValue.text = newValue;
		}
	}
}


