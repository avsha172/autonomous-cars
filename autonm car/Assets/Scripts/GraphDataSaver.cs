using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GraphDataSaver : MonoBehaviour
{
    public string filePath = "/Users/Avshalom/github/autonm car/UnityData";
    public Dictionary<string, bool> DataTypesToSave = new Dictionary<string, bool>();
    public int HomeTrack;
    void Start()
    {
        string fileName = "/home_track" + HomeTrack.ToString();
        filePath = filePath + fileName;
    }

    public void SaveGraphData(string GraphData, string DataType, int TrackNum = -1)
    {
        if (DataTypesToSave[DataType])
        {
            string fileName = DataType + ".txt";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if(TrackNum != -1)
            {
                GraphData = TrackNum.ToString() + ":" + GraphData;
            }
            string fullFilePath = Path.Combine(filePath, fileName);
            using (StreamWriter writer = File.AppendText(fullFilePath))
            {
                writer.WriteLine(GraphData);
            }
        }
    }

    public void SaveWeights(float[,] Weights)
    {
        if (DataTypesToSave["Weights"])
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fullFilePath = Path.Combine(filePath, "Weights.txt");
            int numRows = Weights.GetLength(0);
            int numCols = Weights.GetLength(1);
            using (StreamWriter writer = File.AppendText(fullFilePath))
            {
                writer.WriteLine(numRows);
                writer.WriteLine(numCols);
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        writer.Write(Weights[i, j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
