using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Manager : MonoBehaviour
{
    /*
    float Interval = 0.1f;

    Text _tex;

    float _time_cnt;
    int _frames;
    float _time_mn;
    float _fps;
    */

    private void Start()
    {
        // FPSの値を別の場所で指定しているなら以下の設定は消す
        Application.targetFrameRate = 60;

        // テキストコンポーネントの取得
       // _tex = this.GetComponent<Text>();
    }

    // FPSの表示と計算
    /*private void Update()
    {
        _time_mn -= Time.deltaTime;
        _time_cnt += Time.timeScale / Time.deltaTime;
        _frames++;

        if (0 < _time_mn) return;

        _fps = _time_cnt / _frames;
        _time_mn = Interval;
        _time_cnt = 0;
        _frames = 0;

        _tex.text = "FPS: " + _fps.ToString("f2");
    }*/
}