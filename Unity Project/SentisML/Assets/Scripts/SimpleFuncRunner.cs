using System.Threading.Tasks;
using Unity.Sentis;
using UnityEngine;

public class SimpleFuncRunner : MonoBehaviour
{
    [SerializeField] ModelAsset m_modelAsset;

    private IWorker m_worker;
    private Model m_runtimeModel;

    [ContextMenu("Inspect")]
    public void InspectModel() => Mediator.InspectModel(m_modelAsset);

    void Start()
    {
        m_runtimeModel = ModelLoader.Load(m_modelAsset);
    }

    public async Task<double> Run(float inputValue, bool isServerRequest, ServerRequester serverRequester)
    {
        if (isServerRequest)
            return await serverRequester.SendFloatRequest(inputValue);
        else
            return LocalPrediction(inputValue);
    }

    private double LocalPrediction(float  inputValue)
    {
        float[] data = new float[] { inputValue };
        TensorShape shape = new TensorShape(1, 1);
        TensorFloat tensor = new TensorFloat(shape, data);

        m_worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, m_runtimeModel, verbose: true);
        m_worker.Execute(tensor);

        TensorFloat outputTensor = m_worker.PeekOutput() as TensorFloat;
        outputTensor.MakeReadable();

        Debug.Log($"outputTensor: {outputTensor}");

        var results = outputTensor.ToReadOnlyArray();

        foreach (var floatRes in results)
            Debug.Log($"Results: {floatRes}");

        // Tell the GPU we're finished with the memory the engine used
        m_worker.Dispose();
        tensor?.Dispose();

        return results[0];
    }
}
