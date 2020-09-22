using UnityEngine;
using UnityEngine.Events;

///<summary>
///This class should be attached to a game object that wants to listen for events
///</summary>
namespace HighOrLow
{
	public class EventListener : MonoBehaviour
	{
		//Using events with Unity Events for Decoupling and ease of use
		public GameEvent[] Events;
		public UnityEvent Response;

		//Subscribe to events
		void OnEnable()
		{
			foreach (GameEvent ev in Events) 
			{ 
				ev.Register(this); 
			}
		}

		//Unsubscribe to prevent memory leaks - called before OnDestroy so ubsubbing makes more sense here
		void OnDisable()
		{
			foreach (GameEvent ev in Events) 
			{
				ev.DeRegister(this); 
			}
		}

		public void OnEventRaised() 
		{
			Response.Invoke(); 
		}

	}
}