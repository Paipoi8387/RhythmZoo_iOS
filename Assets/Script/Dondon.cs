using UnityEngine;
using UnityEngine.UI;
using System;

public class Dondon : MonoBehaviour
{
    [SerializeField] Rhythm_Manager rhythm_manager;

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip change_clip;
    [SerializeField] AudioClip success_clap_clip;
    [SerializeField] AudioClip success_voice_clip;
    [SerializeField] AudioClip curtain_clip;
    [SerializeField] AudioClip failuer_clip;

    [SerializeField] GameObject Light;

    [SerializeField] Animator rhythm_anim;
    [SerializeField] Animator curtain_anim;

    [SerializeField] Image gorilla_image;
    [SerializeField] Sprite gorilla_nomal;
    [SerializeField] Sprite gorilla_smile;
    [SerializeField] Sprite gorilla_cool;

    [SerializeField] Image order_image;
    [SerializeField] Sprite order_smile;
    [SerializeField] Sprite order_cool;
    [SerializeField] Sprite order_nomal;

    bool is_success = false;

    int beat_count_roop(int num)
    {
        if (num < 0) num = 32 - (num * -1 % 32);
        else if (num > 31) num %= 32;
        return num;
    }


    public void Action_Every_Diff(int beat_count_in_turn, int turn_length, int[] timing_array)
    {
        if (beat_count_in_turn % 4 == 0) rhythm_anim.Play("Grab_Rhythm");

        if (beat_count_in_turn == turn_length - 1)
        {
            rhythm_manager.Switch_Turn(false); //引数にfalseを入れて、ターンが変わる前に強引にSet_Timing_Arrayを呼び出している
            //sound_source.PlayOneShot(change_clip);
        }

        int num = timing_array[beat_count_in_turn];
        Light_Animation(num);
        Curtain_Animation(num);
        Order_Animation(num);

        //成功判定
        Judge_Rhytym(timing_array[beat_count_roop(beat_count_in_turn - 2)]);
    }


    public void Action_Every_Frame(int[] timing_array, double detail_beat_count_in_turn)
    {
        //本当は良くないが
        //押す側だと若干遅くたたいてしまうので調整用(Androidだと0.3でいいくらい)
        detail_beat_count_in_turn -= Button_Manager.detection_lag;

        if (Button_Manager.button_location == Button_Manager.Button_Location.Right)
        {
            Button_Manager.button_location = Button_Manager.Button_Location.Null;
            gorilla_image.sprite = gorilla_smile;
            //Debug.Log(timing_array[beat_count_roop((int)Math.Round(detail_beat_count_in_turn))]);
            //成功判定
            int num = timing_array[beat_count_roop((int)Math.Round(detail_beat_count_in_turn))];
            if (num == 12 && gorilla_image.sprite == gorilla_smile)
            {
                sound_source.PlayOneShot(success_clap_clip);
                sound_source.PlayOneShot(success_voice_clip);
                is_success = true;
                rhythm_manager.Add_Score(2);
            }
        }
        else if (Button_Manager.button_location == Button_Manager.Button_Location.Left)
        {
            Button_Manager.button_location = Button_Manager.Button_Location.Null;
            gorilla_image.sprite = gorilla_cool;
            //Debug.Log(timing_array[beat_count_roop((int)Math.Round(detail_beat_count_in_turn))]);
            //成功判定
            int num = timing_array[beat_count_roop((int)Math.Round(detail_beat_count_in_turn))];
            if (num == 13 && gorilla_image.sprite == gorilla_cool)
            {
                sound_source.PlayOneShot(success_clap_clip);
                sound_source.PlayOneShot(success_voice_clip);
                is_success = true;
                rhythm_manager.Add_Score(2);
            }
        }
        else if (!Button_Manager.is_touched && gorilla_image.sprite != gorilla_nomal)
        {
            gorilla_image.sprite = gorilla_nomal;
        }

        /*//成功判定
        int num = beat_count_roop((int)Math.Round(detail_beat_count_in_turn));
        if (!(num == 11 || num == 12 || num == 13)) return;

        if ( (num == 11 && gorilla_image.sprite == gorilla_nomal)
          || (num == 12 && gorilla_image.sprite == gorilla_smile)
          || (num == 13 && gorilla_image.sprite == gorilla_cool) )
        {
            sound_source.PlayOneShot(success_clap_clip);
            sound_source.PlayOneShot(success_voice_clip);
            is_success = true;
            rhythm_manager.Add_Score(2);
        }*/
    }

    //失敗判定
    void Judge_Rhytym(int num)
    {
        if ((num == 11 || num == 12 || num == 13) && !is_success)
        {
            sound_source.PlayOneShot(failuer_clip);
            rhythm_manager.Life_Setting(rhythm_manager.life_num - 1);
            //減らしたライフが0ならゲームオーバー
            if (rhythm_manager.life_num == 0)
            {
                rhythm_manager.Game_Over();
            }
        }
    }

    void Curtain_Animation(int num)
    {
        if (num == 1 || num == 2 || num == 3)
        {
            curtain_anim.Play("Curtain_Open");
            sound_source.PlayOneShot(curtain_clip);
            is_success = false;
        }
        else if (num == 4)
        {
            curtain_anim.Play("Curtain_Close");
        }
    }

    void Light_Animation(int num)
    {
        if (num == 11 || num == 12 || num == 13)
        {
            Light.SetActive(true);
        }
        else if (num == 4)
        {
            Light.SetActive(false);
        }
    }

    void Order_Animation(int num)
    {
        if(num == 1)
        {
            order_image.sprite = order_nomal;
            order_image.color = new Color(194f / 255f, 232f / 255f, 255f / 255f, 1);
        }
        else if (num == 2)
        {
            order_image.sprite = order_smile;
            order_image.color = new Color(255f / 255f, 194f / 255f, 237f / 255f, 1);
        }
        else if (num == 3)
        {
            order_image.sprite = order_cool;
            order_image.color = new Color(255f / 255f, 255f / 255f, 194f / 255f, 1);
        }
    }
}