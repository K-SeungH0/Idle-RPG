using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundData : MonoBehaviour
{
    public AudioSource Audio;
    public int MaxCount = 1;

    public bool IsPlayToEnable;
    Button Btn;
    Toggle toggle;
    private void Start()
    {
        TryGetComponent(out Btn);
        TryGetComponent(out toggle);

        if (Btn != null)
            Btn.onClick.AddListener(ButtonClick);

        if (toggle != null)
            toggle.onValueChanged.AddListener(ToggleClick);

        SoundManager.Instance.AddSound(this);
    }

    private void OnEnable()
    {
        if(IsPlayToEnable && SoundManager.Instance) 
        {
            SoundManager.Instance.PlayAudio(this);
        }
    }

    private void ButtonClick()
    {
        SoundManager.Instance.PlayAudio(this, true);
    }
    private void ToggleClick(bool isOn)
    {
        if (isOn)
            SoundManager.Instance.PlayAudio(this, true);
    }
}
