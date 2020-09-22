using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class will just be a component we can add to an object if we want some notes
/// on what the object contains or what it does.
/// This is especially nice for anyone coming from outside the project
/// </summary>
namespace HighOrLow
{
	public class Notes : MonoBehaviour
	{
		[TextArea]
		public string GameObjectNotes;
	}
}