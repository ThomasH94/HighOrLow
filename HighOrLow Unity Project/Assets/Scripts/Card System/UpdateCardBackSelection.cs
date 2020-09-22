using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class will update our UI selection to match which card back we want to set
/// We will then call an event to update the card backs
/// </summary>
namespace HighOrLow
{
	public class UpdateCardBackSelection : MonoBehaviour 
	{
		public Sprite[] cardBacks;
		[SerializeField]
		Image spriteToDisplay;
		int spriteIndex;

		void OnEnable()
		{
			spriteToDisplay.sprite = cardBacks[spriteIndex];
		}

		//Called from the IncreaseCardIndex event to toggle right in our cardbacks array
		public void IncreaseCardSelection(int index)
		{
			spriteIndex ++;
			if(spriteIndex > cardBacks.Length -1)
			{
				spriteIndex = 0;
			}
			spriteToDisplay.sprite = cardBacks[spriteIndex];

		}

		//Called from the DecreaseCardIndex event to toggle left in our cardbacks array
		public void DecreaseCardSelection(int index)
		{
			spriteIndex --;
			if(spriteIndex < 0)
			{
				spriteIndex = cardBacks.Length - 1;
			}
			spriteToDisplay.sprite = cardBacks[spriteIndex];
		}


	}

}

