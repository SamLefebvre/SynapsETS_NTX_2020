using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class GameUDPManager : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    private int port;
    private Slider m_Slider;
    private UDPSocket socket;
    private Toggle toggle;
    private Slider sliderOverride;
    public Transform platform;

    public RotatorX[] rotator;

    private float myoValue;

    private void Awake()
    {
        var culture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 200;
        m_Text = GameObject.Find("udp_values").GetComponent<TextMeshProUGUI>();
        m_Slider = GameObject.Find("Slider").GetComponent<Slider>();
        sliderOverride = GameObject.Find("SliderOverride").GetComponent<Slider>();
        toggle = GameObject.Find("Toggle").GetComponent<Toggle>();

        myoValue = 0;

        ChangePort();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (toggle.isOn)
        {
            myoValue = sliderOverride.value;
            m_Text.text = string.Format("Override value: {0}", sliderOverride.value.ToString("0.00"));

            MovePlatform();
            return; // exit update()
        }

        if (socket != null)
        {
            string data = socket.readData;

            if (data == string.Empty) return;

            m_Text.text = string.Format("UDP Port 5020: {0}", data);

            //data = data.Replace(".", ",");
            float.TryParse(data, out float result);
            myoValue = result;
            m_Slider.value = result;
        }

       
        MovePlatform();
    }

    private void MovePlatform()
    {
        //float newYPosition = Mathf.Abs( myoValue - 5);
        //platform.position = new Vector3(platform.position.x, newYPosition, platform.position.z);

        // Map Range
        float speedRotator = Mathf.Lerp(1, 0, Mathf.InverseLerp(0, 5, myoValue));

        foreach (var item in rotator)
        {
            item.percentSpeed = speedRotator;
        }
    }

    public void ChangePort()
    {
        int.TryParse(GameObject.Find("InputField_Port").GetComponent<TMP_InputField>().text, out port);

        if (socket != null)
        {
            socket.AbortConnection();
        }

        try
        {
            socket = new UDPSocket(port);
        }
        catch (System.Exception e)
        {
            m_Text.text = string.Format("Error: {0}", e.Message);
            socket = null;
        }
    }
}
