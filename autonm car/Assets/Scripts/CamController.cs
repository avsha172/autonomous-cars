/* This class contains all of the logic related to the camera control in game view
 * It follows car with the highest fitness function and has three points of views.
 * Birds-eye from the right, birds-eye from the left, and third person. 
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CamController : MonoBehaviour
{
    // Constants

    public Transform CamTransform;
    public Vector3 CamRotation;

    public Vector3[] ZoomCamPosition;
    public float[] ZoomSightDistance;
    public float[] ZoomFieldOfView;
    [SerializeField] private bool EnableDrive;
    public enum CameraPos { FromAbove = 0, FirstPerson = 1, DriveFirstPerson = 2 };
    [SerializeField] private LayerMask InvsLayer;
    public Camera camera;

    public GameObject objectToFollow;
    public GameObject ObjectToFollow
    {
        get { return objectToFollow; }

        set {
            if (objectToFollow != null)
            {
                ShowCars(true);
            }
            objectToFollow = value;
            if (pos == (int) CameraPos.FirstPerson)
            {
                HideCars(objectToFollow);
            }
            else if (pos == (int) CameraPos.DriveFirstPerson)
            {
                HideCars();
            }
        }
    }
    public const float DrivingSightDist = 20f;
    public Vector3 offset;
    private int pos;
    public RoadHandler SimRef;
    // Rotate camera towards the target
    public void LookFirstPerson()
    {
        transform.position = objectToFollow.transform.position - objectToFollow.transform.forward * 8 + new Vector3(0, 2.5f,0);
        transform.rotation = Quaternion.Euler(0, objectToFollow.transform.rotation.eulerAngles.y, 0);
    }
    public void LookAsDriver()
    {
        transform.position = objectToFollow.transform.position + objectToFollow.transform.forward * 0.2f + new Vector3(0, 1.4f, 0);
        transform.rotation = Quaternion.Euler(0, objectToFollow.transform.rotation.eulerAngles.y, 0);

    }

    private void SetLayerRecursively(GameObject obj, int Layer)
    {
        obj.layer = Layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, Layer);
        }
    }
    private void HideCars(GameObject MyCar = null)
    {
        GameObject[] Cars = SimRef.cars;
        foreach(GameObject car in Cars)
        {
            if(car != MyCar)
            {
                SetLayerRecursively(car, 10);
            }
        }
    }

    private void ShowCars(bool OnlyObjToFollow = false)
    {
        if (OnlyObjToFollow)
        {
            SetLayerRecursively(ObjectToFollow, 9);
        }
        else
        {
            GameObject[] Cars = SimRef.cars;
            foreach (GameObject car in Cars)
            {
                SetLayerRecursively(car, 9);
            }
        }
    }

    void Start()
    {
        pos = 0;
        camera = GetComponent<Camera>();
    }

    // Check for camera angle changes from the user and update
    // camera position accordingly
    void Update()
    {
        int layerToIgnore = 10;
        int layerMask = ~(1 << layerToIgnore);
        camera.cullingMask = layerMask;
        if (Input.GetKeyDown("space"))
        {
            camera.fieldOfView = ZoomFieldOfView[0];
            pos = (pos + 1) % 3;
            Debug.Log(pos);
            if (pos == (int)CameraPos.DriveFirstPerson)
            {
                if(objectToFollow != null)
                {
                    ChangeClippingPlane(DrivingSightDist);
                    HideCars();

                }
                else
                {
                    pos = (pos - 1) % 3;
                }
            }
            else if (pos == (int) CameraPos.FirstPerson)
            {
                if (objectToFollow != null)
                {
                    ChangeClippingPlane(DrivingSightDist);
                    ShowCars(false);
                    HideCars(objectToFollow);
                }
                else
                {
                    pos = (pos - 1 ) % 3;
                }
            }
            else if (pos == (int)CameraPos.FromAbove)
            {
                Debug.Log(ZoomCamPosition[0]);
                transform.position = ZoomCamPosition[0];
                ChangeClippingPlane(ZoomSightDistance[0]);
                transform.rotation = Quaternion.Euler(CamRotation.x, CamRotation.y, CamRotation.z);
                ShowCars();
            }
        }

        if (objectToFollow != null)
        {
            if (pos == (int)CameraPos.FirstPerson)
            {
                LookFirstPerson();
            }
            else if (pos == (int)CameraPos.DriveFirstPerson)
            {
                LookAsDriver();
            }
        }
    }
    
    private void ChangeClippingPlane(float FarClipPlane)
    {
        camera.farClipPlane = FarClipPlane;
        camera.nearClipPlane = FarClipPlane - 19.7f;
    }
    // Update the cameras position and location on every 

    public void RoadDropDown(TMP_Dropdown RoadOption)
    {
        if(pos == (int)CameraPos.FromAbove)
        {
            int RoadNum = RoadOption.value;
            camera.fieldOfView = ZoomFieldOfView[RoadNum];
            CamTransform.position = ZoomCamPosition[RoadNum];
            ChangeClippingPlane(ZoomSightDistance[RoadNum]);
        }
    }
}