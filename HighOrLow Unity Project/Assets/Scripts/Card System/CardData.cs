using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be a scriptable object that we can create in the project
/// and reference from our Card Controller to assign values to a single card prefab 
/// and will get it's data from this class
/// </summary>
namespace HighOrLow
{
	public enum CardSuit
	{
		DIAMOND,
		CLUB,
		HEART,
		SPADE
	}

	[CreateAssetMenu(menuName = "Cards/Card Data")]
	public class CardData : ScriptableObject 
	{
		public string CardName;
		public int CardValue;
		public CardSuit cardSuit;
		public int CardWeight;
		public Sprite cardFront;
	}
}

