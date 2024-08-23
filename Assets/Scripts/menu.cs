using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject menupanel;
    public GameObject settingpanel;
    public GameObject creditpanel;
    // Start is called before the first frame update
    void Start()
    {
        menupanel.SetActive(true);
        settingpanel.SetActive(false);
        creditpanel.SetActive(false);
        settingpanel.GetComponent<SettingAudio>().Start();
        AudioManager.instance.Play("Musik Main");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void SettingButton()
    {
        menupanel.SetActive(false);
        settingpanel.SetActive(true);
    }

    public void CreditButton()
    {
        menupanel.SetActive(false);
        creditpanel.SetActive(true);
    }

    public void BackButton()
    {
        menupanel.SetActive(true);
        settingpanel.SetActive(false);
        creditpanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("ANDA TELAH KELUAR...!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
