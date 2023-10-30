using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>
{
    private Dictionary<string, SoundData> _dictSounds = new Dictionary<string, SoundData>();
    private Dictionary<string, int> _dictSoundsCount = new Dictionary<string, int>();

    public void AddSound(SoundData data)
    {
        if (_dictSounds.ContainsKey(data.Audio.name) == false && _dictSoundsCount.ContainsKey(data.Audio.name) == false)
        {
            var soundObject = Instantiate(data.Audio.gameObject);
            soundObject.transform.SetParent(transform);
            soundObject.name = soundObject.name.Replace("(Clone)", "");
            data.Audio = soundObject.GetComponent<AudioSource>();

            _dictSounds.Add(data.Audio.name, data);
            _dictSoundsCount.Add(data.Audio.name, 0);
        }
    }
    public void PlayAudio(SoundData data, bool isForcePlay = false)
    {
        if(_dictSounds.ContainsKey(data.Audio.name) && _dictSoundsCount.ContainsKey(data.Audio.name))
        {
            if (_dictSoundsCount[data.Audio.name] < _dictSounds[data.Audio.name].MaxCount)
            {
                if (isForcePlay == false)
                {
                    if (!_dictSounds[data.Audio.name].Audio.isPlaying)
                    {
                        _dictSounds[data.Audio.name].Audio.Play();
                        AudioFinish(data.Audio.name, (int)(_dictSounds[data.Audio.name].Audio.clip.length * 1000)).Forget();
                        _dictSoundsCount[data.Audio.name]++;
                    }
                    else
                    {
                        AudioDelay(data).Forget();
                    }
                }
                else
                {
                    _dictSounds[data.Audio.name].Audio.Play();
                    AudioFinish(data.Audio.name, (int)(_dictSounds[data.Audio.name].Audio.clip.length * 1000)).Forget();
                    _dictSoundsCount[data.Audio.name]++;
                }
            }
        }
        else
        {
            AddSound(data);
        }
    }
    private async UniTaskVoid AudioFinish(string name, int timer)
    {
        await UniTask.Delay(timer);
        _dictSoundsCount[name]--;
    }
    private async UniTaskVoid AudioDelay(SoundData data)
    {
        await UniTask.Delay(200);
        _dictSounds[data.Audio.name].Audio.Play();
        AudioFinish(data.Audio.name, (int)(_dictSounds[data.Audio.name].Audio.clip.length * 1000)).Forget();
        _dictSoundsCount[data.Audio.name]++;
    }
}
