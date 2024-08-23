using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingAudio : MonoBehaviour
{
    public Slider volumeMusik;
    public AudioMixer audioMixer;
    //private float valueVol;
    // Start is called before the first frame update
    public void Start()
    {
        audioMixer.SetFloat("volumemusic", PlayerPrefs.GetFloat("Volume Music"));
        volumeMusik.value = PlayerPrefs.GetFloat("Volume Music");
        //audioMixer.GetFloat("volumemusic", out valueVol);
        //volumeMusik.value = valueVol;
    }
    public void setVolume()
    {
        audioMixer.SetFloat("volumemusic", volumeMusik.value);
        PlayerPrefs.SetFloat("Volume Music", volumeMusik.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
