using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageUI : MonoBehaviour
{
    public Action<Texture> OnPredict;
    public Action OnCustom;

    public Color MaxColor;
    public TMP_Text PredictionResult;
    public Image EntryPrefab;
    public RectTransform EntryParent;
    public Button NextButton;
    public Button PreviousButton;
    public Button CustomButton;
    public Button PredictButton;
    public RawImage Img;
    public TMP_Text[] DistributionLabels;

    private int m_iterator = 0;
    private Texture2D[] m_textures;
    private List<GameObject> m_entries;


    void Start()
    {
        NextButton.onClick.AddListener(NextButtonHandler);
        PreviousButton.onClick.AddListener(PrevButtonHandler);
        CustomButton.onClick.AddListener(CustomButtonHandler);
        PredictButton.onClick.AddListener(PredictHandler);

        ClearDistribution();
        ClearPrediction();
    }

    public void Initialize(Texture2D[] textures)
    {
        m_textures = textures;
        SetImg(0);
    }

    public void DisplayPrediction(int prediction, float[] distribution)
    {
        PredictionResult.text = prediction.ToString();

        PopulateEntries(prediction);
        PopulateDistribution(distribution, prediction);
    }

    public void AcceptCustom(Texture2D texture)
    {
        ClearEntries();
        ClearDistribution();
        ClearPrediction();

        Img.texture = texture;
    }

    void SetImg(int index) => Img.texture = m_textures[index];


    void NextButtonHandler()
    {
        m_iterator = Math.Clamp(m_iterator + 1, 0, m_textures.Length - 1);
        SetImg(m_iterator);
    }

    void PrevButtonHandler()
    {
        m_iterator = Math.Clamp(m_iterator - 1, 0, m_textures.Length - 1);
        SetImg(m_iterator);
    }

    void CustomButtonHandler() => OnCustom?.Invoke();

    void PredictHandler() => OnPredict?.Invoke(Img.texture);


    void PopulateEntries(int n)
    {
        ClearEntries();

        for (int i = 0; i < n; i++) 
        {
            var entry = Instantiate(EntryPrefab);
            entry.rectTransform.SetParent(EntryParent);

            m_entries.Add(entry.gameObject);
        }
    }

    void PopulateDistribution(float[] distribution, int maxIndex)
    {
        ClearDistribution();

        for (int i = 0; i < distribution.Length;i++)
            DistributionLabels[i].text = $"{i}: {distribution[i].ToString("F2")}";

        DistributionLabels[maxIndex].color = MaxColor;
    }


    void ClearDistribution()
    {
        for (int i = 0; i < DistributionLabels.Length; i++)
        {
            DistributionLabels[i].text = string.Empty;
            DistributionLabels[i].color = Color.white;
        }
    }

    void ClearPrediction() => PredictionResult.text = string.Empty;

    private void ClearEntries()
    {
        m_entries ??= new();

        foreach (var entry in m_entries)
            Destroy(entry);

        m_entries.Clear();
    }
}
