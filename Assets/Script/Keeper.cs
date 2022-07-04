using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper : MonoBehaviour
{
    public static bool is_finished = false;
    [SerializeField] Eat eat;

    void Keeper_Finish()
    {
        is_finished = true;
        eat.Food_Display(eat.food_type);
    }
}
