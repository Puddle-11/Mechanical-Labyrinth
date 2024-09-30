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
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            AddListener(volumeSliders[i], i);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            ClearListeners(volumeSliders[i]);
        }
    }
    private void InitializeSliders()
    {
        if (SettingsController.instance == null) return;
        for (int i = 0; i < volumeSliders.Length; i++)
        {
           // Debug.Log($"{i} fetched {SettingsController.instance.GetTypeVolume(i)}");

            volumeSliders[i].value = SettingsController.instance.GetTypeVolume(i);
        }
        sensitivitySlider.value = SettingsController.instance.GetSettings().S_cameraSensitivity;
    }
    public void AddListener(Slider _slider, int _index)
    {
        _slider.onValueChanged.AddListener(delegate { UpdateVolume(_index); });
    }
    public void ClearListeners(Slider _slider)
    {
        _slider.onValueChanged.RemoveAllListeners();
    }
    public void UpdateVolume(int index)
    {
        if (SettingsController.instance != null)
        {
            Debug.Log($"Changed Value: {index}: {volumeSliders[index].value}");
            SettingsController.instance.SetTypeVolume(index, volumeSliders[index].value);
        }
        else
        {
            Debug.LogWarning("Failed to update volume, no settings controller found");
        }
    }

}
