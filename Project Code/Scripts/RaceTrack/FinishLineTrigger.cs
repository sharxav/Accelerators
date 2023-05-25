//script that is called when the car crosses the finish line
using UnityEngine;


public class FinishLineTrigger : MonoBehaviour
{

	//called when a car crosses the finish line trigger
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "ColliderFront")
		{

			PlayerPrefs.SetInt("MinSave", RaceTimeManager.MinuteCount);
			PlayerPrefs.SetInt("SecSave", RaceTimeManager.SecondCount);
			PlayerPrefs.SetFloat("MiliSave", RaceTimeManager.MiliCount);
			PlayerPrefs.SetFloat("RawTime", RaceTimeManager.RawTime);

			SavePlayerTime(RaceTimeManager.MinuteCount, RaceTimeManager.SecondCount, RaceTimeManager.MiliCount);

			Debug.Log("Current time:" + PlayerPrefs.GetInt("MinSave") + " " + PlayerPrefs.GetInt("SecSave"));

		}

	}
	//saves the race complete time as a string
    private void SavePlayerTime(int minuteCount, int secondCount, float miliCount)
    {
		PlayerPrefs.SetString("Time", "0" + minuteCount + ":" + secondCount + "." + (int)miliCount);
    }
}
