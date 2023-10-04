using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarControlls : MonoBehaviour
{
    // Start is called before the first frame update
    public int IndexReached = 0;
    public RoadHandler SimRef;
    [SerializeField] public int probingDistance;
    [SerializeField] private LayerMask raycastMask;
    public float MaxSteeringAngale;
    public float Speed;
    private bool PlayerHit = false;
    public int inputSize = 6;
    public int hiddenSize = 30;
    public int outputSize = 1;
    [HideInInspector] public bool DontChangeWeights = false;
    // Weights for the connections between input and hidden layer, and between hidden and output layer
    public float[,] inputWeights;
    public float[,] hiddenWeights;
    
    void OnEnable()
    {
        PlayerHit = false;
        if(SimRef != null)
        {
            transform.position = SimRef.StartPosition;
            transform.rotation = SimRef.StartRotation;
        }
    }

    public void RandomWeights()
    {
        inputWeights = new float[inputSize, hiddenSize];
        hiddenWeights = new float[hiddenSize, outputSize];
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                inputWeights[i, j] = Random.Range(-1f, 1f);
            }
        }
        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                hiddenWeights[i, j] = Random.Range(-1f, 1f);
            }
        }
    }
    public void MutateWeights(float[,] HiddenWeights, float[,] InputWeights, float mutationRate)
    {
        if (!DontChangeWeights)
        {
            inputWeights = new float[inputSize, hiddenSize];
            hiddenWeights = new float[hiddenSize, outputSize];
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    inputWeights[i, j] = InputWeights[i, j];
                    if (Random.value < mutationRate)
                    {
                        inputWeights[i, j] = Random.Range(-1f, 1f);
                    }
                }
            }
            for (int i = 0; i < hiddenSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    hiddenWeights[i, j] = HiddenWeights[i, j];
                    if (Random.value < mutationRate)
                    {
                        hiddenWeights[i, j] = Random.Range(-1f, 1f);
                    }
                }
            }
        }
        else
        {
            DontChangeWeights = false;
        }
    }

    public void CrossOver(CarControlls Script1, CarControlls Script2)
    {
        if (!DontChangeWeights)
        {
            int Middle = inputSize * hiddenSize;
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    Middle--;
                    if (hiddenSize * inputSize / 2 < Middle)
                    {
                        inputWeights[i, j] = Script1.inputWeights[i, j];
                    }
                    else
                    {
                        inputWeights[i, j] = Script2.inputWeights[i, j];
                    }
                }
            }

            // Perform crossover for hidden to output weights
            Middle = outputSize * hiddenSize;
            for (int i = 0; i < hiddenSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    Middle--;
                    if (hiddenSize * outputSize / 2 > Middle)
                    {
                        hiddenWeights[i, j] = Script1.hiddenWeights[i, j];
                    }
                    else
                    {
                        hiddenWeights[i, j] = Script2.hiddenWeights[i, j];
                    }
                }
            }
        }
        else
        {
            DontChangeWeights = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!PlayerHit)
        {
            PlayerHit = true;
            GetComponent<CarControlls>().enabled = false;
            SimRef.AliveCars -= 1;
        }
    }
    void FixedUpdate()
    {
        float[] input = CastRaycasts();
        double output = CalculateOutput(input);
        //Debug.Log(output);
        transform.rotation = Quaternion.Euler(0,(float) (transform.rotation.eulerAngles.y + output * Time.deltaTime * MaxSteeringAngale), 0);
        transform.position += transform.forward * Time.deltaTime * Speed;
        
    }



    double CalculateOutput(float[] input)
    {
        // Calculate the values of the hidden layer

        double[] hiddenLayer = new double[hiddenSize];
        for (int i = 0; i < hiddenSize; i++)
        {
            float sum = 0f;
            for (int j = 0; j < inputSize; j++)
            {
                sum += input[j] * inputWeights[j, i];
            }
            //hiddenLayer[i] = System.Math.Max(0f, sum);
            hiddenLayer[i] = System.Math.Tanh(sum);
        }

        // Calculate the output of the neural network
        double output = 0f;
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0f;
            for (int j = 0; j < hiddenSize; j++)
            {
                sum += hiddenLayer[j] * hiddenWeights[j, i];
            }
            output = System.Math.Tanh(sum);
        }

        return output;
    }
        float[] CastRaycasts()
        {
        int numRays = inputSize-1;
        float angleStep = 45f;
        float[] input = new float[inputSize];
        for (int i = 0; i < numRays; i++)
        {
            Vector3 offset = new Vector3(0, 1, 0);
            float angle = i * angleStep;
            float distance;
            Vector3 direction = Quaternion.Euler(0f, angle-90, 0f) * transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, direction, out hit, probingDistance, raycastMask))
            {
                distance = hit.distance;
            }
            else
            {
                distance = probingDistance;
            }

            input[i] = distance / probingDistance;
        }
        input[inputSize-1] = 1;// for the bais
        return input;
    }
}