using System.Collections.Generic;
using UnityEngine;

///<summary>
///This class will be a scriptable object event for game objects to listen to
/// Anytime we create an event, we can raise the event from the object this
/// is attached to or another object
///</summary>
namespace HighOrLow
{
	[CreateAssetMenu(menuName = "Events/Game Event")]
	public class GameEvent : ScriptableObject
	{
		List<EventListener> eventListeners = new List<EventListener>();

		public void Raise()
		{
			Debug.Log(this.name + " Raised");
			for (int i = 0; i < eventListeners.Count; i++)
			{
				eventListeners[i].OnEventRaised();
				Debug.Log(eventListeners[i].name + " is listening to this event");
			}
		}

		public void Register(EventListener listener)
		{
			if (!eventListeners.Contains(listener))
			{
				eventListeners.Add(listener);
			}
		}

		public void DeRegister(EventListener listener)
		{
			if (eventListeners.Contains(listener))
			{
				eventListeners.Remove(listener);
			}
		}
	}
}