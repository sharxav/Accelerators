//script that handles the speedometer needle rotation
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Rotation : MonoBehaviour
{
    private float speed;
    private void Update()
    {
        float speed = CarController.Instance.CurrentSpeed;
        //Debug.Log("Speed = " + speed);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(218.429f, -37.856f, speed / 180));
    }
}
