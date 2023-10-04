using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using Barmetler.RoadSystem;
public class RoadHandler : MonoBehaviour
{
    public bool TestOnOtherRoads;
    public string fileName = "C:/Users/Avshalom/github/autonm car/UnityData/524a479f-7d75-4dee-a4b5-5ddc021efd54/weights.txt";
    public bool TestWeights = false;
    private CamController camcontroller;
    public CamController camController
    {
        get
        {
            return camcontroller;
        }
        set
        {
            camcontroller = value;
            camcontroller.SimRef = GetComponent<RoadHandler>();
        }
    }
    [SerializeField] private int FPS;
    public Vector3[] pointsArray;
    public GameObject carPrefab;
    public int CarNum;
    private Barmetler.RoadSystem.NavigationLineUpdater NaviGationScript;
    public Barmetler.RoadSystem.NavigationLineUpdater[] NaviGationScriptArray;
    private RoadSystemNavigator navigator;
    public Vector3 StartPosition;
    public Quaternion StartRotation
    {
        get { return Quaternion.Euler(0, YRotation, 0); }
    }
    public Vector3[] startPositionArray;
    [SerializeField] private float YRotation;
    public float[] YRotationArray;
    [SerializeField] private float mutationRate;
    private int Generation = -1;
    public GameObject[] cars;
    [SerializeField] private GraphDataSaver DataSaver;
    [SerializeField] private int aliveCars;
    public int HomeTrack;
    [SerializeField] public int currTrack;
    public bool[] AllTracks;
    public GameObject[] TestingCars;
    public int AliveCars
    {
        get
        {
            return aliveCars;
        }
        set
        {

            if (aliveCars > 1)
            {
                aliveCars = value;
            }
            else
            {
                if(currTrack == HomeTrack)
                {
                    aliveCars = CarNum;
                    Generation += 1;
                    if (TestWeights)
                    {
                        NewRoundTestWeights(cars);
                        for(int k = 0; k < cars.Length; k++)
                        {
                            cars[k].GetComponent<CarControlls>().enabled = true;
                        }
                    }
                    else
                    {
                        NewRound();
                    }
                }
                else
                {

                    NewRoundTestWeights(TestingCars);
                    TestOtherRoads(TestingCars);
                }
            }
        }
    }

    void NewRoundTestWeights(GameObject[] Cars)
    {
        Vector3[] points = NaviGationScript.points;
        SaveOtherTracks(points, Cars);
        if (camController && Cars.Length > 0)
        {
            camController.ObjectToFollow = Cars[0];
        }
    }
    void NewRound()
    {
        //maybe wait a few sec
        pointsArray = NaviGationScript.points;
        Debug.Log("NEW ROUND TRACK: " + HomeTrack.ToString());
        int winnerNum = (int)(CarNum / 4);
        if (winnerNum < 1)
        {
            winnerNum = 1;
        }

        GameObject[] FarthestCars = FindClosestObjects(pointsArray, cars, winnerNum);
        if (camController)
        {
            camController.ObjectToFollow = FarthestCars[0];
        }
        //Debug.Log(FarthestCars.Length);
        bool DontChangeWeights;
        int CurrWinnerGenom = 0;
        foreach (GameObject car in cars)
        {
            DontChangeWeights = false;
            foreach (GameObject car1 in FarthestCars)
            {
                if (car1 == car)
                {
                    DontChangeWeights = true;
                    break;
                }
            }
            if (!DontChangeWeights)
            {
                if (CurrWinnerGenom < winnerNum)
                {
                    CarControlls OldCarScript = FarthestCars[CurrWinnerGenom % winnerNum].GetComponent<CarControlls>();
                    CarControlls NewCarScript = car.GetComponent<CarControlls>();
                    NewCarScript.MutateWeights(OldCarScript.hiddenWeights, OldCarScript.inputWeights, mutationRate);
                }
                else if (CurrWinnerGenom < winnerNum * 2) //i dont want crossover
                {
                    CarControlls OldCarScript1 = FarthestCars[CurrWinnerGenom % winnerNum].GetComponent<CarControlls>();
                    CarControlls OldCarScript2 = FarthestCars[(CurrWinnerGenom + 1) % winnerNum].GetComponent<CarControlls>();
                    CarControlls NewCarScript = car.GetComponent<CarControlls>();
                    NewCarScript.CrossOver(OldCarScript1, OldCarScript2);
                }
                else
                {
                    car.GetComponent<CarControlls>().RandomWeights();
                }
                if (TestOnOtherRoads && Generation  % 5 == 0)
                {
                    car.transform.position = new Vector3(1000, 0, 1000);
                }
                CurrWinnerGenom++;
            }
        }
        if(Generation < 70  )
        {
            if (TestOnOtherRoads && Generation % 5 == 0)
            {
                SaveOtherTracks(pointsArray, FarthestCars);
                TestOtherRoads(FarthestCars);
            }
            else
            {
                foreach (GameObject car in cars)
                {
                    car.GetComponent<CarControlls>().enabled = true;
                }
            }
        }
        else
        {
            foreach (GameObject car in cars)
            {
                car.transform.position = new Vector3(1000, 0, 1000);
            }
        }
    }

    private void TestOtherRoads(GameObject[] Cars)
    {

        currTrack = (currTrack + 1) % AllTracks.Length;
        while (!AllTracks[currTrack])
        {
            currTrack = (currTrack + 1) % AllTracks.Length;
        }
        StartPosition = startPositionArray[currTrack];
        YRotation = YRotationArray[currTrack];
        NaviGationScript = NaviGationScriptArray[currTrack];
        if(currTrack == HomeTrack)
        {
            aliveCars = CarNum;
            Cars = cars;
        }
        else
        {
            TestingCars = Cars;
            aliveCars = Cars.Length;
        }   
        foreach (GameObject car in Cars)
        {
            CarControlls carScript = car.GetComponent<CarControlls>();
            carScript.enabled = true;
        }
    }
    private void SaveOtherTracks(Vector3[] points, GameObject[] Cars)
    {
        float IndexsSum = 0;
        float[] IndexsArray = new float[Cars.Length];
        int n = 0;
        foreach (GameObject Car in Cars)
        {
            float ClosestIndex = 0;
            float Closest = Vector3.Distance(Car.transform.position, points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                float Curr = Vector3.Distance(Car   .transform.position, points[i]);
                if (Closest >= Curr)
                {
                    Closest = Curr;
                    ClosestIndex = i;
                }
            }
            IndexsSum += ClosestIndex;
            IndexsArray[n] = ClosestIndex/points.Length*100;
            n ++;   
        }
        IndexsSum = IndexsSum/Cars.Length/points.Length*100;
        DataSaver.SaveGraphData(string.Join(",", IndexsArray), "farthest_reaches_other_tracks", currTrack);
        DataSaver.SaveGraphData((IndexsSum).ToString(), "distance_reached_average_other_tracks", currTrack);
    }
    private GameObject[] FindClosestObjects(Vector3[] points, GameObject[] Cars, int numObjects, bool DifferentTracks = false)
    {
        int[] FarthestPoints = new int[numObjects];
        GameObject[] FarthestCars = new GameObject[numObjects];
        float[] ClosestIndexs = new float[Cars.Length];
        float FarthestPointsSum = 0;
        for(int k = 0; k < Cars.Length; k++)
        {
            int ClosestIndex = 0;
            float Closest = Vector3.Distance(Cars[k].transform.position, points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                float Curr = Vector3.Distance(Cars[k].transform.position, points[i]);
                if (Closest >= Curr)
                {
                    Closest = Curr;
                    ClosestIndex = i;
                }
            }
            Cars[k].GetComponent<CarControlls>().IndexReached = ClosestIndex;
            FarthestPointsSum += ClosestIndex;
            ClosestIndexs[k] = (float) ClosestIndex / points.Length * 100;
        }
        Cars = Cars.OrderByDescending(obj => obj.GetComponent<CarControlls>().IndexReached).ToArray();
        for(int n = 0; n < numObjects; n++)
        {
            FarthestCars[n] = Cars[n];
        }
        DataSaver.SaveGraphData(string.Join(",", ClosestIndexs), "farthest_reaches");
        DataSaver.SaveGraphData((FarthestPointsSum / points.Length / cars.Length * 100).ToString(), "distance_reached_average");
        return FarthestCars;
    }
    void Start()
    {
        currTrack = HomeTrack;
        YRotation = YRotationArray[HomeTrack];
        StartPosition = startPositionArray[HomeTrack];
        NaviGationScript = NaviGationScriptArray[HomeTrack];
        QualitySettings.vSyncCount = 0;
        fileName = "C:/Users/Avshalom/github/autonm car/UnityData/" + fileName + "/Weights.txt";
        Application.targetFrameRate = FPS;
        aliveCars = CarNum;
        cars = new GameObject[CarNum];
        Physics.queriesHitBackfaces = true;
        StartCoroutine(WaitTillCall(1f));
    }
    IEnumerator WaitTillCall(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        if (TestWeights)
        {
            LoadWeights();
        }
        else
        {
            SpawnCars(CarNum);
        }
    }
    void SpawnCars(int CarCount)
    {
        for (int i = CarNum - 1; i > CarNum - CarCount - 1; i--)
        {
            GameObject car = Instantiate(carPrefab, StartPosition, StartRotation);
            car.tag = "Car";
            cars[i] = car;
            CarControlls NewCarScript = car.GetComponent<CarControlls>();
            NewCarScript.RandomWeights();
            NewCarScript.SimRef = GetComponent<RoadHandler>();
            NewCarScript.MaxSteeringAngale = SimHandler.MaxSteeringAngale;
            NewCarScript.Speed = SimHandler.Speed;
    //car.hi = 1f;
}
        CarControlls carScript = cars[0].GetComponent<CarControlls>();
        DataSaver.SaveGraphData("MaxSteeringAngale: " + carScript.MaxSteeringAngale.ToString(), "meta_data");
        DataSaver.SaveGraphData("probingDistance: " + carScript.probingDistance.ToString(), "meta_data");
        DataSaver.SaveGraphData("speed: " + carScript.Speed.ToString(), "meta_data");
        DataSaver.SaveGraphData("inputSize: " + carScript.inputSize.ToString(), "meta_data");
        DataSaver.SaveGraphData("hiddenSize: " + carScript.hiddenSize.ToString(), "meta_data");
        DataSaver.SaveGraphData("outputSize: " + carScript.outputSize.ToString(), "meta_data");
        if (TestOnOtherRoads)
        {
            DataSaver.SaveGraphData("AllTracks: " + AllTracks.ToString(), "meta_data");
        }
    }
    void LoadWeights()
    {
        using (StreamReader reader = new StreamReader(fileName))
        {
            int lines1 = 6;
            int lines2 = 30;
            int NumberOfCars = 0;
            while (reader.Peek() >= 0) // Check if the reader can still read a line
            {
                reader.ReadLine();
                NumberOfCars++;
            }
            NumberOfCars = NumberOfCars / (lines1 + lines2 + 4);
            if(NumberOfCars > CarNum)
                {
                NumberOfCars = CarNum;
                }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            CarNum = NumberOfCars;
            AliveCars = NumberOfCars;
            cars = new GameObject[NumberOfCars];
            for (int i = 0; i < NumberOfCars; i++)
            {
                GameObject car = Instantiate(carPrefab, StartPosition, StartRotation);
                car.tag = "Car";
                cars[i] = car;
                CarControlls carScript = car.GetComponent<CarControlls>();
                int n = 2;
                while (n > 0)
                {
                    n -= 1;
                    string line1 = reader.ReadLine();
                    string line2 = reader.ReadLine();
                    int dim1 = int.Parse(line1);
                    int dim2 = int.Parse(line2);

                    float[,] array = new float[dim1, dim2];

                    for (int j = 0; j < dim1; j++)
                    {
                        string line = reader.ReadLine();
                        string[] lineValues = line.Split(' ');

                        for (int k = 0; k < dim2; k++)
                        {
                            array[j, k] = float.Parse(lineValues[k]);
                        }
                    }
                    if (n == 1)
                    {
                        carScript.inputWeights = array;
                    }
                    else
                    {
                        carScript.hiddenWeights = array;
                    }
                }

            }

            CarControlls randCarScript = cars[0].GetComponent<CarControlls>();
            DataSaver.SaveGraphData("MaxSteeringAngale: " + randCarScript.MaxSteeringAngale.ToString(), "meta_data");
            DataSaver.SaveGraphData("probingDistance: " + randCarScript.probingDistance.ToString(), "meta_data");
            DataSaver.SaveGraphData("speed: " + randCarScript.Speed.ToString(), "meta_data");
            DataSaver.SaveGraphData("inputSize: " + randCarScript.inputSize.ToString(), "meta_data");
            DataSaver.SaveGraphData("hiddenSize: " + randCarScript.hiddenSize.ToString(), "meta_data");
            DataSaver.SaveGraphData("outputSize: " + randCarScript.outputSize.ToString(), "meta_data");
            DataSaver.SaveGraphData("track_number: " + HomeTrack.ToString(), "meta_data");
        }
    }
} 
