using Unity.Sentis;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class MNISTRunner : MonoBehaviour
{
    [SerializeField] private Texture2D[] m_inputTexture;
    [SerializeField] private ModelAsset m_modelAsset;

    private Model m_runtimeModel;
    private IWorker m_worker;

    public Texture2D[] Textures => m_inputTexture;

    [ContextMenu("Inspect")]
    public void InspectModel() => Mediator.InspectModel(m_modelAsset);

    public async Task<(int index, float[] probabilityDistribution)> Run(Texture2D textue, bool isServerRequest, ServerRequester serverRequester)
    {
        if (isServerRequest)
            return await serverRequester.SendTextureRequest(textue);
        else
            return LocalPrediction(textue);       
    }

    public (int index, float[] probabilityDistribution) RunLocal(Texture2D textue)
    {
        return LocalPrediction(textue);
    }

    private (int, float[]) LocalPrediction(Texture2D textue)
    {
        m_runtimeModel = ModelLoader.Load(m_modelAsset);

        TextureTransform settings = new TextureTransform().SetTensorLayout(TensorLayout.NHWC).SetDimensions(width: 28, height: 28, channels: 1);
        Tensor inputTensor = TextureConverter.ToTensor(textue, settings);

        m_worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, m_runtimeModel);
        m_worker.Execute(inputTensor);

        TensorFloat outputTensor = m_worker.PeekOutput() as TensorFloat;
        outputTensor.MakeReadable();

        var predictedProbability = outputTensor.ToReadOnlyArray();
        var index = predictedProbability.ToList().IndexOf(predictedProbability.Max());

        //Prediction: 0. Probability: 12,41915, -8,864447, 2,103586, 0,1844991, -7,704805, 1,19522, 0,04424899, -2,034581, -1,937755, 0,6853594
        //Debug.Log($"Prediction: {index}. Probability: {string.Join(", ", predictedProbability)}");

        // Tell the GPU we're finished with the memory the engine used
        m_worker.Dispose();
        inputTensor?.Dispose();

        return (index, predictedProbability);
    }
}
