using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject Player;
    private CarController RR;
    private GameObject child;
    private float speed;
    public float defaulFOV, desiredFOV = 0;
    [Range(0, 2)]public float smoothTime = 0;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        RR = Player.GetComponent<CarController>();
        child = Player.transform.Find("camera constraint").gameObject;
        defaulFOV = Camera.main.fieldOfView;
    }

    private void FixedUpdate()
    {
        Follow();
        BoostFOV();
    }

    void Follow()
    {
        speed = Mathf.Lerp(speed, RR.KPH / 4, Time.deltaTime);

        gameObject.transform.position = Vector3.Lerp(transform.position, child.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.transform.position);
    }

    void BoostFOV()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFOV, Time.deltaTime * smoothTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaulFOV, Time.deltaTime * smoothTime);
        }
    }
}
