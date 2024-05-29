using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoseManager : MonoBehaviour
{
    public TMPro.TMP_Dropdown GearBoxDropdown;
    public TMPro.TMP_Dropdown TransmissionDropdown;
    private void Update()
    {
        if (GearBoxDropdown.options[GearBoxDropdown.value].text == "Auto GearBox")
        {
            PlayerPrefs.SetString("gearBox", "auto");
        }
        else if (GearBoxDropdown.options[GearBoxDropdown.value].text == "Manual GearBox")
        {
            PlayerPrefs.SetString("gearBox", "manual");
        }

        if (TransmissionDropdown.options[TransmissionDropdown.value].text == "Rear Wheels")
        {
            PlayerPrefs.SetString("transmission", "rear");
        }
        else if (TransmissionDropdown.options[TransmissionDropdown.value].text == "Front Wheels")
        {
            PlayerPrefs.SetString("transmission", "front");
        }
        else if (TransmissionDropdown.options[TransmissionDropdown.value].text == "All Wheels")
        {
            PlayerPrefs.SetString("transmission", "all");
        }
        PlayerPrefs.Save();
    }
    public void LoadVsAI()
    {
        SceneManager.LoadScene("main");
    }
    public void LoadArcade()
    {
        SceneManager.LoadScene("Arcade");
    }
}
