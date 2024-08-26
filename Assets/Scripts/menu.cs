using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject menupanel;
    public GameObject controlpanel;
    public GameObject settingpanel;
    public GameObject creditpanel;

    // Start is called before the first frame update
    void Start()
    {
        HideAllPanels();
        menupanel.SetActive(true);  // Show the menu panel by default when the scene starts
        settingpanel.GetComponent<SettingAudio>().Start();
        AudioManager.instance.Play("Musik Main");
    }

    // Hide all panels to prevent them from overlapping
    void HideAllPanels()
    {
        menupanel.SetActive(false);
        controlpanel.SetActive(false);
        settingpanel.SetActive(false);
        creditpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void ControlButton()
    {
        HideAllPanels();
        controlpanel.SetActive(true);
    }

    public void SettingButton()
    {
        HideAllPanels();
        settingpanel.SetActive(true);
    }

    public void CreditButton()
    {
        HideAllPanels();
        creditpanel.SetActive(true);
    }

    public void BackButton()
    {
        HideAllPanels();
        menupanel.SetActive(true);
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
