using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] AdMob_Manager admob_manager;

    public static bool is_touched = false;
    //ドラムアクション時の左右ボタン
    public enum Button_Location {Null,Right,Left};
    public static Button_Location button_location = Button_Location.Null;

    //設定用
    [SerializeField] GameObject Setting_Menu;
    //Timing_Lagを調整するポインタ
    [SerializeField] Transform Button_Detection_Pointer_Trans;
    public static double detection_lag;

    //ランキング用
    [SerializeField] GameObject Ranking_Menu;
    [SerializeField] Ranking_Manager ranking_manager;
    [SerializeField] GameObject Register_Profile;
    [SerializeField] Text your_name;

    //コイン用
    [SerializeField] Home_Manager home_manager;
    [SerializeField] GameObject Coin_Menu;
    [SerializeField] GameObject Remix_Release_Button;
    [SerializeField] Text coin_num;
    [SerializeField] GameObject Confirm_Menu;
    [SerializeField] Text confirm_text;
    int need_coin = 0;
    string release_stage_name = "";

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip coin_sound;

    //リリース後に上から画像をかぶせてボタンを消す
    [SerializeField] GameObject Eat_Button_Released;
    [SerializeField] GameObject Grab_Button_Released;
    [SerializeField] GameObject Clap_Button_Released;
    [SerializeField] GameObject Dondon_Button_Released;
    [SerializeField] GameObject Remix_Button_Released;


    void Start()
    {
        //登録時に感知度ポインタを0にした
        if (Button_Detection_Pointer_Trans != null && PlayerPrefs.GetInt("FINISH_REGISTER", 0) == 1)
        {
            Button_Detection_Pointer_Trans.localPosition = new Vector2(-210 + 100 * PlayerPrefs.GetInt("DETECTION", 0),0);
            detection_lag = 0.1 * PlayerPrefs.GetInt("DETECTION", 0);
        }

        //PlayerPrefs.SetInt("COIN_NUM", 2000);
        //PlayerPrefs.SetInt("RELEASE_Eat", 0);
    }

    //ボタンを押した瞬間
    public void EnterButton()
    {
        is_touched = true;
    }

    //ボタンを離した瞬間
    public void ExitButton()
    {
        is_touched = false;
    }

    //右ボタンと左ボタンは上の1つ用のボタンと別にする
    public void RightButton()
    {
        if (is_touched) return;
        button_location = Button_Location.Right;
        is_touched = true;
    }

    public void LeftButton()
    {
        if (is_touched) return;
        button_location = Button_Location.Left;
        is_touched = true;
    }
    
    //ホームに遷移
    public void HomeButton()
    {
        SceneManager.LoadScene("Home");
    }
    //アクションに遷移
    public void ActionButton(string stage_name)
    {
        admob_manager.Hide_Banner();

        PlayerPrefs.SetString("STAGE_NAME", stage_name);
        Debug.Log(PlayerPrefs.GetInt("STAGE_NAME", 0));

        //引数渡しのが良いかも
        SceneManager.LoadScene("Action");
    }

    //アクションに遷移
    public void RetryButton()
    {
        SceneManager.LoadScene("Action");
    }


    public void Reset__Button()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Home");
    }

    //設定画面
    public void Setting_Button(bool active)
    {
        Setting_Menu.SetActive(active);
    }

    //ランキング画面
    public void Ranking_Button(bool active)
    {
        Ranking_Menu.SetActive(active);
        //デフォルトではDramステージのランキングを開く
        //これによりアクションステージで選んだstage_nameに左右されることなく、毎回ランキングを開ける
        if (active) ranking_manager.Register_Data("Dram");
    }

    //コイン画面
    public void Coin_Button(bool active)
    {
        coin_num.text = PlayerPrefs.GetInt("COIN_NUM",0).ToString();
        Coin_Menu.SetActive(active);
        Check_Released();
    }

    //ボタンの感度を調整する
    public void Modify_ButtonDetection_Button(int detection)
    {
        PlayerPrefs.SetInt("DETECTION", detection);
        Button_Detection_Pointer_Trans.localPosition = new Vector2(-210 + 100 * PlayerPrefs.GetInt("DETECTION", detection), 0);
        detection_lag = 0.1 * PlayerPrefs.GetInt("DETECTION", detection);
    }

    //一番最初にユーザ名を登録するボタン
    public void OK_Name_Button()
    {
        //最初に入力した名前と3桁の数値をアカウント名とする
        string account = your_name.text + "@" + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString();
        PlayerPrefs.SetString("ACCOUNT", account);
        Register_Profile.SetActive(false);
        PlayerPrefs.SetInt("FINISH_REGISTER", 1);
        ranking_manager.Register_Data_First("Dram");
    }

    public void Get_Coin_After_Ad()
    {
        PlayerPrefs.SetInt("COIN_NUM", PlayerPrefs.GetInt("COIN_NUM", 0) + 1000);
        coin_num.text = PlayerPrefs.GetInt("COIN_NUM", 0).ToString();
        sound_source.PlayOneShot(coin_sound);
    }

    //各ステージのボタンを押してステージを解放する
    public void Release_Action_Button(string stage_name)
    {
        Confirm_Menu.SetActive(true);
        release_stage_name = stage_name;
        //必要なコインを設定
        if (stage_name == "Eat") need_coin = 1000;
        else if (stage_name == "Grab") need_coin = 1000;
        else if (stage_name == "Clap") need_coin = 1000;
        else if (stage_name == "Dondon") need_coin = 1000;
        else if (stage_name == "Remix") need_coin = 2000;

        confirm_text.text = "Do you get New Stage\n"
                          + "for <color=#ff0000>" + need_coin + "</color> coins ?";       
    }

    public void Release_Yes_Button()
    {
        if (PlayerPrefs.GetInt("COIN_NUM", 0) < need_coin) return;
        
        int after_coin = PlayerPrefs.GetInt("COIN_NUM", 0) - need_coin;
        PlayerPrefs.SetInt("COIN_NUM", after_coin);
        coin_num.text = after_coin.ToString();
        PlayerPrefs.SetInt("RELEASE_" +  release_stage_name, 1);
        ranking_manager.Register_Data_First(release_stage_name);
        Check_Released();
        home_manager.Display_Stage_Button();

        Confirm_Menu.SetActive(false);
    }

    public void Release_No_Button()
    {
        Confirm_Menu.SetActive(false);
        need_coin = 0;
        release_stage_name = "";
    }

    void Check_Released()
    {
        if (PlayerPrefs.GetInt("RELEASE_Eat", 0) == 1 &&
            PlayerPrefs.GetInt("RELEASE_Grab", 0) == 1 &&
            PlayerPrefs.GetInt("RELEASE_Clap", 0) == 1 &&
            PlayerPrefs.GetInt("RELEASE_Dondon", 0) == 1)
            Remix_Release_Button.SetActive(true);

        if (PlayerPrefs.GetInt("RELEASE_Eat", 0) == 1) Eat_Button_Released.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Grab", 0) == 1) Grab_Button_Released.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Clap", 0) == 1) Clap_Button_Released.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Dondon", 0) == 1) Dondon_Button_Released.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Remix", 0) == 1) Remix_Button_Released.SetActive(true);
    }
}
