using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    public System.Action<float> OnPredict;

    public Button PredictButton;
    public Button ClearButton;
    public TMP_Text PredictionResult;
    public TMP_InputField InputField;


    // Start is called before the first frame update
    void Start()
    {
        PredictButton.onClick.AddListener(PredictHandler);
        ClearButton.onClick.AddListener(Clear);
        Clear();
    }

    public void AcceptPrediction(double prediction)
    {
        PredictionResult.text = prediction.ToString("F5");
    }

    public void Clear()
    {
        PredictionResult.text = string.Empty;
        InputField.text = string.Empty;
    }

    void PredictHandler()
    {
        var text = InputField.text;

        if (string.IsNullOrEmpty(text))
            return;

        if (float.TryParse(text, out var v))
        {
            OnPredict?.Invoke(v);
        }
    }
}
