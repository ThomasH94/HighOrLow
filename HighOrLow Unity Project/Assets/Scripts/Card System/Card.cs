using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///This class should be responsible for containing all of the data a card will have, as well as being able
///to reveal and move accordingly
///We have one basecard prefab that will get it's data assigned when created by the card controller
///</summary>

namespace HighOrLow
{
	public class Card : MonoBehaviour
	{
		[Header("Data")]
		public CardData cardData;

		public bool isRevealed = false;

		[Header("Rotation Variables")]
		public float rotateTime;
		public float rotateSpeed;

		public bool rotating = false;
		public bool canRotate = true;

		
		[Header("Graphics")]

		//This will be over the card face to hide it's value and will be disabled/enabled with the Reveal Method
		public SpriteRenderer cardBack;

		public SpriteRenderer cardFront;

		[Header("Audio")]
		public AudioClip revealSound;

		public void UpdateValues()
		{
			cardFront.sprite = cardData.cardFront;
			cardData.CardName = cardData.name;
		}

		//Wrapper method for the reveal routine which rotates the card
		//then disables the card back, and rotates back
		public void RevealCard()
		{
			StartCoroutine(RevealRoutine());
		}

		//Call the reveal routine and re-enable the card back
		public void UnRevealCard()
		{
			if(!isRevealed)
			{
				return;
			}
			
			float timeToRotate = 0.1f;
			StartCoroutine(RevealRoutine(false, timeToRotate));
		}

		//This coroutine is called when we reveal the card and rotates on the Y
		//We then disable the card back image, and rotate back seamlessly
		IEnumerator RevealRoutine(bool reveal = true, float timeToRotate = 0.3f)
		{
			rotating = true;
			canRotate = false;

			LeanTween.rotateY(gameObject, 90, timeToRotate);

			yield return new WaitForSeconds(timeToRotate);

			cardBack.enabled = !reveal;
			SoundManager.Instance.PlayRandomSound(revealSound);

			LeanTween.rotateY(gameObject, 0f, timeToRotate);

			yield return new WaitForSeconds(timeToRotate);
			
			rotating = false;
			canRotate = true;
			isRevealed = true;
		}

		//Called from an event and hides the card front
		public void Reset()
		{
			UnRevealCard();
		}

	}
}