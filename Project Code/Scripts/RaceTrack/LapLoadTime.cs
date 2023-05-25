//script that displays the best time
using UnityEngine;
using UnityEngine.UI;

public class LapLoadTime : MonoBehaviour
{
	private int MinCount;
	private int SecCount;
	private float MiliCount;
	[SerializeField] private GameObject MinDisplay;
	[SerializeField] private GameObject SecDisplay;
	[SerializeField] private GameObject MiliDisplay;

    void Start()
    {

		MinCount = PlayerPrefs.GetInt("MinSave");
		SecCount = PlayerPrefs.GetInt("SecSave");
		MiliCount = PlayerPrefs.GetFloat("MiliSave");

		if (SecCount <= 9)
			{
				SecDisplay.GetComponent<Text>().text = "0" + SecCount + ".";
			}
			else
			{
				SecDisplay.GetComponent<Text>().text = "" + SecCount + ".";
			}
			if (MinCount <= 9)
			{
				MinDisplay.GetComponent<Text>().text = "0" + MinCount + ":";
			}
			else
			{
				MinDisplay.GetComponent<Text>().text = "" + MinCount + ":";
			}
			MiliDisplay.GetComponent<Text>().text = "" + MiliCount;
		}
	
    }


