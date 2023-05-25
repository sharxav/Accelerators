//script that generates a random lobby code
using UnityEngine;


public class RandomCode : MonoBehaviour
{
    char[] JoinCode = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public void GenerateCode()
    {

        string code = "";
        for(int i=0;i<5;i++)
        {
            code+= JoinCode[Random.Range(0, JoinCode.Length)];
        }
       
        Debug.Log("Join Code:" + code);
        PlayerPrefs.SetString("LobbyCode", code);
        
    }

   
}
