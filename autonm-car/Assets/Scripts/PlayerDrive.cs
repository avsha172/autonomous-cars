using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrive : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam;
    public bool playerDrive = false;
    [SerializeField] private float MaxSteeringAngale;
    private DriveSim SimRef;
    [SerializeField] private float speed;
    private bool PlayerHit = false;
    private bool CamAsDriver = true; // should do it as delegates instead 
    void Start()
    {
        SimRef = FindObjectOfType<DriveSim>();
    }

    public IEnumerator PlayerDrives()
    {
        while (playerDrive)
        {
            if(Input.GetKeyUp("space")){
                CamAsDriver = !CamAsDriver;
            }
            if (!PlayerHit)
            {
                
                float output = Input.GetAxis("Horizontal");
                transform.rotation = Quaternion.Euler(0, (float)(transform.rotation.eulerAngles.y + output * Time.deltaTime * MaxSteeringAngale / 2), 0);
                transform.position += transform.forward * Time.deltaTime * speed / 2;
                if(CamAsDriver){
                    cam.position = transform.position + transform.forward * 0.2f + new Vector3(0, 1.4f, 0);
                }
                else{
                    cam.position = transform.position - transform.forward * 8 + new Vector3(0, 2.5f,0);
                }
                cam.rotation = transform.rotation;
            }
            yield return new WaitForSeconds(1f / 60);
        }
    }
    public void StartDrive()
    {
        playerDrive = true;
        PlayerHit = false;
        StartCoroutine(PlayerDrives());
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!PlayerHit)
        {
            PlayerHit = true;
            playerDrive = false;
            SimRef.NewRound();
        }
    }
}