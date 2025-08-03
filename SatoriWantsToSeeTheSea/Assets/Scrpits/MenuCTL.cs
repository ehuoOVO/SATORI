using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCTL : MonoBehaviour
{
    public GameObject MenuPanel;
    public Button StartButton;
    public Button SettingButton;
    public Button ContinueButton;
    public Button LoadButton;
    public Button QuitButton;

    public GameObject SettingPanel;

    private bool hasStart = false;
    private bool isSetting = false;

    public static MenuCTL Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        MenuButtonBind();
        SettingPanel.SetActive(false);
    }

    void MenuButtonBind()
    {
        StartButton.onClick.AddListener(StartGame);
        ContinueButton.onClick.AddListener(ContinueGame);
        SettingButton.onClick.AddListener(VNmanager.Instance.ClickSetting);
    }

    private void StartGame()
    {
        hasStart = true;
        VNmanager.Instance.StartGame();
        MenuPanel.SetActive(false);
        VNmanager.Instance.GamePanel.SetActive(true);
    }

    private void ContinueGame()
    {
        if (hasStart)
        {
            MenuPanel.SetActive(false);
            VNmanager.Instance.GamePanel.SetActive(true);
        }
    }

}
