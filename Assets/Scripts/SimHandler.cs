using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SimHandler : MonoBehaviour
{

    [SerializeField] private CamController camController;
    [SerializeField] private SaveToFile[] Endings;
    [SerializeField] private GameObject roadHandlerPrefab;
    [SerializeField] private bool[] TracksToInclude;
    [SerializeField] private Vector3[] startPositionArray;
    [SerializeField] private float[] YRotationArray;
    [SerializeField] private Barmetler.RoadSystem.NavigationLineUpdater[] NaviGationScriptArray;
    [SerializeField] private string[] DictKeys;
    [SerializeField] private bool[] DictValues;
    [SerializeField] private bool TestOnOtherRoads;
    [SerializeField] public const float AngaleMax= 450f;
    [SerializeField] public const float SpeedMax = 125f;
    public static float MaxSteeringAngale = 150f;
    public static float Speed = 125f / 2;
    private Dictionary<string, bool> DataTypesToSave = new Dictionary<string, bool>();


    public Slider SpeedSlider;
    public TextMeshProUGUI SpeedText;

    public void SpeedSliderFunc()
    {
        float SliderValue = SpeedSlider.value;
        if (SliderValue < 0.1f)
        {
            SliderValue = 0.1f;
        }
        SpeedText.text = "Speed = " + (SliderValue * SpeedMax).ToString();
        Speed = SliderValue * SpeedMax;
        MaxSteeringAngale = SliderValue * AngaleMax;
    }

    void Start()
    {
        SpeedSlider.value = Speed / SpeedMax;
        Debug.Log(SpeedSlider.value);
        for (int i = 0; i < DictKeys.Length; i++)
        {
            DataTypesToSave[DictKeys[i]] = DictValues[i];
        }

        string data_file_path = "/Users/Avshalom/github/autonm car/UnityData/" + System.Guid.NewGuid().ToString();
        RoadHandler simScript = null;
        for (int k = 0; k < TracksToInclude.Length; k++)
        {
            if(TracksToInclude[k])
            {

                GameObject roadSim = Instantiate(roadHandlerPrefab);
                simScript = roadSim.GetComponent<RoadHandler>();

                GraphDataSaver dataScript = roadSim.GetComponent<GraphDataSaver>();
                dataScript.filePath = data_file_path;
                dataScript.HomeTrack = k;
                dataScript.DataTypesToSave = DataTypesToSave;
                bool[] newArray;
                newArray = (bool[]) TracksToInclude.Clone();
                simScript.HomeTrack = k;
                simScript.AllTracks = newArray;
                simScript.YRotationArray = YRotationArray;
                simScript.startPositionArray = startPositionArray;
                simScript.NaviGationScriptArray = NaviGationScriptArray;
                simScript.TestOnOtherRoads = TestOnOtherRoads;
                Endings[k].DataSaver = dataScript;

            }
        }
        if(simScript != null)
        {
            simScript.camController = camController;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
