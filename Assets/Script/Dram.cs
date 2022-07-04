using UnityEngine;
using UnityEngine.UI;
using System;

public class Dram : MonoBehaviour
{
    [SerializeField] Rhythm_Manager rhythm_manager;
    [SerializeField] Effect_Manager effect_manager;

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip dram_clip;
    [SerializeField] AudioClip symbal_clip;
    [SerializeField] AudioClip change_clip;
    [SerializeField] AudioClip correct_clip;

    [SerializeField] Animator enemy_dram_anim;
    [SerializeField] Animator enemy_symbal_anim;
    [SerializeField] Animator player_dram_anim;
    [SerializeField] Animator player_symbal_anim;
    [SerializeField] Animator bird_face_anim;


    [SerializeField] Transform bird_arm_trans;
    [SerializeField] Image bird_arm;
    [SerializeField] Sprite bird_right_arm;
    [SerializeField] Sprite bird_left_arm;
    [SerializeField] Sprite bird_no_arm;
    [SerializeField] Image bird_face;
    [SerializeField] Sprite bird_nomal;
    [SerializeField] Sprite bird_happy;
    [SerializeField] Sprite bird_sad;
    [SerializeField] Sprite bird_change;

    [SerializeField] Transform playerstick_trans;

    //ドラムやシンバルを鳴らした時の音のテキスト
    [SerializeField] GameObject Parent_Text;
    [SerializeField] GameObject Tam_Text;
    [SerializeField] GameObject Shan_Text;

    [SerializeField] Text enemy_timing;
    [SerializeField] Text player_timing;


    int beat_count_roop(int num)
    {
        if (num < 0) num = 32 - (num * -1 % 32);
        else if (num > 31) num %= 32;
        return num;
    }

    //diffが < 0　,、つまり1/4拍毎にやる処理
    public void Action_Every_Diff(double diff, bool is_myturn, int beat_count_in_turn, int turn_length, int[] timing_array, double detail_beat_count_in_turn)
    {
        //叩くべきタイミングで叩くのを逃した場合の判定
        if (is_myturn && timing_array[beat_count_roop(beat_count_in_turn - 2)] != 0) //ここでSet_SuccessJudge_Timing_Arrayによる0のセットが活きてくる
        {
            Wrong_Action();
        }

        //ターン変更時のclip再生
        if (beat_count_in_turn == turn_length - 2)
        {
            if (rhythm_manager.life_num >= 1 && is_myturn) rhythm_manager.Add_Score(rhythm_manager.life_num);

            //パーフェクトならhappy、1ミスならchange、2ミスはsad、3ミス以降はゲームオーバー
            if (rhythm_manager.life_num == 3 && is_myturn) bird_face.sprite = bird_happy;
            else if (rhythm_manager.life_num == 2 && is_myturn) bird_face.sprite = bird_change;
            else if (rhythm_manager.life_num == 1 && is_myturn) bird_face.sprite = bird_sad;
            else if (rhythm_manager.life_num <= 0 && is_myturn) rhythm_manager.Game_Over();
            else bird_face.sprite = bird_change;

            if(is_myturn) rhythm_manager.Life_Setting(Rhythm_Manager.life_num_max);
        }
        else if (beat_count_in_turn == turn_length - 1)
        {
            sound_source.PlayOneShot(change_clip);
            rhythm_manager.Switch_Turn();
        }
        else if (beat_count_in_turn == 0)
        {
            bird_face.sprite = bird_nomal;
        }

        Bird_Stick_Motion(is_myturn, beat_count_in_turn, timing_array);
    }


    //毎フレームやる処理
    public void Action_Every_Frame(double diff, bool is_myturn, int beat_count_in_turn, int turn_length, int[] timing_array, double detail_beat_count_in_turn)
    {
        if (!is_myturn) return; //自分のターンでなければアーリーリターン

        if (beat_count_in_turn + 1 >= timing_array.Length) return; //timing_arrayより大きいとエラーが発生するため

        //本当は良くないが
        //押す側だと若干遅くたたいてしまうので調整用(Androidだと0.3でいいくらい)
        //Debug.Log(Button_Manager.detection_lag);
        detail_beat_count_in_turn -= Button_Manager.detection_lag;


        //ボタンに関してなぜか初回だけ若干のラグがある(特に音声)
        //事前に連打と化するとラグがなくなる(ついでに音声のラグもなくなるのは不思議)
        if (Button_Manager.button_location == Button_Manager.Button_Location.Left)
        {
            //チャタリング対策
            Button_Manager.button_location = Button_Manager.Button_Location.Null;

            sound_source.PlayOneShot(dram_clip);
            player_dram_anim.Play("Dram");
            playerstick_trans.localPosition = new Vector2(-450, -400);
            playerstick_trans.eulerAngles = new Vector3(0, 0, -30);

            Judge_Rhythm(1, detail_beat_count_in_turn, timing_array);
            //デバッグ用
            player_timing.text = detail_beat_count_in_turn.ToString("F5");
        }
        else if (Button_Manager.button_location == Button_Manager.Button_Location.Right)
        {
            //ボタンのチャタリング対策
            Button_Manager.button_location = Button_Manager.Button_Location.Null;

            sound_source.PlayOneShot(symbal_clip);
            player_symbal_anim.Play("Symbal");
            playerstick_trans.localPosition = new Vector2(450, -400);
            playerstick_trans.eulerAngles = new Vector3(0, 0, 30);

            Judge_Rhythm(2, detail_beat_count_in_turn, timing_array);
            //デバッグ用
            player_timing.text = detail_beat_count_in_turn.ToString("F5");
        }
    }


    //////////////////毎フレームと毎diffで共通して使用する//////////////////
    //タイミング判定成功時のアクション
    void Correct_Action()
    {
        //sound_source.PlayOneShot(correct_clip);
        bird_face.sprite = bird_nomal;
    }


    //タイミング判定失敗時のアクション
    void Wrong_Action()
    {
        bird_face.sprite = bird_sad;
        rhythm_manager.Life_Setting(rhythm_manager.life_num - 1);        
    }

    //毎フレームで使用する
    //叩いた時に成功か失敗かの判定
    void Judge_Rhythm(int touch_location, double detail_beat_count_in_turn, int[] timing_array)
    {
        //現在のdetail_beat_count_in_turnの±0.5を成功範囲とする
        int round_beat_count = (int)Math.Round(detail_beat_count_in_turn);
        if (timing_array[round_beat_count] == touch_location)
        {
            rhythm_manager.Set_SuccessJudge_Timing_Array(round_beat_count);
            Correct_Action();
        }
    }

    //////////////////毎diffで使用する////////////////////////////////////
    //ニワトリがドラムかシンバルを叩く処理
    void Bird_Stick_Motion(bool is_myturn, int beat_count_in_turn, int[] timing_array)
    {
        if (is_myturn) return;    //相手のターンでないなら、アーリーリターン

        //timing_arrayが0,1,2のどれかでにわとりの動作を変える
        if (timing_array[beat_count_in_turn] == 0)
        {
            bird_arm.sprite = bird_no_arm;
            bird_arm_trans.localPosition = new Vector2(0, 250f);
            return; //1か2の時だけ鶏の顔を動かしたいのでリターン
        }
        else if (timing_array[beat_count_in_turn] == 1)
        {
            sound_source.PlayOneShot(dram_clip);
            enemy_dram_anim.Play("Dram");
            bird_arm.sprite = bird_left_arm;
            bird_arm_trans.localPosition = new Vector2(-37.5f, 157.5f);
            
            effect_manager.Display_Effect(Tam_Text, Parent_Text);
            //デバッグ用
            enemy_timing.text = beat_count_in_turn.ToString("F5");
        }
        else if (timing_array[beat_count_in_turn] == 2)
        {
            sound_source.PlayOneShot(symbal_clip);
            enemy_symbal_anim.Play("Symbal");
            bird_arm.sprite = bird_right_arm;
            bird_arm_trans.localPosition = new Vector2(37.5f, 157.5f);

            effect_manager.Display_Effect(Shan_Text, Parent_Text);
            //デバッグ用
            enemy_timing.text = beat_count_in_turn.ToString("F5");
        }
        //鶏の顔をリズムに合わせて動かす
        //bird_face_anim.Play("Bird_Face");
        bird_face_anim.Play("Bird_Face_PP");
    }
}
