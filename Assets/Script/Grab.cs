using UnityEngine;
using UnityEngine.UI;
using System;

public class Grab : MonoBehaviour
{
    [SerializeField] Rhythm_Manager rhythm_manager;

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip change_clip;
    [SerializeField] AudioClip fish_jump_clip;
    [SerializeField] AudioClip bear_grab_clip;
    [SerializeField] AudioClip together_clip;

    [SerializeField] Image bear0_image;
    [SerializeField] Image bear1_image;
    [SerializeField] Image player_image;
    Image[] bear_image_array = new Image[2];
    [SerializeField] Sprite bear_nomal;
    [SerializeField] Sprite bear_success;
    [SerializeField] Sprite bear_failuer;
    [SerializeField] GameObject bear0_arm;
    [SerializeField] GameObject bear1_arm;
    [SerializeField] GameObject player_arm;
    GameObject[] bear_arm_array = new GameObject[2];

    [SerializeField] Animator rhythm_anim;

    [SerializeField] Animator fish0_anim;
    [SerializeField] Animator fish1_anim;
    [SerializeField] Animator fish2_anim;
    [SerializeField] Transform fish0_trans;
    [SerializeField] Transform fish1_trans;
    [SerializeField] Transform fish2_trans;
    Animator[] fish_anim_array = new Animator[3];

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
        fish_anim_array[0] = fish0_anim;
        fish_anim_array[1] = fish1_anim;
        fish_anim_array[2] = fish2_anim;

        bear_image_array[0] = bear0_image;
        bear_image_array[1] = bear1_image;

        bear_arm_array[0] = bear0_arm;
        bear_arm_array[1] = bear1_arm;
    }

    //diffが < 0　,、つまり1/4拍毎にやる処理
    public void Action_Every_Diff(int beat_count_in_turn, int turn_length, int[] timing_array)
    {
        if (beat_count_in_turn % 4 == 0) rhythm_anim.Play("Grab_Rhythm");

        if (beat_count_in_turn == turn_length - 1)
        {
            rhythm_manager.Switch_Turn(false); //引数にfalseを入れて、ターンが変わる前に強引にSet_Timing_Arrayを呼び出している
            //sound_source.PlayOneShot(change_clip);
        }

        Bear_Action(timing_array[beat_count_in_turn],beat_count_in_turn);
        Fish_Action(timing_array[beat_count_in_turn]);

        Judge_Damage(timing_array[beat_count_roop(beat_count_in_turn - 3)]);
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
            Button_Manager.button_location = Button_Manager.Button_Location.Null;

            player_arm.SetActive(true);
            player_timing.text = detail_beat_count_in_turn.ToString("F5");
            if (timing_array[(int)Math.Round(detail_beat_count_in_turn) ] == 7 
             || timing_array[(int)Math.Round(detail_beat_count_in_turn)] == 10
            //if (timing_array[beat_count_in_turn] == 9
            // || timing_array[beat_count_in_turn] == 10
             || timing_array[beat_count_in_turn] == 11)
            {
                fish2_anim.Play("Fish_Get");
                player_image.sprite = bear_success;
                sound_source.PlayOneShot(bear_grab_clip);
                rhythm_manager.Add_Score(3);
            }
            else
            {
                player_image.sprite = bear_failuer;
            }
        }
        else if (!Button_Manager.is_touched && player_image.sprite != bear_nomal)
        {
            player_image.sprite = bear_nomal;
            player_arm.SetActive(false);
        }
        //Debug.Log(Button_Manager.button_location);
        //Debug.Log(Button_Manager.is_touched);
        //Debug.Log(player_image.sprite != bear_nomal);
    }

    
    void Judge_Damage(int num)
    {
        if ( !(num == 10 || num == 11) ) return;
        if (player_image.sprite == bear_success) return;
        rhythm_manager.Life_Setting(rhythm_manager.life_num - 1);
        //減らしたライフが0ならゲームオーバー
        if (rhythm_manager.life_num == 0)
        {
            rhythm_manager.Game_Over();
        }
    }

    //魚の動きをcsvの1～4にする
    void Fish_Action(int num)
    {
        if (num == 1 || num == 2 || num == 3)
        {
            fish_anim_array[num - 1].Play("Fish_Up");
            sound_source.PlayOneShot(fish_jump_clip);
        }
        else if (num == 4)
        {
            fish_anim_array[0].Play("Fish_Down");
            fish_anim_array[1].Play("Fish_Down");
            fish_anim_array[2].Play("Fish_Down");
        }
        else if (num == 5 || num == 6)
        {
            fish_anim_array[num - 5].Play("Fish_Get");
        }
        else if (num == 7)
        {
            fish_anim_array[0].Play("Fish_Up");
            fish_anim_array[1].Play("Fish_Up");
            fish_anim_array[2].Play("Fish_Up");
        }
        else if (num == 11)
        {
            fish_anim_array[0].Play("Fish_Get");
            fish_anim_array[1].Play("Fish_Get");
        }
    }

    //Remixで呼ばれる用
    public void Fish_Initiate()
    {
        fish0_trans.localPosition = new Vector2(-500, -600);
        fish1_trans.localPosition = new Vector2(0, -600);
        fish2_trans.localPosition = new Vector2(500, -600);
    }

    void Bear_Action(int num,double beat_count_in_turn)
    {
        if(num == 5 || num == 6)
        {
            bear_image_array[num - 5].sprite = bear_success;
            bear_arm_array[num - 5].SetActive(true);
            sound_source.PlayOneShot(bear_grab_clip);
            enemy_timing.text = beat_count_in_turn.ToString("F5");
        }
        else if(num == 4)
        {
            bear_image_array[0].sprite = bear_nomal;
            bear_image_array[1].sprite = bear_nomal;
            bear_arm_array[0].SetActive(false);
            bear_arm_array[1].SetActive(false);
        }
        else if(num == 8)
        {
            sound_source.PlayOneShot(together_clip);
        }
        else if (num == 11)
        {
            bear_image_array[0].sprite = bear_success;
            bear_image_array[1].sprite = bear_success;
            bear_arm_array[0].SetActive(true);
            bear_arm_array[1].SetActive(true);
            sound_source.PlayOneShot(bear_grab_clip);
            enemy_timing.text = beat_count_in_turn.ToString("F5");
        }
    }
}
