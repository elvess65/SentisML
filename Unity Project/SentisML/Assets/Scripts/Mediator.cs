using System;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.UI;

public class Mediator : MonoBehaviour
{
    public ImageUI UI;
    public SimpleUI SimpleUI;
    public SimpleFuncRunner SimpleFuncRunner;
    public MNISTRunner MNISTRunner;
    public LineGenerator lineGenerator;
    public ServerRequester ServerRequester;
    public Toggle ServerToggle;
    public Toggle RealtimeToggle;
    public bool DefaultServerValue = true;
    public GameObject[] RealtimeObjs;

    private bool m_isRealTime = false;

    void Start()
    {
        UI.Initialize(MNISTRunner.Textures);
        UI.OnPredict += MnistPredictHandler;
        UI.OnCustom += CustomHandler;

        lineGenerator.Initialize();
        lineGenerator.OnRealtimeSubmit += OnRealtimeSubmit;
        lineGenerator.OnSubmit += OnLineAccept;
        lineGenerator.OnCancel += OnLineCancel;

        if (SimpleUI != null)
            SimpleUI.OnPredict += SimplePredicthandler;

        RealtimeToggle.onValueChanged.AddListener(RealTimeToggleChanged);
        RealtimeToggle.isOn = false;

        ServerToggle.onValueChanged.AddListener(ServerToggleChanged);
        ServerToggle.isOn = !ServerToggle.isOn;
        ServerToggle.isOn = DefaultServerValue;
    }

    private async void SimplePredicthandler(float v)
    {
        var predicted = await SimpleFuncRunner.Run(v, ServerToggle.isOn, ServerRequester);
        SimpleUI.AcceptPrediction(predicted);
    }

    private async void MnistPredictHandler(Texture img)
    {
        var predicted = await MNISTRunner.Run(img as Texture2D, ServerToggle.isOn, ServerRequester);
        UI.DisplayPrediction(predicted.index, predicted.probabilityDistribution);
    }


    void RealTimeToggleChanged(bool isRealtime)
    {
        m_isRealTime = isRealtime;

        if (isRealtime && ServerToggle.isOn)
            ServerToggle.isOn = false;
    }

    void ServerToggleChanged(bool isServer)
    {
        if (isServer && RealtimeToggle.isOn)
            RealtimeToggle.isOn = false;
    }

    void CustomHandler()
    {
        if (!m_isRealTime)
            UI.gameObject.SetActive(false);

        lineGenerator.Activate(true);
        lineGenerator.ToggleRealtime(m_isRealTime);
        ToggleRealtimeObj(false);
    }

    void OnLineAccept(Texture texture)
    {
        if (SimpleUI != null)
            SimpleUI.Clear();

        OnLineCancel();
        UI.AcceptCustom(texture as Texture2D);
    }

    void OnRealtimeSubmit(Texture texture)
    {
        UI.AcceptCustom(texture as Texture2D);
        var predicted = MNISTRunner.RunLocal(texture as Texture2D);
        UI.DisplayPrediction(predicted.index, predicted.probabilityDistribution);
    }

    void OnLineCancel()
    {
        if (!m_isRealTime)
            UI.gameObject.SetActive(true);
        
        lineGenerator.Activate(false);
        lineGenerator.ToggleRealtime(false);
        ToggleRealtimeObj(true);
    }

    void ToggleRealtimeObj(bool isActive)
    {
        foreach (var obj in RealtimeObjs)
        {
            if (obj == null)
                continue;

            obj.SetActive(isActive);
        }
    }

    public static void InspectModel(ModelAsset asset)
    {
        var runtimeModel = ModelLoader.Load(asset);

        List<Model.Input> inputs = runtimeModel.inputs;

        Debug.Log($"***INPUTS*** {inputs.Count}");

        // Loop through each input
        foreach (var input in inputs)
        {
            // Log the name of the input, for example Input3
            Debug.Log($"Name: {input.name}");

            // Log the tensor shape of the input, for example (1, 1, 28, 28)
            Debug.Log($"Shape: {input.shape}");
        }

        List<string> outputs = runtimeModel.outputs;
        Debug.Log($"***OUTPUTS*** {outputs.Count}");

        // Loop through each output
        foreach (var output in outputs)
        {
            // Log the name of the output
            Debug.Log($"Output: {output}");
        }

        Debug.Log($"***LAYERS*** {runtimeModel.layers.Count}");
        foreach (var layer in runtimeModel.layers)
            Debug.Log(layer);
    }
}
