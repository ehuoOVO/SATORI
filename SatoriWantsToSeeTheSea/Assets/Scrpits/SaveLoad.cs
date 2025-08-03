using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void ShowSaveLoad(bool save)
    {
        isSaving = save;
        Title.text = isSaving ? "保存旅途节点" : "载入旅途节点" ;
        UpdateSaveLoad();
        SaveLoadPanel.SetActive(true);
        LoadStoryLineAndScreenShots();
    }

    private void UpdateSaveLoad()
    {
        for(int i = 0;i < SlotsPerPage; i++)
        {
            int SlotsIndex = CurrentPage * SlotsPerPage + i;
            if(SlotsIndex < TotalSlots)
            {
                SaveLoadButton[i].gameObject.SetActive(true);
                SaveLoadButton[i].interactable = true;

                var SlotText = (SlotsIndex + 1) + "：空节点";
                var TextComponents = SaveLoadButton[i].GetComponentsInChildren<TextMeshProUGUI>();
                TextComponents[0].text = null;
                TextComponents[1].text = SlotText;
                SaveLoadButton[i].GetComponentInChildren<RawImage>().texture = null;

            }
            else SaveLoadButton[i].gameObject.SetActive(false);
        }
    }

    private void PrevPage()
    {
        if(CurrentPage > 0)
        {
            CurrentPage--;
            UpdateSaveLoad();
            LoadStoryLineAndScreenShots();
        }
    }
    private void NextPage()
    {
        if ((CurrentPage + 1) * SlotsPerPage < TotalSlots) 
        {
            CurrentPage++;
            UpdateSaveLoad();
            LoadStoryLineAndScreenShots();
        }
    }

    public void GoBack()
    {
        SaveLoadPanel.SetActive(false);
    }
    

    private void LoadStoryLineAndScreenShots()
    {

    }
}
