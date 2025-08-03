using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
//using UnityEngine.UIElements;

public class VNmanager : MonoBehaviour
{
    #region Variables
    public struct Trust
    {
        public int RIN;
        public int REK;
    }

    public Trust TrustData;

    public GameObject GamePanel;

    public TextMeshProUGUI Name;
    public GameObject DialogueBoard;

    public AudioSource BGM;
    public AudioSource AUD;
    public Image BGI;

    public TypewriterEffect Effect;

    public Image Chara_1;
    public Image Chara_2;
    public Image Chara_3;
    public Image Chara_4;
    public Image Chara_5;


    public GameObject ChoiceBoard;
    public float CountDownFullNum = Constants.Default_Count_Down_Full_Num;
    public float CountDownNum = Constants.Default_Count_Down_Num;
    public TextMeshProUGUI CountDown;
    public bool isCounting = false;
    public Slider ChoiceSlider;
    public Button Choice_1;
    public Button Choice_2;
    public Button Choice_3;
    public Button Choice_4;

    public GameObject ReadBoard;
    public Button ReadButton;
    public TextMeshProUGUI ReadWords;
    public TextMeshProUGUI MPText;
    public int MP = Constants.Default_MP;

    public GameObject SettingBoard;
    public Button AbleToSeeSwitch;
    public Button SettingButton;
    public Button SaveButton;
    public Button LoadButton;
    public Button HistoryButton;

    private bool AbleToSee = true;



    public GameObject SettingPanel;
    private bool isSetting = false;
    public Slider BGMSound;
    public Slider AUDSound;
    public Slider TypingSpeed;
    public Button HomeButton;
    public Button BackButton;

    private bool isSaveOrLoad = false;
    public Button SLBack;
    public ScreenShotter screenShotter;

    private string SaveFolderPath;
    private byte[] ScreenShotData;
    private string currentContent;


    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.Default_Story_File_Name;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.Default_Start_Line;
    private int CharaNum = Constants.Default_Chara_Num;
    #endregion
    #region Coroutines
    private IEnumerator Keyboardcoroutine()
    {
        if (Input.GetKeyDown(KeyCode.S)) ClickSetting();
        if (Input.GetKeyDown(KeyCode.A) && !isSetting) ClickATS();
        if (Input.GetKeyDown(KeyCode.K) && !isSetting) ClickSave();
        if (Input.GetKeyDown(KeyCode.L) && !isSetting) ClickLoad();

        //Debug.Log("ATSing!");
        yield return null;
    }

    private IEnumerator MousePositionCoroutine()
    {
        if (Input.mousePosition.x <= 210) SettingBoard.SetActive(true);
        else SettingBoard.SetActive(false);
        //Debug.Log("MPing!");
        yield return null;
    }

    private IEnumerator MouseClickCoroutine()
    {
        if (!MenuCTL.Instance.MenuPanel.activeSelf && 
           !SaveLoad.Instance.SaveLoadPanel.activeSelf &&
           !SettingPanel.activeSelf &&
           !SettingBoard.activeSelf &&
           GamePanel.activeSelf &&
           Input.GetMouseButtonDown(0))
        {
            if (!AbleToSee) ClickATS();
            else if (!isCounting) DisplayNextLine();
        }
        yield return null;
    }

    private IEnumerator CountingController()
    {
        if (isCounting)
        {
            if (CountDownNum <= 0)
            {
                InitializeAndLoad(storyData[currentLine].ChD.CT0);
            }
            else
            {
                CountDownNum -= Time.deltaTime;
                CountDown.text = CountDownNum.ToString("F1");
                float SliderValue = 1 - CountDownNum / CountDownFullNum;
                ChoiceSlider.value = SliderValue;
            }
        }
        yield return null;
    }

    private IEnumerator VolumnController()
    {
        if (isSetting) {
            Effect.UpdateSpeed(0.13f - TypingSpeed.value / 10);
            BGM.volume = BGMSound.value;
            AUD.volume = AUDSound.value;
        }
        yield return null; 
    }
    #endregion
    #region DEBUG_Variables
    public TextMeshProUGUI MousePosition;
    public TextMeshProUGUI ATS;
    public TextMeshProUGUI STS;
    public TextMeshProUGUI SLS;
    public TextMeshProUGUI TDRIN;
    public TextMeshProUGUI TDREK;
    #endregion
    #region Lifecycle
    public static VNmanager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        BindButton();
        GamePanel.SetActive(false);
        InitializeSaveFilePath();
    }

    void Update()
    {
        //-------调试行-------
        MousePosition.text = Input.mousePosition.x + " " + Input.mousePosition.y;
        ATS.text = AbleToSee.ToString();
        STS.text = Effect.waiting_seconds.ToString();
        SLS.text = isSaveOrLoad.ToString();
        TDRIN.text = TrustData.RIN.ToString();
        TDREK.text = TrustData.REK.ToString();
        //-------调试行-------

        StartAllCoroutine();
    }
    #endregion
    #region Initializations
    public void StartGame()
    {
        InitializeAndLoad(defaultStoryFileName);
    }

    public void InitializeSaveFilePath()
    {
        SaveFolderPath = Path.Combine(Application.persistentDataPath, Constants.Save_File_Path);
        if (!Directory.Exists(SaveFolderPath)) Directory.Exists(SaveFolderPath);
    }

    void InitializeAndLoad(string filename)
    {
        Initialize();
        LoadStoryFromFlie(filename);//加载
        DisplayNextLine();
    }

    void Initialize()
    {
        currentLine = Constants.Default_Start_Line;
        isCounting = false;
        CountDownNum = 0;
        CountDownFullNum = 0;
        AbleToSee = true;
        BGI.gameObject.SetActive(true);
        ChoiceBoard.SetActive(false);
        DialogueBoard.SetActive(true);
        SettingBoard.SetActive(true);
        Chara_1.gameObject.SetActive(false);
        Chara_2.gameObject.SetActive(false);
        Chara_3.gameObject.SetActive(false);
        Chara_4.gameObject.SetActive(false);
        Chara_5.gameObject.SetActive(false);
        ReadBoard.SetActive(false);
        SettingPanel.SetActive(false);
        BGM.volume = Constants.Default_BGM_Sound;
        AUD.volume = Constants.Default_AUD_Sound;
    }

    void BindButton()
    {
        AbleToSeeSwitch.onClick.AddListener(ClickATS);
        SettingButton.onClick.AddListener(ClickSetting);
        SaveButton.onClick.AddListener(ClickSave);
        LoadButton.onClick.AddListener(ClickLoad);
        HomeButton.onClick.AddListener(BackToHome);
        BackButton.onClick.AddListener(ClickSetting);

        SLBack.onClick.AddListener(SLBackSWT);
    }

    void StartAllCoroutine()
    {
        StartCoroutine(Keyboardcoroutine());
        StartCoroutine(MousePositionCoroutine());
        StartCoroutine(MouseClickCoroutine());
        StartCoroutine(VolumnController());
        StartCoroutine(CountingController());
    }

    void LoadStoryFromFlie(string filename)
    {
        var path = storyPath + filename + Constants.Excel_File_Extension;
        storyData = ExcelReader.ReadExcel(path);
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError("No data found in the file.");
        }
        return;
    }
    #endregion
    #region Display
    void DisplayNextLine()//下一行
    {
        if(currentLine > storyData.Count)
        {
            Debug.LogError("Breakthrough limit!!");
            return;
        }
        if (Effect.isTyping())
        {
            Effect.CompleteTyping();
        }
        else
        {
            DisplayThisLine();
        }
        return; 
    }

    void DisplayThisLine()
    {
        var data = storyData[currentLine];
        if(data.Name == "end")
        {
            Debug.Log(Constants.End_Of_Story);
            return;
        }
        if(data.Name == "choice")
        {
            ShowChoice(data.ChD);
            return;
        }
        else
        {
            ChoiceBoard.SetActive(false);
            DialogueBoard.SetActive(true);
            ReadBoard.SetActive(false);
            Name.text = data.Name;
            currentContent = data.Content;
            Effect.StartTyping(currentContent);

            if (NotNullNorEmpty(data.TD.REK)) TrustData.REK += int.Parse(data.TD.REK);
            if (NotNullNorEmpty(data.TD.RIN)) TrustData.RIN += int.Parse(data.TD.RIN);

            if (NotNullNorEmpty(data.BGI)) UpdateBGI(data.BGI);
            if (NotNullNorEmpty(data.BGM)) UpdateBGM(data.BGM);

            UpdateChara(data.CD);

            currentLine++;
        }
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    void UpdateChara(CharaData CD)
    {
        int num = int.Parse(CD.CharaNum);
        Chara_1.gameObject.SetActive(false);
        Chara_2.gameObject.SetActive(false);
        Chara_3.gameObject.SetActive(false);
        Chara_4.gameObject.SetActive(false);
        Chara_5.gameObject.SetActive(false);
        if(num == 1)
        {
            int c1site = int.Parse(CD.Chara1Site);
            string imagePath = Constants.Chara_Path + CD.Chara1;
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            if (sprite != null)
            {
                SiteGate(c1site).sprite = sprite;
                SiteGate(c1site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath);
            }
        }
        if (num == 2)
        {
            int c1site = int.Parse(CD.Chara1Site);
            int c2site = int.Parse(CD.Chara2Site);
            string imagePath1 = Constants.Chara_Path + CD.Chara1;
            string imagePath2 = Constants.Chara_Path + CD.Chara2;
            Sprite sprite1 = Resources.Load<Sprite>(imagePath1);
            if (sprite1 != null)
            {
                SiteGate(c1site).sprite = sprite1;
                SiteGate(c1site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath1);
            }

            Sprite sprite2 = Resources.Load<Sprite>(imagePath2);
            if (sprite2 != null)
            {
                SiteGate(c2site).sprite = sprite2;
                SiteGate(c2site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath2);
            }

        }

        if (num == 3)
        {
            int c1site = int.Parse(CD.Chara1Site);
            int c2site = int.Parse(CD.Chara2Site);
            int c3site = int.Parse(CD.Chara3Site);
            string imagePath1 = Constants.Chara_Path + CD.Chara1;
            string imagePath2 = Constants.Chara_Path + CD.Chara2;
            string imagePath3 = Constants.Chara_Path + CD.Chara3;

            Sprite sprite1 = Resources.Load<Sprite>(imagePath1);
            if (sprite1 != null)
            {
                SiteGate(c1site).sprite = sprite1;
                SiteGate(c1site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath1);
            }

            Sprite sprite2 = Resources.Load<Sprite>(imagePath2);
            if (sprite2 != null)
            {
                SiteGate(c2site).sprite = sprite2;
                SiteGate(c2site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath2);
            }

            Sprite sprite3 = Resources.Load<Sprite>(imagePath3);
            if (sprite3 != null)
            {
                SiteGate(c3site).sprite = sprite3;
                SiteGate(c3site).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError(Constants.Chara_Failed + imagePath3);
            }
        }

    }
    #endregion
    #region Choice
    void ShowChoice(ChoiceData ChD)
    {
        int num = int.Parse(ChD.ChoiceNum);
        float StartTime = float.Parse(ChD.ChioceTime);
        Choice_1.onClick.RemoveAllListeners();
        Choice_2.onClick.RemoveAllListeners();
        Choice_3.onClick.RemoveAllListeners();
        Choice_4.onClick.RemoveAllListeners();
        Choice_1.gameObject.SetActive(false);
        Choice_2.gameObject.SetActive(false);
        Choice_3.gameObject.SetActive(false);
        Choice_4.gameObject.SetActive(false);

        ChoiceBoard.SetActive(true);
        CountDown.gameObject.SetActive(true);
        DialogueBoard.SetActive(false);

        CountDownNum = StartTime;
        CountDownFullNum = StartTime;
        CountDown.text = CountDownNum.ToString("F1");
        isCounting = true;

        //CountDown.Start(StartTime);
        if(ChD.Read != null)
        {
            //ReadBoard.gameObject.
            ReadBoard.gameObject.SetActive(true);
            ReadOrNot(ChD.Read);
        }

        if (ChD.Choice1 != null)
        {
            Choice_1.GetComponentInChildren<TextMeshProUGUI>().text = ChD.Choice1;
            Choice_1.onClick.AddListener(() => InitializeAndLoad(ChD.CT1));
            Choice_1.gameObject.SetActive(true);
        }
        if (ChD.Choice2 != null)
        {
            Choice_2.GetComponentInChildren<TextMeshProUGUI>().text = ChD.Choice2;
            Choice_2.onClick.AddListener(() => InitializeAndLoad(ChD.CT2));
            Choice_2.gameObject.SetActive(true);
        }
        if (ChD.Choice3 != null)
        {
            Choice_3.GetComponentInChildren<TextMeshProUGUI>().text = ChD.Choice3;
            Choice_3.onClick.AddListener(() => InitializeAndLoad(ChD.CT3));
            Choice_3.gameObject.SetActive(true);
        }
        if (ChD.Choice4 != null)
        {
            Choice_4.GetComponentInChildren<TextMeshProUGUI>().text = ChD.Choice4;
            Choice_4.onClick.AddListener(() => InitializeAndLoad(ChD.CT4));
            Choice_4.gameObject.SetActive(true);
        }

        
    }

    private Image SiteGate(int site)
    {
        if (site == 1) return Chara_1;
        else if (site == 2) return Chara_2;
        else if (site == 3) return Chara_3;
        else if (site == 4) return Chara_4;
        else if (site == 5) return Chara_5;
        else
        {
            Debug.LogError("Site Not Found,default return site 1");
            return Chara_1;
        }
    }
    #endregion
    #region BGIandAudio
    void UpdateBGI(string name)
    {
        string BGIPath = Constants.BGI_Path + name;
        Sprite sprite = Resources.Load<Sprite>(BGIPath);
        if (sprite != null)
        {
            BGI.sprite = sprite;
        }
        else Debug.LogError(Constants.BGI_Failed + name);
    }

    void UpdateBGM(string name)
    {
        if(name == "stop")
        {
            BGM.Stop();
            return;
        }
        string BGMPath = Constants.BGM_Path + name;
        AudioClip audioclip = Resources.Load<AudioClip>(BGMPath);
        if (audioclip != null)
        {
            BGM.clip = audioclip;
            BGM.Play();
        }
        else Debug.LogError(Constants.BGM_Failed + name);
    }

    void PlayAudio(string name)
    {
        string audioPath = Constants.Audio_Path + name;
        AudioClip audioclip = Resources.Load<AudioClip>(audioPath);
        if (audioclip != null)
        {
            
        }
        else
        {
            Debug.LogError(Constants.Audio_Failed + audioPath);
        }
    }
    #endregion
    #region ReadHeart
    void ReadOrNot(string text)
    {
        MPText.text = "可使用次数：" + MP;
        ReadWords.text = text;
        ReadWords.gameObject.SetActive(false);
        ReadButton.onClick.AddListener(ShowReadWords);
    }

    void ShowReadWords()
    {
        if(MP >= 1)
        {
            MP--;
            ReadWords.gameObject.SetActive(true);
            MPText.text = "可使用次数：" + MP;
            ReadButton.onClick.RemoveAllListeners();
            return;
        }
    }
    #endregion
    #region Buttons
    #region GameButtons
    void ClickATS()
    {
        if (isCounting) return;
        AbleToSee = !AbleToSee;
        if (!AbleToSee)
        {
            DialogueBoard.SetActive(false);
            SettingBoard.SetActive(false);
            //ChoiceBoard.SetActive(false);
            //AbleToSee = false;
            return;
        }
        else
        {
            DialogueBoard.SetActive(true);
            SettingBoard.SetActive(true);
            //AbleToSee = true;
            return;
        }
    }

    public void ClickSetting()
    {
        if (isCounting) return;
        isSetting = !isSetting;
        if (!isSetting)
        {
            SettingPanel.SetActive(false);
            return;
        }
        else
        {
            SettingPanel.SetActive(true);
            return;
        }
    }
    #region Save
    public class SaveData
    {
        public string currentContent;
        public byte[] ScreenshotData;
        public int TrustRIN;
        public int TrustREK;
    }

    void ClickSave()
    {
        isSaveOrLoad = !isSaveOrLoad;
        if (isSaveOrLoad)
        {
            Texture2D ScreenShot = screenShotter.CaptureScreenShot();
            ScreenShotData = ScreenShot.EncodeToPNG();
            SaveLoad.Instance.ShowSaveLoad(true);
        }
        else
        {
            SaveLoad.Instance.GoBack();
        }
    }

    private void Save(int slotIndex)
    {
        
    }
    #endregion
    void ClickLoad()
    {
        isSaveOrLoad = !isSaveOrLoad;
        if (isSaveOrLoad)
        {
            SaveLoad.Instance.ShowSaveLoad(false);
        }
        else
        {
            SaveLoad.Instance.GoBack();
        }
    }
    #endregion
    #region SettingButtons
    public void BackToHome()
    {
        GamePanel.SetActive(false);
        MenuCTL.Instance.MenuPanel.SetActive(true);
        SettingPanel.SetActive(false);
        isSetting = !isSetting;
    }
    #endregion
    #region SaveLoadButtons
    void SLBackSWT() { isSaveOrLoad = !isSaveOrLoad; }
    #endregion
    #endregion
}


