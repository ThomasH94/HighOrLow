using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
///The purpose of this class is to get our cards shuffled, placed, and revealed to score points
/// We do this by calling events and listening to events
/// We have one card prefab that gets it's data from the card data we provide it using Scriptable Objects
/// All cards get their data from the cards data list 
///</summary>
namespace HighOrLow
{
	public class CardController : MonoBehaviour
	{
		[Header("Card Info")]
		public GameObject cardPrefab;
		public List<Card> Deck = new List<Card>();
		public List<CardData> CardsData = new List<CardData>();

		[Header("Graphics")]
		public Sprite[] cardBacks;

		public Sprite activeCardBack;
		private int cardBackIndex;

		[SerializeField]
		private Transform deckTransform;

		[SerializeField]
		private Transform[] cardPositions;

		[Header("Audio")]
		public Card[] activeCards;
		public AudioClip cardShuffleSFX;
		public AudioClip cardSlideSFX;
		public AudioClip cardBridgeSFX;
		public AudioClip winSFX;
		public AudioClip loseSFX;

		#region Events

		[Header("Events")]
		public GameEvent canDealEvent;
		public GameEvent gameLoadedEvent;
		public GameEvent gameResetEvent;
		public GameEvent playerCardRevealedEvent;
		public GameEvent cardsDealtEvent;
		public GameEvent winEvent;
		public GameEvent loseEvent;
		public GameEvent cardValueWasHigherEvent;
		public GameEvent cardValueWasLowerEvent;
		public GameEvent suitValueWasHigherEvent;
		public GameEvent suitvalueWasLowerEvent;

		#endregion

		
		//I prefer using OnEnable as it can be called multiple times during the games execution
		void OnEnable()
		{
			gameLoadedEvent.Raise();
		}

		//Creat a deck of 52 cards, give them all a card front from the list, and the selected card back
		public void CreateDeck()
		{
			for(int i = 0; i < CardsData.Count; i++)
			{
				GameObject newCard = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity) as GameObject;
				Card card = newCard.GetComponent<Card>();
				card.cardData = CardsData[i];
				card.UpdateValues();
				card.cardBack.sprite = cardBacks[cardBackIndex];


				newCard.name = card.cardData.CardName;
				newCard.transform.parent = deckTransform;

				Deck.Add(newCard.GetComponent<Card>());
				Debug.Log("New card created! Card name is " + newCard.name + ", card value is " + card.cardData.CardValue + ", and the card has been added to the list");				
			}

			ShuffleDeck(Deck);
		}

		//Shuffle the Deck based on the Fisher-Yates Shuffle
		//We then play shuffle the card gameobjects for presentation
		//We take a generic type for sake of ease
		void ShuffleDeck<T>(List<T> listToShuffle, int timesToShuffle = 1)
		{
			System.Random random = new System.Random();
			int n = listToShuffle.Count;
			
			while (n > 1)
			{
				int k = random.Next(n);
				n--;
				T temp = listToShuffle[k];
				listToShuffle[k] = listToShuffle[n];
				listToShuffle[n] = temp;
			}

			StartCoroutine(ShuffleCardAnimation());

		}

		//Using Lean Tween, we move the cards into 2 seperate stacks and play a shuffling sound
		//Then go back through them in reverse and place them back into the deck position and play a bridging sound
		//We use a coroutine to time the cards shuffling and to trigger events in time
		IEnumerator ShuffleCardAnimation(float timeToWait = 0.5f)
		{
			float xOffset = 4.5f;
			float timeToMove = 0.03f;
			float lessTimeToWait = 0.0075f;

			Vector3 leftPosition = new Vector3((deckTransform.position.x - xOffset), deckTransform.position.y, deckTransform.position.z);
			Vector3 rightPosition = new Vector3((deckTransform.position.x + xOffset), deckTransform.position.y, deckTransform.position.z);

			WaitForSeconds timeToWaitForSeconds = new WaitForSeconds(timeToWait);
			WaitForSeconds lessTimeToWaitForSeconds = new WaitForSeconds(lessTimeToWait);


			yield return timeToWaitForSeconds;
			SoundManager.Instance.PlaySingleClip(cardShuffleSFX);

			for(int i = 0; i < Deck.Count; i++)
			{
				if(i % 2 == 0)
				{
					LeanTween.move(Deck[i].gameObject, rightPosition, timeToMove);
				}
				else
				{
					LeanTween.move(Deck[i].gameObject, leftPosition, timeToMove);
				}
				
				yield return lessTimeToWaitForSeconds;
			}

			yield return timeToWaitForSeconds;
			SoundManager.Instance.PlaySingleClip(cardBridgeSFX);

			for(int i = Deck.Count - 1; i >= 0; i--)
			{
				if(i % 2 == 0)
				{
					LeanTween.move(Deck[i].gameObject, deckTransform.position, timeToMove);
				}
				else
				{
					LeanTween.move(Deck[i].gameObject, deckTransform.position, timeToMove);
				}

				yield return lessTimeToWaitForSeconds;
			}

			yield return timeToWaitForSeconds;

			canDealEvent.Raise();
		}

		//Using Events, we call this method due to restrictions calling Coroutines with Events
		public void DealCards()
		{
			StartCoroutine(PullRandomCards());
		}
		
		//Using a coroutine, we pull 2 cards based on their weight if we don't have any currently active cards
		//This method can be used to pull multiple cards if we want to expand the game
		IEnumerator PullRandomCards(int cardAmountToPull = 2, float timeToWait = 0.3f)
		{
			for(int i = 0; i < cardAmountToPull; i++)
			{
				if(activeCards[i] != null)
				{
					break;
				}

				int randomWeight = CheckWeights();
				activeCards[i] = (Deck[randomWeight]);
				Deck.RemoveAt(randomWeight);

				PlaceCard(activeCards[i].gameObject, cardPositions[i].transform);
				SoundManager.Instance.PlayRandomSound(cardSlideSFX);
				yield return new WaitForSeconds(timeToWait);
			}

			RevealCard(0);
			yield return new WaitForSeconds(timeToWait);

			cardsDealtEvent.Raise();
		}

		//Only called from the Pull Random Cards method to add all the weights of our cards in the deck
		//and get the heaviest weights more often
		int CheckWeights()
		{
			int totalWeight = 0;
			for(int i = 0; i < Deck.Count; i++)
			{
				totalWeight += Deck[i].cardData.CardWeight;
			}

			int randomWeight = Random.Range(0, totalWeight);
			int index = 0;

			for(int i = 0; i < Deck.Count; i++)
			{
				index += Deck[i].cardData.CardWeight;
				if(randomWeight < index)
				{
					return Deck[i].cardData.CardWeight;
				}
			}

			return Random.Range(0, Deck.Count);
		}
		
		//Simply move a selected card to a destination, mainly pulling cards and
		//putting them back into the deck
		void PlaceCard(GameObject cardToMove, Transform destination, float timeToMove = 0.3f)
		{
			LeanTween.move(cardToMove, destination.position, timeToMove).setEaseInOutSine();
		}

		public void UpdateCardIndex(int index)
		{
			cardBackIndex += index;

			if(cardBackIndex > cardBacks.Length - 1)
			{
				cardBackIndex = 0;
			}
			else if(cardBackIndex < 0)
			{
				cardBackIndex = cardBacks.Length - 1;
			}
			activeCardBack = cardBacks[cardBackIndex];
			SetCardBacks();
		}

		//Grab all of the active cards and the cardsin the deck, then change their cardbacks
		void SetCardBacks()
		{
			foreach(Card card in Deck)
			{
				card.cardBack.sprite = activeCardBack;
			}

			foreach(Card card in activeCards)
			{
				if(card != null)
				{
					card.cardBack.sprite = activeCardBack;
				}
			}
		}
		
		//Send which card you want revealed
		//Then call it's Reveal Method
		void RevealCard(int cardToReveal)
		{
			if(activeCards[cardToReveal] != null && activeCards[cardToReveal].canRotate)
			{
				activeCards[cardToReveal].RevealCard();
			}
		}

		//Using an event, we unreveal cards when we reset the game
		void UnRevealCards()
		{
			for(int i = 0; i < activeCards.Length; i++)
			{
				if(activeCards[i] != null && activeCards[i].canRotate)
				{
					activeCards[i].UnRevealCard();
				}
			}
		}

		//Here we will check the active cards values and suits, then reveal the players card
		//Next we compare the values, then suits, and call the check for win coroutine
		//based on the result. Bigger suit wins a draw
		public void CompareCardValues(bool choiceIsHigher)
		{
			int firstCompareValue = activeCards[0].cardData.CardValue;
			int secondCompareValue = activeCards[1].cardData.CardValue;
			bool cardValueIsHigher = false;

			int firstCardSuit = (int)activeCards[0].cardData.cardSuit;
			int secondCardSuit = (int)activeCards[1].cardData.cardSuit;
			bool suitValueIsHigher = false;

			RevealCard(1);

			if(firstCompareValue > secondCompareValue)
			{
				cardValueIsHigher = true;
				Debug.Log("Your ard is higher in value!");
			}
			else if(firstCompareValue < secondCompareValue)
			{
				Debug.Log("Your card is lower in value..");
			}

			else if(firstCompareValue == secondCompareValue)
			{
				Debug.Log("Value is a tie!");
				if(firstCardSuit > secondCardSuit)
				{
					cardValueIsHigher = true;
					suitValueIsHigher = false;
				}

				StartCoroutine(CheckForWin(cardValueIsHigher, choiceIsHigher, suitValueIsHigher));
				return;
			}

			if(firstCardSuit > secondCardSuit)
			{
				Debug.Log("Card suit was higher in value!");
				suitValueIsHigher = false;
			}
			else
			{
				Debug.Log("Card suit was lower in value..");
				suitValueIsHigher = true;
			}

			StartCoroutine(CheckForWin(cardValueIsHigher, choiceIsHigher, suitValueIsHigher));
		}

		//We use compare values to check the values and tell this method who won
		//This will raise a win or lose event based on the result
		IEnumerator CheckForWin(bool firstCardValueIsHigher, bool choiceIsHigher, bool suitValueIsHigher, float timeToWait = 0.5f)
		{
			WaitForSeconds waitTimer = new WaitForSeconds(timeToWait);
			yield return waitTimer;

			playerCardRevealedEvent.Raise();

			if(firstCardValueIsHigher && !choiceIsHigher)
			{
				SoundManager.Instance.PlaySingleClip(winSFX);
				winEvent.Raise();
				cardValueWasLowerEvent.Raise();
			}
			else if(firstCardValueIsHigher && choiceIsHigher)
			{
				SoundManager.Instance.PlaySingleClip(loseSFX);
				loseEvent.Raise();
				cardValueWasLowerEvent.Raise();
			}
			else if(!firstCardValueIsHigher && !choiceIsHigher)
			{
				SoundManager.Instance.PlaySingleClip(loseSFX);
				loseEvent.Raise();
				cardValueWasHigherEvent.Raise();
			}
			else if(!firstCardValueIsHigher && choiceIsHigher)
			{
				SoundManager.Instance.PlaySingleClip(winSFX);
				winEvent.Raise();
				cardValueWasHigherEvent.Raise();
			}

			if(suitValueIsHigher)
			{
				suitValueWasHigherEvent.Raise();
			}
			else
			{
				suitvalueWasLowerEvent.Raise();
			}
		}


		//Wrapper for our ResetRoutine to be called from an event
		public void ResetGame()
		{
			StartCoroutine(ResetRoutine());
		}

		//We use an Event to alert all listening objects to Reset in various ways
		//Here we just return all the cards to the deck, and make them face down
		//Finally shuffling the deck afterwards
		IEnumerator ResetRoutine()
		{
			float timeToMove = 0.3f;

			if(activeCards != null)
			{
				for(int i = 0; i < activeCards.Length; i++)
				{
					if(activeCards[i] != null)
					{
						activeCards[i].Reset();
						yield return new WaitForSeconds(timeToMove);
						LeanTween.move(activeCards[i].gameObject, deckTransform.position, timeToMove);
						SoundManager.Instance.PlayRandomSound(cardSlideSFX);
						Deck.Add(activeCards[i]);
						activeCards[i] = null;
					}
				}
			}

			ShuffleDeck(Deck);
		}

	}
}