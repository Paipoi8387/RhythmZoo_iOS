using UnityEngine;
using UnityEngine.UI;
using System;

public class Clap : MonoBehaviour
{
    [SerializeField] Rhythm_Manager rhythm_manager;

    [SerializeField] AudioSource sound_source;
    [SerializeField] AudioClip change_clip;
    [SerializeField] AudioClip wan_clip;
    [SerializeField] AudioClip wan_plus_clip;
    [SerializeField] AudioClip wan_minus_clip;
    [SerializeField] AudioClip nya_clip;
    [SerializeField] AudioClip nya_plus_clip;
    [SerializeField] AudioClip nya_minus_clip;
    [SerializeField] AudioClip ready_clip;
    [SerializeField] AudioClip dog_voice_clip;
    [SerializeField] AudioClip cat_voice_clip;
    [SerializeField] AudioClip dog_player_clip;
    [SerializeField] AudioClip cat_player_clip;
    [SerializeField] AudioClip failuer_clip;

    [SerializeField] Animator rhythm_anim;

    [SerializeField] Image backimage;
    //アイドルの画像
    [SerializeField] Image idol_image;
    [SerializeField] Sprite idol_dog;
    [SerializeField] Sprite idol_cat;
    [SerializeField] Sprite idol_ready;
    [SerializeField] Sprite idol_no;
    //イヌの画像
    [SerializeField] Image dog1_image;
    [SerializeField] Image dog2_image;
    [SerializeField] Image dog_player_image;
    [SerializeField] Sprite dog_nomal;
    [SerializeField] Sprite dog_jump;
    //ネコの画像
    [SerializeField] Image cat1_image;
    [SerializeField] Image cat2_image;
    [SerializeField] Image cat_player_image;
    [SerializeField] Sprite cat_nomal;
    [SerializeField] Sprite cat_jump;

    [SerializeField] GameObject Angry_Mark;

    bool[] judge_array = new bool[3] {false,false,false};



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
            judge_array[0] = false;
            judge_array[1] = false;
            judge_array[2] = false;
        }

        Idol_Action(timing_array[beat_count_in_turn]);
        Dog_Action(timing_array[beat_count_in_turn]);
        Cat_Action(timing_array[beat_count_in_turn]);

        //Every_Frameだと画像を戻すタイミングが速いので、Every_Diffで行う
        /*if (Button_Manager.button_location == Button_Manager.Button_Location.Null)
        {
            if (cat_player_image.sprite == cat_jump) cat_player_image.sprite = cat_nomal;
            if (dog_player_image.sprite == dog_jump) dog_player_image.sprite = dog_nomal;
        }*/
        if (Angry_Mark.activeSelf) Angry_Mark.SetActive(false);
        //ダメージ判定
        Judge_Damage(timing_array[beat_count_roop(beat_count_in_turn - 2)]);
    }


    public void Action_Every_Frame(int[] timing_array, double detail_beat_count_in_turn)
    {
        backimage.color = Color.HSVToRGB(Time.time % 1, 0.5f, 0.5f);

        if (Button_Manager.button_location == Button_Manager.Button_Location.Left)
        {
            Button_Manager.button_location = Button_Manager.Button_Location.Null;
            sound_source.PlayOneShot(dog_player_clip);
            int round_beat_count = (int)Math.Round(detail_beat_count_in_turn);
            //音楽ループする時にdiffがめっちゃ大きい値になり、detail_beat_count_in_turnがめっちゃ小さい負の値になる
            //そこでbeat_count_roopに入れても、数字が負になり、エラーが発生してた
            int num = timing_array[beat_count_roop(round_beat_count)];
            dog_player_image.sprite = dog_jump;
            Judge_Success(0, num);
        }
        else if (Button_Manager.button_location == Button_Manager.Button_Location.Right)
        {
            Button_Manager.button_location = Button_Manager.Button_Location.Null;
            sound_source.PlayOneShot(cat_player_clip);
            int round_beat_count = (int)Math.Round(detail_beat_count_in_turn);
            //音楽ループする時にdiffがめっちゃ大きい値になり、detail_beat_count_in_turnがめっちゃ小さい負の値になる
            //そこでbeat_count_roopに入れても、数字が負になり、エラーが発生してた
            int num = timing_array[beat_count_roop(round_beat_count)];
            cat_player_image.sprite = cat_jump;
            Judge_Success(1, num);
        }
        else if (!Button_Manager.is_touched && dog_player_image.sprite != dog_nomal)
        {
            dog_player_image.sprite = dog_nomal;
        }
        else if (!Button_Manager.is_touched && cat_player_image.sprite != cat_nomal)
        {
            cat_player_image.sprite = cat_nomal;
        }
    }


    void Judge_Success(int button_num, int num)
    {
        if ((num == 11 || num == 12 || num == 13) && button_num == 0)
        {
            judge_array[num - 11] = true;
            rhythm_manager.Add_Score(2);
        }
        else if ((num == 15 || num == 16 || num == 17) && button_num == 1)
        {
            judge_array[num - 15] = true;
            rhythm_manager.Add_Score(2);
        }
    }

    void Judge_Damage(int num)
    {
        if (num == 11 || num == 12 || num == 13)
        {
            if(!judge_array[num - 11]) Cheer_Failuer();
        }
        else if (num == 15 || num == 16 || num == 17)
        {
            if (!judge_array[num - 15]) Cheer_Failuer();
        }
    }

    void Cheer_Failuer()
    {
        sound_source.PlayOneShot(failuer_clip);
        Angry_Mark.SetActive(true);
        rhythm_manager.Life_Setting(rhythm_manager.life_num - 1);
        //減らしたライフが0ならゲームオーバー
        if (rhythm_manager.life_num == 0)
        {
            rhythm_manager.Game_Over();
        }
    }

    void Idol_Action(int num)
    {
        if (num == 1) sound_source.PlayOneShot(wan_clip);
        else if (num == 2) sound_source.PlayOneShot(wan_minus_clip);
        else if (num == 3) sound_source.PlayOneShot(wan_plus_clip);
        else if (num == 5) sound_source.PlayOneShot(nya_clip);
        else if (num == 6) sound_source.PlayOneShot(nya_minus_clip);
        else if (num == 7) sound_source.PlayOneShot(nya_plus_clip);
        else if (num == 10) sound_source.PlayOneShot(ready_clip);

        if (num == 0) idol_image.sprite = idol_no;
        else if (num == 1 || num == 2 || num == 3) idol_image.sprite = idol_dog;
        else if (num == 5 || num == 6 || num == 7) idol_image.sprite = idol_cat;
        else if (num == 10) idol_image.sprite = idol_ready;
    }

    void Dog_Action(int num)
    {
        if (num == 11 || num == 12 || num == 13)
        {
            dog1_image.sprite = dog_jump;
            dog2_image.sprite = dog_jump;
            sound_source.PlayOneShot(dog_voice_clip);
            sound_source.PlayOneShot(dog_voice_clip);
        }
        else if (num == 14)
        {
            dog1_image.sprite = dog_nomal;
            dog2_image.sprite = dog_nomal;
        }
    }

    void Cat_Action(int num)
    {
        if (num == 15 || num == 16 || num == 17)
        {
            cat1_image.sprite = cat_jump;
            cat2_image.sprite = cat_jump;
            sound_source.PlayOneShot(cat_voice_clip);
            sound_source.PlayOneShot(cat_voice_clip);
        }
        else if (num == 18)
        {
            cat1_image.sprite = cat_nomal;
            cat2_image.sprite = cat_nomal;
        }
    }
}
