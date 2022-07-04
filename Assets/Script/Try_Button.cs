using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Try_Button : MonoBehaviour
{
    [SerializeField] Text Touch_Text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Button_Manager.button_location.ToString() == "Null") Touch_Text.text = "";
        else
        {
            Touch_Text.text = Button_Manager.button_location.ToString();
            //ここでデバッグとして出力されているならば、テキスト上で確認できずとも、LateUpdateが活きている証拠
            //テキストは若干処理が重いので反映されない可能性がある
            //Debug.Log(Button_Manager.button_location.ToString());
        }
    }
}
