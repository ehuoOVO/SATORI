using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using Unity.VisualScripting;

public class SaveLoad : MonoBehaviour
{
    public GameObject SaveLoadPanel;
    public TextMeshProUGUI Title;
    public Button[] SaveLoadButton;
    public Button PrevButton;
    public Button NextButton;
    public Button BackButton;

    private bool isSaving;
    private int CurrentPage = Constants.Defalut_Start_Index;
    private readonly int SlotsPerPage = Constants.Slots_Per_Page;
    private readonly int TotalSlots = Constants.Total_Slots;
    private System.Action<int> CurrentAction;

    public static SaveLoad Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        PrevButton.onClick.AddListener(PrevPage);
        NextButton.onClick.AddListener(NextPage);
        BackButton.onClick.AddListener(GoBack);
        SaveLoadPanel.SetActive(false);
    }

    public void ShowSavePanel(System.Action<int> action)
    {
        isSaving = true;
        Title.text = "保存旅途节点";
        CurrentAction = action;
        UpdateUI();
        SaveLoadPanel.SetActive(true);
    }
    public void ShowLoadPanel(System.Action<int> action)
    {
        isSaving = false;
        Title.text = "载入旅途节点";
        CurrentAction = action;
        UpdateUI();
        SaveLoadPanel.SetActive(true);
    }

    private void UpdateUI()
    {
        for(int i = 0; i < SlotsPerPage; i++)
        {
            int SlotsIndex = CurrentPage * SlotsPerPage + i;
            if (SlotsIndex < TotalSlots)
            {
                UpdateSaveLoad(SaveLoadButton[i], SlotsIndex); 
                LoadStoryLineAndScreenShots(SaveLoadButton[i], SlotsIndex);
            }
            else SaveLoadButton[i].gameObject.SetActive(false);
        }
    }

    private void UpdateSaveLoad(Button B,int index)
    {
        B.gameObject.SetActive(true);
        B.interactable = true;

        var savePath = GenerateDataPath(index);
        var fileExists = File.Exists(savePath);


        if(!isSaving && !fileExists)
        {
            B.interactable = false;
        }

        var textComponents = B.GetComponentsInChildren<TextMeshProUGUI>();
        textComponents[0].text = null;
        textComponents[1].text = (index + 1) + "：空的节点";
        B.GetComponentInChildren<RawImage>().texture = null;

        B.onClick.RemoveAllListeners();
        B.onClick.AddListener(() => OnButtonClick(B, index));
    }

    private void OnButtonClick(Button B,int index)
    {
        CurrentAction?.Invoke(index);
        if (isSaving)
        {
            LoadStoryLineAndScreenShots(B, index);
        }
        else
        {

        }
    }

    private string GenerateDataPath(int index)
    {
        return Path.Combine(Application.persistentDataPath, Constants.Save_File_Path, index + Constants.Save_File_Extension);
    }

    private void PrevPage()
    {
        if(CurrentPage > 0)
        {
            CurrentPage--;
            UpdateUI();
        }
    }
    private void NextPage()
    {
        if ((CurrentPage + 1) * SlotsPerPage < TotalSlots) 
        {
            CurrentPage++;
            UpdateUI();
        }
    }

    public void GoBack()
    {
        SaveLoadPanel.SetActive(false);
    }
    

    private void LoadStoryLineAndScreenShots(Button B,int index)
    {
        var savePath = GenerateDataPath(index);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<VNmanager.SaveData>(json);
            if (saveData.ScreenshotData != null)
            {
                Texture2D screenShot = new Texture2D(2, 2);
                screenShot.LoadImage(saveData.ScreenshotData);
                B.GetComponentInChildren<RawImage>().texture = screenShot;
            }
            if(saveData.currentSpeakingContent != null)
            {
                var textComponents = B.GetComponentsInChildren<TextMeshProUGUI>();
                textComponents[0].text = saveData.currentSpeakingContent;
                textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G");
            }
        }
    }
}
