using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Rhythm_Manager : MonoBehaviour
{
    [SerializeField] Dram dram;
    [SerializeField] GameObject Dram_Image;
    [SerializeField] AudioClip dram_bgm;
    const double dram_bpm = 130.02;
    const int  dram_max_turn_count = 20;

    [SerializeField] Eat eat;
    [SerializeField] GameObject Eat_Image;
    [SerializeField] AudioClip eat_bgm;
    const double eat_bpm = 130.509;
    const int eat_max_turn_count = 17;

    [SerializeField] Grab grab;
    [SerializeField] GameObject Grab_Image;
    [SerializeField] AudioClip grab_bgm;
    const double grab_bpm = 120;
    const int grab_max_turn_count = 22;

    [SerializeField] Clap clap;
    [SerializeField] GameObject Clap_Image;
    [SerializeField] AudioClip clap_bgm;
    const double clap_bpm = 115.01;
    const int clap_max_turn_count = 14;

    [SerializeField] Dondon dondon;
    [SerializeField] GameObject Dondon_Image;
    [SerializeField] AudioClip dondon_bgm;
    const double dondon_bpm = 110;
    const int dondon_max_turn_count = 16;

    [SerializeField] AudioClip remix_bgm;
    const double remix_bpm = 130.01;
    const int remix_max_turn_count = 30;
    int remix_action_num = 1; //どのアクションを選択しているかの番号
    int[] ran_num_array = new int[4]; //次に選択するアクション候補の番号のリスト

    [SerializeField] Text score_text;
    [SerializeField] Text score_text_gameover;
    [SerializeField] Text coin_text_gameover;
    [SerializeField] GameObject GameOver;
    [SerializeField] GameObject Best_Score_Decollate;
    [SerializeField] GameObject Music_Roop;

    [SerializeField] AudioSource bgm_source;
    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip beat_clip;
    [SerializeField] AudioClip gameover_clip;

    //bool is_start = false;
    enum State { Null, Start, Rest, Stop, Finish}; //ゲームをスタートしたか、していないか
    State state = State.Null;

    //bpmなどのタイミングを取得するのに必要な変数
    double bpm = 130.02;
    int beat_count = 1; //一拍毎に加算
    const int beat_div = 4; //裏拍などのためにbpmにバイアスをかける　2のべき乗　4の場合は裏拍の裏拍
    const double record_num_per_sec = 44100; //一秒辺りのデータ録音回数
    double diff; //一拍の間に1～0の値を取る
    
    string stage_name; //PlayerPrefsなどの際にも使用するステージ名

    bool is_myturn = false;
    int turn_count = 1; //何ターン経過したか　判定の時に使うくらいかな

    int score = 0; //こいつが結果に使われるので一番大事

    static readonly int turn_length = 32; //一ターンの長さ
    int max_turn_count; //Musicループする条件で使用する
    int beat_count_in_turn = 1;  //1～16の値を取らせたい
    double detail_beat_count_in_turn; //beat_count_in_turn + diff

    int[] timing_array = new int[turn_length];  //敵の攻撃のタイミングのリストで0～16の値がランダムで入る

    //csv関係
    TextAsset CSV_File;
    List<string[]> csv_data = new List<string[]>();
    
    //ライフ関連
    public const int life_num_max = 3;
    public int life_num = 3;
    [SerializeField] Image HP3;
    [SerializeField] Image HP2;
    [SerializeField] Image HP1;

    [SerializeField] GameObject Tap_Text;
    [SerializeField] Text turn_text;


    void Start()
    {
        stage_name = PlayerPrefs.GetString("STAGE_NAME", "");
        //stage_name = "Remix";

        if (stage_name == "Dram")
        { Action_Initiate("Dram", dram_bgm, Dram_Image, dram_bpm, dram_max_turn_count); }
        else if (stage_name == "Eat")
        { Action_Initiate("Eat", eat_bgm, Eat_Image, eat_bpm, eat_max_turn_count); }
        else if (stage_name == "Grab")
        { Action_Initiate("Grab", grab_bgm, Grab_Image, grab_bpm, grab_max_turn_count); }
        else if (stage_name == "Clap")
        { Action_Initiate("Clap", clap_bgm, Clap_Image, clap_bpm, clap_max_turn_count); }
        else if (stage_name == "Dondon")
        { Action_Initiate("Dondon", dondon_bgm, Dondon_Image, dondon_bpm, dondon_max_turn_count); }
        else if (stage_name == "Remix") //最初のステージはGrabにする
        {
            Action_Initiate("Grab", remix_bgm, Grab_Image, remix_bpm, remix_max_turn_count);
        }
    }

    void Update()
    {
        //イベントトリガーの起動用に一回ボタンを押させる
        if(state == State.Null && Button_Manager.button_location != Button_Manager.Button_Location.Null)
        {
            Tap_Text.SetActive(false);
            state = State.Finish;
            Invoke("Music_Start", 1f);
        }


        if (state != State.Start && state != State.Rest) return;

        //bpm * beat_divで裏拍を取れる
        diff = beat_count - (bgm_source.timeSamples / record_num_per_sec) * (bpm * beat_div/ 60);

        if (diff < 0)
        {
            beat_count++;
            beat_count_in_turn = beat_count % turn_length;
            //ここにも同じ処理を書けば、beat_count_in_turnとdiffを足しても変な値にならない
            diff = beat_count - (bgm_source.timeSamples / record_num_per_sec) * (bpm * beat_div / 60);
            if (beat_count_in_turn % 4 == 0) sound_source.PlayOneShot(beat_clip);
            if (state == State.Start) Action_Every_Diff();
        }
        else if (diff > 2) //曲が終わった証　おそらく21ターン目の敵の番で曲が終わるので、20ターン目の自分の番が終了後に何かした方が良い
        {
            beat_count = 1;
            beat_count_in_turn = 1;
            state = State.Start;
            Music_Roop.SetActive(false);
        }
        detail_beat_count_in_turn = beat_count_in_turn + (1 - diff);
        if(state == State.Start) Action_Every_Frame();

    }

    //背景や音楽、CSVの読み込みなどを行う予定
    void Action_Initiate(string csv_name, AudioClip clip_name, GameObject back_image, double bpm_, int max_turn_count_)
    {
        if (stage_name == "Remix")
        {
            Load_CSV("Dram");
            Load_CSV("Eat");
            Load_CSV("Grab");
            Load_CSV("Clap");
            Load_CSV("Dondon");
            Remix_Switch();
        }
        else
        {
            Load_CSV(csv_name);
            back_image.SetActive(true);
            remix_action_num = 1;
        }
        Set_Timing_Array();
        bgm_source.clip = clip_name;
        bpm = bpm_;
        max_turn_count = max_turn_count_;
    }

    void Action_Every_Frame()
    {
        //引数が多いが、staticが多いのも気持ち悪かったので…
        if (stage_name == "Dram")
        {
            dram.Action_Every_Frame(diff, is_myturn, beat_count_in_turn, turn_length, timing_array, detail_beat_count_in_turn);
        }
        else if (stage_name == "Eat")
        {
            eat.Action_Every_Frame(beat_count_in_turn, timing_array, detail_beat_count_in_turn);
        }
        else if (stage_name == "Grab")
        {
            grab.Action_Every_Frame(beat_count_in_turn, timing_array, detail_beat_count_in_turn);
        }
        else if (stage_name == "Clap")
        {
            clap.Action_Every_Frame(timing_array,detail_beat_count_in_turn);
        }
        else if (stage_name == "Dondon")
        {
            dondon.Action_Every_Frame(timing_array, detail_beat_count_in_turn);
        }
        else if (stage_name == "Remix")
        {
            if(remix_action_num == 1) dram.Action_Every_Frame(diff, is_myturn, beat_count_in_turn, turn_length, timing_array, detail_beat_count_in_turn);
            else if (remix_action_num == 2) eat.Action_Every_Frame(beat_count_in_turn, timing_array, detail_beat_count_in_turn);
            else if (remix_action_num == 3) grab.Action_Every_Frame(beat_count_in_turn, timing_array, detail_beat_count_in_turn);
            else if (remix_action_num == 4) clap.Action_Every_Frame(timing_array, detail_beat_count_in_turn);
            else if (remix_action_num == 5) dondon.Action_Every_Frame(timing_array, detail_beat_count_in_turn);
        }
    }


    void Action_Every_Diff()
    {
        //引数が多いが、staticが多いのも気持ち悪かったので…
        if (stage_name == "Dram")
        {
            dram.Action_Every_Diff(diff, is_myturn, beat_count_in_turn, turn_length, timing_array, detail_beat_count_in_turn);
        }
        else if (stage_name == "Eat")
        {
            eat.Action_Every_Diff(beat_count, beat_count_in_turn, turn_length, max_turn_count, timing_array);
        }
        else if (stage_name == "Grab")
        {
            grab.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
        }
        else if (stage_name == "Clap")
        {
            clap.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
        }
        else if (stage_name == "Dondon")
        {
            dondon.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
        }
        else if (stage_name == "Remix")
        {
            if (remix_action_num == 1) dram.Action_Every_Diff(diff, is_myturn, beat_count_in_turn, turn_length, timing_array, detail_beat_count_in_turn);
            else if (remix_action_num == 2) eat.Action_Every_Diff(beat_count, beat_count_in_turn, turn_length, max_turn_count, timing_array);
            else if (remix_action_num == 3) grab.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
            else if (remix_action_num == 4) clap.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
            else if (remix_action_num == 5) dondon.Action_Every_Diff(beat_count_in_turn, turn_length, timing_array);
        }
    }

    public void Add_Score(int num)
    {
        score += num;
        score_text.text = "Score " + score.ToString();
    }


    void Remix_Switch()
    {
        if (stage_name != "Remix") return;

        if (remix_action_num == 1) Dram_Image.SetActive(false);
        else if (remix_action_num == 2)
        {
            Eat_Image.SetActive(false);
            eat.Food_Panish();
        }
        else if (remix_action_num == 3)
        {
            Grab_Image.SetActive(false);
            grab.Fish_Initiate();
        }
        else if (remix_action_num == 4) Clap_Image.SetActive(false);
        else if (remix_action_num == 5) Dondon_Image.SetActive(false);

        int j = 0;
        for (int i = 0; i < 5; i++)
        {
            if (remix_action_num == i + 1) continue;
            ran_num_array[j] = i + 1;
            j++;
        }
        //Debug.Log(ran_num_array[0].ToString() + "," + ran_num_array[1].ToString() + "," + ran_num_array[2].ToString() + "," + ran_num_array[3].ToString());
        remix_action_num = ran_num_array[Random.Range(0, 4)];
        //remix_action_num = Random.Range(2, 3);

        if (remix_action_num == 1) Dram_Image.SetActive(true);
        else if (remix_action_num == 2) Eat_Image.SetActive(true);
        else if (remix_action_num == 3) Grab_Image.SetActive(true);
        else if (remix_action_num == 4) Clap_Image.SetActive(true);
        else if (remix_action_num == 5) Dondon_Image.SetActive(true);
    }


    //敵のターンと相手のターンの切り替え
    //各アクションの方から呼び出すことでターンが存在するやつとしないやつに対応できる
    public void Switch_Turn(bool is_turn_devide = true)
    {
        is_myturn = !is_myturn;
        if(!is_myturn)
        {
            Remix_Switch();
            state = State.Start;
            turn_count++;
            turn_text.text = turn_count.ToString();
            Set_Timing_Array(); //相手のターンに切り替わったタイミングでないと、自分のターンと相手のターンの配列の中身が違ってしまう
        }
        else if (is_myturn)
        {
            //ここでSet_Timing_Arrayが描き変わってしまうので1,2,3,4が上手くいかない
            if (!is_turn_devide) Set_Timing_Array();
        }

        if (turn_count > max_turn_count)
        {
            state = State.Rest;
            turn_count = 1;
            //このroop_numを得点計算に使用する
            Music_Roop.SetActive(true);
        }
    }


    void Set_Timing_Array()
    {
        int attack_type_num = Random.Range(0, 3);
        for (int i = 0; i < turn_length; i++)
        {
            timing_array[i] = int.Parse(csv_data[(remix_action_num - 1) * 3 + attack_type_num][i]);
        }
    }

    //成功判定の際にTiming_Arrayの対象番目の0以外の数値を0にする
    //『Timing_Arrayの現在のbeat_count_in_turnよりも少し後の方が0かどうか』を確認することで失敗かどうか分かる
    public void Set_SuccessJudge_Timing_Array(int num)
    {
        timing_array[num] = 0;
    }

    void Music_Start()
    {
        state = State.Start;
        bgm_source.Play();
    }

    public void Game_Over()
    {
        state = State.Finish;
        bgm_source.Stop();
        sound_source.PlayOneShot(gameover_clip);
        GameOver.SetActive(true);
        score_text_gameover.text = "Score " + score.ToString();
        Judge_BestScore();
        //コイン関係
        int coin_num = PlayerPrefs.GetInt("COIN_NUM", 0) + score;
        PlayerPrefs.SetInt("COIN_NUM", coin_num);
        coin_text_gameover.text = "Get " + score.ToString() + " Coins";

    }


    public void Life_Setting(int life_goal)
    {
        life_num = life_goal;
        //life_num = 3;

        if (life_num == 3)
        {
            HP1.color = new Color(1, 0, 0, 1);
            HP2.color = new Color(1, 0, 0, 1);
            HP3.color = new Color(1, 0, 0, 1);
        }
        else if (life_num == 2) HP3.color = new Color(0.5f, 0.5f, 0.5f, 1);
        else if (life_num == 1) HP2.color = new Color(0.5f, 0.5f, 0.5f, 1);
        else if (life_num == 0) HP1.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }


    //ゲームオーバー時にベストスコア更新かどうかを確認する
    void Judge_BestScore()
    {
        bool update_bestscore = false;

        if (score > PlayerPrefs.GetInt("SCORE_" + stage_name, 0)) update_bestscore = true;

        if (update_bestscore)
        {
            Best_Score_Decollate.SetActive(true);
            PlayerPrefs.SetInt("SCORE_" + stage_name, score);
        }
    }

    //csv読み込み
    void Load_CSV(string file_name)
    {
        CSV_File = Resources.Load(file_name) as TextAsset; // Resouces下のCSV読み込み        
        StringReader Reader = new StringReader(CSV_File.text);
        while (Reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string[] Line_List = Reader.ReadLine().Split(','); // 一行ずつ読み込み
            csv_data.Add(Line_List);
        }
    }
}
