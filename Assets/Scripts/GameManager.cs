using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CarController RR;

    public GameObject needle;
    public TextMeshProUGUI kph;
    public TextMeshProUGUI gearText;
    private float startPosition = 135f, endPosition = -135f;
    private float desiredPosition;

    private GameObject[] presentGameObjectVehicles;
    public List<GameObject> presentVehicles;

    private void Awake()
    {
        RR = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        presentGameObjectVehicles = GameObject.FindGameObjectsWithTag("AI");
        presentVehicles = new List<GameObject>();
        foreach(GameObject R in presentGameObjectVehicles)
            presentVehicles.Add(R);
        presentVehicles.Add(RR.gameObject);
    }

    private void FixedUpdate()
    {
        kph.text = RR.KPH.ToString("0");
        updateNeedle();
    }

    public void updateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = RR.engineRPM / 10000;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition));
    }

    public void changeGear()
    {
        gearText.text = (!RR.reverse) ? (RR.gearNum + 1).ToString() : "R";
    }
}
