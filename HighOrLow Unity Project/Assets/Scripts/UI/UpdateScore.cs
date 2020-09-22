using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class will update the score when the update score event is raised
/// </summary>
namespace HighOrLow
{
	public class UpdateScore : MonoBehaviour 
	{
		public TextMeshProUGUI scoreValueText;
		int score;

		void Start()
		{
			scoreValueText.text = score.ToString();
		}

		public void UpdateScoreAmount()
		{
			score ++;
			scoreValueText.text = score.ToString();
		}
	}
}


