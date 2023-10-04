using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveToFile: MonoBehaviour
{
    [SerializeField] public GraphDataSaver DataSaver;
    private List<GameObject> AlreadyWon = new List<GameObject>();
    [SerializeField] private int CarNum;
    private int OtherTracKWinner = 0;
    void OnCollisionEnter(Collision collision)
    {
        if(AlreadyWon.Count == CarNum)
        {
            //ChangeCourse();
        }
        else
        {
            CarControlls CarScript = collision.gameObject.GetComponent<CarControlls>();
            //Debug.Log(gameObject.ToString() + CarScript.SimRef.HomeTrack.ToString() + OtherTracKWinner.ToString());
            if (CarScript.SimRef.HomeTrack == CarScript.SimRef.currTrack)
            {
                CarScript.DontChangeWeights = true;
                if (!AlreadyWon.Contains(collision.gameObject))
                {
                    OtherTracKWinner += 1;
                    DataSaver.SaveWeights(CarScript.inputWeights);
                    DataSaver.SaveWeights(CarScript.hiddenWeights);
                    AlreadyWon.Add(collision.gameObject);
                }
            }
            else
            {
                OtherTracKWinner += 1;
            }
        }
    }
}
