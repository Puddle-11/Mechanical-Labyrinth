using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIInterface : MonoBehaviour
{
    public Slider[] volumeSliders;
    public Slider sensitivitySlider;
    public TMP_Text senseDisplay;
    private void Awake()
    {
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            AddListener(volumeSliders[i], i);
        }
        sensitivitySlider.onValueChanged.AddListener(UpdateSense);
    }
    public void UpdateSense(float _val)
    {
        senseDisplay.text = (Mathf.Round(_val * 100)/100).ToString();
        SettingsController.instance.SetSensitivity(_val);
    }
    private void OnEnable()
    {
        InitializeSliders();
    }
    private void InitializeSliders()
    {
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            volumeSliders[i].value = SettingsController.instance.GetTypeVolume(i);

        }
        sensitivitySlider.value = SettingsController.instance.GetSettings().S_cameraSensitivity;
    }
    public void AddListener(Slider _slider, int _index)
    {
        _slider.onValueChanged.AddListener(delegate { UpdateVolume(_index); });
    }
    public void UpdateVolume(int index)
    {
        if (SettingsController.instance != null)
        {
            SettingsController.instance.SetTypeVolume(index, volumeSliders[index].value);
        }
        else
        {
            Debug.LogWarning("Failed to update volume, no settings controller found");
        }
    }

}
