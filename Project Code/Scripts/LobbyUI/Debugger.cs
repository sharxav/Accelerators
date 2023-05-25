using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Debugger : MonoBehaviour
{
    [SerializeField] private TMP_Text debugTxt;
    [SerializeField] private GameObject debugPanel;
    public static Debugger instance;
    public void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        debugPanel.SetActive(false);
    }
    public void LogInfo(string message)
    {
        debugPanel.SetActive(true);
        ClearLines();

        debugTxt.text =message;
        StartCoroutine(WaitForSec());
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(2);
        debugPanel.SetActive(false);
    }

    private void ClearLines()
    {
        
        debugTxt.text = string.Empty;
        
    }
}
