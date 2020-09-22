using System.Collections;
using UnityEngine;

/// <summary>
/// This class is only responsible for closing the game by selecting the
/// Quit Game Button
/// </summary>
namespace HighOrLow
{
	public class QuitGame : MonoBehaviour
	{
		public void Quit()
		{
			Application.Quit();
		}
	}
}
