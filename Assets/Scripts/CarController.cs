using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    internal enum driveType 
    { 
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    internal enum gearBox 
    {
        automatic,
        manual
    }

    [SerializeField] private driveType drive;
    [SerializeField] private gearBox gearChange;

    [Header("Variables")]
    public float handBrakeFrictionMultiplier = 2f;
    public float totalPower;
    public float KPH;
    public float wheelsRPM;
    public float engineRPM;

    public float maxRPM, minRPM;
    public float[] gears;
    public int gearNum;
    public bool reverse = false;

    public AnimationCurve enginePower;

    private InputManager IM;
    public GameManager manager;

    private GameObject wheelMeshes, wheelColliders;
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] meshes = new GameObject[4];
    private GameObject centerOfMass;
    private Rigidbody rigidbody;

    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    private float radius = 6, brakePower = 50000, downforceValue = 10f, smoothTime = 0.09f, driftFactor;

    [Header("DEBUG")]
    public float[] slip = new float[4];

    private void Start()
    {
        getObjects();
    }

    private void FixedUpdate()
    {
        shifter();
        addDownForce();
        MoveVehicle();
        SteerVehicle();
        animateWheels();
        //getFriction();
        calculateEnginePower();
        adjustTraction();
    }

    private void calculateEnginePower()
    {
        wheelRPM();

        totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * IM.vertical;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
    }

    public void shifter()
    {
        if (!isGrounded()) return;

        if (gearChange == gearBox.automatic)
        {
            if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !reverse)
            {
                gearNum++;
                manager.changeGear();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && gearNum < gears.Length - 1)
            {
                gearNum++;
                manager.changeGear();
            }
        }
        if (engineRPM < minRPM && gearNum > 0)
        {
            gearNum--;
            manager.changeGear();
        }
    }

    private bool isGrounded()
    {
        if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
            return true;
        else 
            return false;
    }

    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for(int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;

        if(wheelsRPM < 0 && !reverse)
        {
            reverse = true;
            manager.changeGear();
        }
        else if(wheelsRPM > 0 && reverse)
        {
            reverse = false;
            manager.changeGear();
        }
    }

    private void MoveVehicle()
    {
        if (drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = (totalPower / 4);
            }
        }
        else if(drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = (totalPower / 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = (totalPower / 2);
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;
    }

    private void SteerVehicle()
    {
        if(IM.horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if(IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }

    private void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            meshes[i].transform.position = wheelPosition;
            meshes[i].transform.rotation = wheelRotation;
        }
    }

    private void addDownForce()
    {
        rigidbody.AddForce(-transform.up * downforceValue * rigidbody.velocity.magnitude);
    }

    private void getFriction()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.sidewaysSlip;
        }
    }

    private void getObjects()
    {
        IM = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();

        wheelColliders = GameObject.Find("wheelColliders");
        wheelMeshes = GameObject.Find("wheelMeshes");

        /*if(gameObject.tag == "AI")
        {
            meshes[0] = wheelMeshes.transform.Find("0").gameObject;
            meshes[1] = wheelMeshes.transform.Find("1").gameObject;
            meshes[2] = wheelMeshes.transform.Find("2").gameObject;
            meshes[3] = wheelMeshes.transform.Find("3").gameObject;

            wheels[0] = wheelColliders.transform.Find("0").GetComponent<WheelCollider>();
            wheels[1] = wheelColliders.transform.Find("1").GetComponent<WheelCollider>();
            wheels[2] = wheelColliders.transform.Find("2").GetComponent<WheelCollider>();
            wheels[3] = wheelColliders.transform.Find("3").GetComponent<WheelCollider>();
        }*/
        

        centerOfMass = GameObject.Find("center of mass");
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void adjustTraction()
    {
        //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

        if (IM.handbrake)
        {
            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier, ref velocity, driftSmothFactor);

            for (int i = 0; i < 4; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }
            rigidbody.AddForce(transform.forward * (KPH / 400) * 10000);
        }
        //executed when handbrake is being held
        else
        {

            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((KPH * handBrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;

            }
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {

            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
            //smoke
            if (wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f || wheelHit.forwardSlip >= .3f || wheelHit.forwardSlip <= -0.3f)
                playPauseSmoke = true;
            else
                playPauseSmoke = false;


            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
        }

    }

    [HideInInspector] public bool playPauseSmoke = false;
    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;
        }
    }
}