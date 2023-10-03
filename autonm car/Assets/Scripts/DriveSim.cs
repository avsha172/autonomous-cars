using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveSim : MonoBehaviour
{
    public GameObject Car;
    public static Quaternion StartRotation;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float YRotation;
    public void NewRound()
    {
        Car.transform.rotation = StartRotation;
        Car.transform.position = startPosition;
        StartCoroutine(WaitTillDrive(1f));
    }
    IEnumerator WaitTillDrive(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        Car.GetComponent<PlayerDrive>().StartDrive();
    }
    void Start()
    {
        StartRotation = Quaternion.Euler(0, YRotation, 0);
        Car.transform.rotation = StartRotation;
        Car.transform.position = startPosition; 
        Car.tag = "Car";
        StartCoroutine(WaitTillDrive(1f));
    }
}
