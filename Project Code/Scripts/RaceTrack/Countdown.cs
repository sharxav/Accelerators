//script to handle the countdown animation
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
	[SerializeField] private GameObject CountDown;
	[SerializeField] private GameObject LapTimer;
	[SerializeField] private GameObject CarControls;
	[SerializeField] private AudioSource Ready;
	[SerializeField] private AudioSource Go;
	
    void Start()
    {
		StartCoroutine(CountStart());
    }

	IEnumerator CountStart(){
		LapTimer.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		CountDown.GetComponent<Text>().text = "3";
		Ready.Play();
		CountDown.SetActive(true);

		yield return new WaitForSeconds(1);
		CountDown.SetActive(false);
		CountDown.GetComponent<Text>().text= "2";
		Ready.Play();
		CountDown.SetActive(true);

		yield return new WaitForSeconds(1);
		CountDown.SetActive(false);
		CountDown.GetComponent<Text>().text = "1";
		Ready.Play();
		CountDown.SetActive(true);

		yield return new WaitForSeconds(1);
		CountDown.SetActive(false);
		Go.Play();
		LapTimer.SetActive(true);  //starts the race timer
		CarControls.SetActive(true); //enables the script that handles the car user controls
		
	}
}
