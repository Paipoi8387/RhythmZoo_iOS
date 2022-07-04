using UnityEngine;
using UnityEngine.UI;

public class Home_Manager : MonoBehaviour
{
    [SerializeField] GameObject Register_Profile;

    [SerializeField] GameObject Eat_Stage_Button;
    [SerializeField] GameObject Grab_Stage_Button;
    [SerializeField] GameObject Clap_Stage_Button;
    [SerializeField] GameObject Dondon_Stage_Button;
    [SerializeField] GameObject Remix_Stage_Button;

    [SerializeField] Text dram_best_score;
    [SerializeField] Text eat_best_score;
    [SerializeField] Text grab_best_score;
    [SerializeField] Text clap_best_score;
    [SerializeField] Text dondon_best_score;
    [SerializeField] Text remix_best_score;

    //ホーム画面でリズムよくタイトルを拡大
    [SerializeField] AudioSource bgm_source;
    double diff;
    int beat_count;
    double bpm = 105.09;
    const double record_num_per_sec = 44100;
    [SerializeField] Transform Title_Trans;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.SetInt("FINISH_REGISTER", 0);
        if (PlayerPrefs.GetInt("FINISH_REGISTER", 0) == 0)
        {
            Register_Profile.SetActive(true);
        }
        Debug.Log(PlayerPrefs.GetString("ACCOUNT", ""));

        Display_Stage_Button();
        Display_BestScore();
    }

    private void Update()
    {
        diff = beat_count - (bgm_source.timeSamples / record_num_per_sec) * (bpm / 60);
        if(diff < 1)Title_Trans.localScale = new Vector2(1 + (float)(diff) / 16, 1 + (float)(diff) / 16);

        if (diff < 0)
        {
            beat_count++;
            diff = beat_count - (bgm_source.timeSamples / record_num_per_sec) * (bpm / 60);
        }
        else if(diff > 2)
        {
            beat_count = 0;
            diff = 0;
        }
    }

    //各ステージのボタンを表示させるかどうか
    public void Display_Stage_Button()
    {
        if (PlayerPrefs.GetInt("RELEASE_Eat", 0) == 1) Eat_Stage_Button.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Grab", 0) == 1) Grab_Stage_Button.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Clap", 0) == 1) Clap_Stage_Button.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Dondon", 0) == 1) Dondon_Stage_Button.SetActive(true);
        if (PlayerPrefs.GetInt("RELEASE_Remix", 0) == 1) Remix_Stage_Button.SetActive(true);
        //else if (PlayerPrefs.GetInt("RELEASE_Eat", 0) == 1) Eat_Stage_Button.SetActive(true);
    }

    //各ステージのボタンの下に表示させる記録
    void Display_BestScore()
    {
        dram_best_score.text = PlayerPrefs.GetInt("SCORE_Dram", 0).ToString();
        //第２ステージのやつ
        eat_best_score.text = PlayerPrefs.GetInt("SCORE_Eat", 0).ToString();
        //第３ステージのやつ
        grab_best_score.text = PlayerPrefs.GetInt("SCORE_Grab", 0).ToString();
        //第４ステージのやつ
        clap_best_score.text = PlayerPrefs.GetInt("SCORE_Clap", 0).ToString();
        //第５ステージのやつ
        dondon_best_score.text = PlayerPrefs.GetInt("SCORE_Dondon", 0).ToString();
        //第６ステージのやつ
        remix_best_score.text = PlayerPrefs.GetInt("SCORE_Remix", 0).ToString();
    }
}
