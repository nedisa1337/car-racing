using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public GameObject winText;
    public GameObject loseText;
    private bool finished = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && finished == false)
        {
            winText.SetActive(true);
            finished = true;
        }
        else if (other.gameObject.tag == "AI" && finished == false)
        {
            loseText.SetActive(true);
            finished = true;
        }
        Invoke(nameof(ReturnToHub), 3f);
    }

    private void ReturnToHub()
    {
        SceneManager.LoadScene("Hub");
    }
}
