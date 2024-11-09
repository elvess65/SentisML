using UnityEngine;
using System.Collections.Generic;

public class LineGenerator : MonoBehaviour
{
    public System.Action<Texture> OnSubmit;
    public System.Action<Texture> OnRealtimeSubmit;
    public System.Action OnCancel;

    public int Width = 28;
    public int Height = 28;
    public Line LinePrefab;
    public Camera cam;
    public GameObject isRealtimeNotification;

    private Line line;
    private List<Line> lines = new List<Line>();

    private bool m_isRealtime = false;

    public void Initialize()
    {
        isRealtimeNotification.SetActive(false);
    }

    public void ToggleRealtime(bool isRealtime)
    {
        m_isRealtime = isRealtime;
        isRealtimeNotification.gameObject.SetActive(isRealtime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            line = Instantiate(LinePrefab);
            lines.Add(line);
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            line = null;
        }

        if (line != null) 
        { 
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            line.UpdateLine(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Clear();
            OnCancel?.Invoke();
        }

        if (!m_isRealtime && Input.GetKeyDown(KeyCode.Return))
        {
            var t = RTImage();
            Clear();

            Debug.Log(t == null);
            OnSubmit?.Invoke(t);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Clear();
        }
    }

    private void FixedUpdate()
    {
        if (m_isRealtime)
        {
            var t = RTImage();
            OnRealtimeSubmit?.Invoke(t);
            cam.gameObject.SetActive(true);
        }
    }

    public void Activate(bool isActive)
    {
        cam.gameObject.SetActive(isActive);
        gameObject.SetActive(isActive);
    }

    private void Clear()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();
    }

    private Texture2D RTImage()
    {
        cam.gameObject.SetActive(false);
        Rect rect = new Rect(0, 0, 28, 28);

        RenderTexture renderTexture = new RenderTexture(28, 28, 24);

        Texture2D screenShot = new Texture2D(28, 28, TextureFormat.RGBA32, false);

        cam.targetTexture = renderTexture;
        
        RenderTexture.active = renderTexture;
        GL.Clear(false, true, Color.clear);
        cam.Render();

        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        Destroy(renderTexture);
        renderTexture = null;

        return screenShot;
    }
}
