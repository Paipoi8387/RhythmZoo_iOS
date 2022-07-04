using UnityEngine;
using UnityEngine.UI;
using System;

public class Eat : MonoBehaviour
{
    [SerializeField] Rhythm_Manager rhythm_manager;

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip nomal_clip;
    [SerializeField] AudioClip double_clip;
    [SerializeField] AudioClip eat_clip;
    [SerializeField] AudioClip one_clip;
    [SerializeField] AudioClip two_clip;
    [SerializeField] AudioClip three_clip;
    [SerializeField] AudioClip four_clip;
    [SerializeField] AudioClip false_clip;
    AudioClip[] clip_array;

    //キャラクターの画像等
    [SerializeField] Sprite alpha_zero;

    [SerializeField] Image crocodile_image;
    [SerializeField] Sprite crocodile_set;
    [SerializeField] Sprite crocodile_eat;
    [SerializeField] Sprite crocodile_wrong;

    [SerializeField] Animator keepers_anim;
    [SerializeField] Image keepers_image;
    [SerializeField] Sprite keepers_nomal;
    [SerializeField] Sprite keepers_double;
    [SerializeField] Sprite keepers_fourth;
    Sprite[] keepers_array;

    [SerializeField] GameObject food_parent;
    [SerializeField] Image food_1_image;
    [SerializeField] Image food_2_image;
    [SerializeField] Image food_3_image;
    [SerializeField] Sprite food_nomal;
    [SerializeField] Sprite food_double;
    [SerializeField] Sprite food_fourth;

    int food_num; 
    public int food_type = 0;

    //デバッグ用
    [SerializeField] Text enemy_timing;
    [SerializeField] Text player_timing;

    int beat_count_roop(int num)
    {
        if (num < 0) num = 32 - (num * -1 % 32);
        else if (num > 31) num %= 32;
        return num;
    }



    private void Start()
    {
        clip_array = new AudioClip[] { one_clip, two_clip, three_clip, four_clip, nomal_clip, double_clip };      
        keepers_array = new Sprite[] { keepers_fourth, alpha_zero, alpha_zero, alpha_zero, keepers_nomal, keepers_double };
    }


    //diffが < 0　,、つまり1/4拍毎にやる処理
    public void Action_Every_Diff(int beat_count,int beat_count_in_turn, int turn_length,int max_turn_count, int[] timing_array)
    {
        Food_Drop(timing_array[beat_count_roop(beat_count_in_turn - 5)]);
        //csvの番号に応じた音声鳴らす
        Set_Food_Action(timing_array[beat_count_in_turn],beat_count_in_turn,beat_count, max_turn_count); //beat_count_in_turnは0～31

        if (beat_count_in_turn == turn_length - 1)
        {
            rhythm_manager.Switch_Turn(false); //引数にfalseを入れて、ターンが変わる前に強引にSet_Timing_Arrayを呼び出している
        }
    }


    //毎フレームやる処理
    public void Action_Every_Frame(int beat_count_in_turn, int[] timing_array, double detail_beat_count_in_turn)
    {
        if (beat_count_in_turn + 1 >= timing_array.Length) return; //timing_arrayより大きいとエラーが発生するため

        //本当は良くないが
        //押す側だと若干遅くたたいてしまうので調整用(Androidだと0.3でいいくらい)
        detail_beat_count_in_turn -= Button_Manager.detection_lag;

        if (Button_Manager.button_location != Button_Manager.Button_Location.Null)
        {
            //チャタリング対策
            player_timing.text = detail_beat_count_in_turn.ToString("F5");
            Button_Manager.button_location = Button_Manager.Button_Location.Null;
            if (!Judge_Rhythm(detail_beat_count_in_turn, timing_array))
            {
                crocodile_image.sprite = crocodile_wrong;
                sound_source.PlayOneShot(false_clip);
            }
            else
            {
                sound_source.PlayOneShot(eat_clip);
                crocodile_image.sprite = crocodile_eat;
                food_num -= 1;
                Food_Num_Change(food_num);
                rhythm_manager.Add_Score(1);
                player_timing.text = detail_beat_count_in_turn.ToString("F5");
            }

        }
        else if (!Button_Manager.is_touched && crocodile_image.sprite != crocodile_set)
        {
            crocodile_image.sprite = crocodile_set;
        }
    }

    //////////////////毎diffで使用する////////////////////////////////////


    void Set_Food_Action(int num, int beat_count_in_turn, int beat_count, int max_turn_count)
    {
        if (0 >= num || num > 6) return;
        //曲の最後のセットのbeat_countを求める式
        //曲の最後のセットは、セットしないようにする
        if (beat_count == 64 * max_turn_count - 4) return;

        sound_source.PlayOneShot(clip_array[num - 1]);
        keepers_image.sprite = keepers_array[num - 1];
        keepers_anim.Play("Keepers");
        food_type = num;
        enemy_timing.text = beat_count_in_turn.ToString("F5");
    }

    void Food_Num_Change(int food_num)
    {
        if (food_num == 0)
        {
            food_1_image.color = new Color(1, 1, 1, 0);
            food_2_image.color = new Color(1, 1, 1, 0);
            food_3_image.color = new Color(1, 1, 1, 0);
        }
        else if (food_num == 1)
        {
            food_1_image.color = new Color(1, 1, 1, 1);
            food_2_image.color = new Color(1, 1, 1, 0);
            food_3_image.color = new Color(1, 1, 1, 0);
        }
        else if (food_num == 2)
        {
            food_1_image.color = new Color(1, 1, 1, 1);
            food_2_image.color = new Color(1, 1, 1, 1);
            food_3_image.color = new Color(1, 1, 1, 0);
        }
        else if (food_num == 3)
        {
            food_1_image.color = new Color(1, 1, 1, 1);
            food_2_image.color = new Color(1, 1, 1, 1);
            food_3_image.color = new Color(1, 1, 1, 1);
        }
    }

    public void Food_Display(int food_type)
    {
        if (!(food_type == 1 || food_type == 5 || food_type == 6)) return;

        if (food_type == 1)
        {
            food_1_image.sprite = food_fourth;
            food_2_image.sprite = food_fourth;
            food_3_image.sprite = food_fourth;
            food_num = 3;
        }
        else if (food_type == 5)
        {
            food_1_image.sprite = food_nomal;
            food_2_image.sprite = food_nomal;
            food_3_image.sprite = food_nomal;
            food_num = 1;
        }
        else if (food_type == 6)
        {
            food_1_image.sprite = food_double;
            food_2_image.sprite = food_double;
            food_3_image.sprite = food_double;
            food_num = 2;
        }
        Food_Num_Change(food_num);
        food_parent.SetActive(true);
    }

    void Food_Drop(int food_type)
    {
        if ( !(food_type == 4 || food_type == 5 || food_type == 7)) return;
        food_parent.SetActive(false);

        //食べ物を消すタイミングで食べ物が残っていたら、ライフを減らす
        if (food_num > 0)
        {
            //フードドロップと食べるタイミングが重なるといけない
            rhythm_manager.Life_Setting(rhythm_manager.life_num - 1);
            sound_source.PlayOneShot(false_clip);
            food_num = 0;
        }
        //減らしたライフが0ならゲームオーバー
        if (rhythm_manager.life_num == 0)
        {
            rhythm_manager.Game_Over();
        }
    }

    //Remixで呼ぶ用
    public void Food_Panish()
    {
        food_num -= 1;
        Food_Num_Change(food_num);
    }

    //////////////////毎フレームで使用する//////////////////

    bool Judge_Rhythm(double detail_beat_count_in_turn, int[] timing_array)
    {
        bool is_success = false;
        //現在のdetail_beat_count_in_turnの±0.5を成功範囲とする
        int round_beat_count = (int)Math.Round(detail_beat_count_in_turn);
        if (round_beat_count == 32 || round_beat_count == 0) is_success = true;

        if (round_beat_count - 4 < 0) return is_success;
    
        if (timing_array[round_beat_count - 4] == 1) is_success = true;
        else if (timing_array[round_beat_count - 4] == 2) is_success = true;
        else if (timing_array[round_beat_count - 4] == 3) is_success = true;
        else if (timing_array[round_beat_count - 4] == 5) is_success = true;
        else if (timing_array[round_beat_count - 4] == 6) is_success = true;
        else if (timing_array[round_beat_count - 4] == 7) is_success = true;

        return is_success;
    }
}