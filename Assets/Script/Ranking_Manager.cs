using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;

public class Ranking_Manager : MonoBehaviour
{
    [SerializeField] Text ranking_name_text;
    [SerializeField] Text score_text;
    int seq_num = 0; //同じ順位が何回続いたか
    string before_score = "";

    [SerializeField] ScrollRect scrollrect;
    float my_rank = 0;
    float all_rank = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Register_Data_First("Eat");
    }

    //初回時にデータ登録とObjectIdを取得
    //以後、ステージを増やすときはここでstage_nameを指定する
    public void Register_Data_First(string stage_name)
    {
        // クラスのNCMBObjectを作成
        NCMBObject RankingClass = new NCMBObject("RankingClass");
        //ここでどのランキングを使うか決める
        if (stage_name == "Dram") RankingClass = new NCMBObject("DramRankingClass");
        else if (stage_name == "Eat") RankingClass = new NCMBObject("EatRankingClass");
        else if (stage_name == "Grab") RankingClass = new NCMBObject("GrabRankingClass");
        else if (stage_name == "Clap") RankingClass = new NCMBObject("ClapRankingClass");
        else if (stage_name == "Dondon") RankingClass = new NCMBObject("DondonRankingClass");
        else if (stage_name == "Remix") RankingClass = new NCMBObject("RemixRankingClass");

        RankingClass["name"] = PlayerPrefs.GetString("ACCOUNT", "No Name");
        RankingClass["score"] = 0;
        // データストアへの登録
        RankingClass.SaveAsync((NCMBException e) =>
        {
            if (e == null) //ここでObjectIdを取得することで後にデータの更新が出来る
            {
                PlayerPrefs.SetString("OBJECT_ID_"+ stage_name, RankingClass.ObjectId);
                Debug.Log(RankingClass.ObjectId);
                PlayerPrefs.Save();
            }
        });
    }

    public void Register_Data(string stage_name)
    {
        // クラスのNCMBObjectを作成
        NCMBObject RankingClass = new NCMBObject("RankingClass");
        RankingClass = new NCMBObject(stage_name + "RankingClass");
        RankingClass.ObjectId = PlayerPrefs.GetString("OBJECT_ID_" + stage_name, RankingClass.ObjectId);
        RankingClass["name"] = PlayerPrefs.GetString("ACCOUNT", "No Name");
        RankingClass["score"] = PlayerPrefs.GetInt("SCORE_" + stage_name, 0);
        // データストアへの登録
        RankingClass.SaveAsync((NCMBException e) =>
        {
            if (e == null)
            {
                Display_Ranking(stage_name);
            }
        });
    }

    //ランキング表示機能
    void Display_Ranking(string stage_name)
    {
        //ここでどのランキングを使うか決める
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(stage_name + "RankingClass");
        //Scoreフィールドの降順でデータを取得
        query.OrderByDescending("score");

        //データストアでの検索を行う
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null) return;

            string ranking_name = "";
            string score_string = "";
            before_score = "";
            for (int i = 0; i < objList.Count; i++)
            {
                string name = objList[i]["name"].ToString();
                if (name == PlayerPrefs.GetString("ACCOUNT", "")) ranking_name += "<color=#ffffff>";
                //同じ回数なら順位を同じにする
                if (objList[i]["score"].ToString() == before_score)
                {
                    seq_num++; //最初の1回はこのループに入らないので先に加算
                    ranking_name += (i + 1 - seq_num).ToString();
                }
                else
                {
                    ranking_name += (i + 1).ToString();
                    seq_num = 0;
                }
                ranking_name += ". " + name.Substring(0, name.Length - 4);
                if (name == PlayerPrefs.GetString("ACCOUNT", "")) ranking_name += "</color>";
                if (name == PlayerPrefs.GetString("ACCOUNT", "")) my_rank = i;
                ranking_name += "\n";

                score_string += "                              "; //Tabを8回だけインデント挿入
                if (name == PlayerPrefs.GetString("ACCOUNT", "")) score_string += "<color=#ffffff>";
                score_string += "　Score " + int.Parse(objList[i]["score"].ToString()).ToString();
                if (name == PlayerPrefs.GetString("ACCOUNT", "")) score_string += "</color>";
                score_string += "\n";
                //前のターンの回数を覚えておく
                before_score = objList[i]["score"].ToString();
            }
            ranking_name_text.text = ranking_name;
            score_text.text = score_string;
            all_rank = objList.Count;
        });

    }

    public void Scroll_To_MyName()
    {
        //Debug.Log(1 - my_rank / all_rank);
        scrollrect.verticalNormalizedPosition = 1 - my_rank / all_rank;
    }

}
