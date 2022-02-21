using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    public string hand_in_therapy;
    public string id;
    public int N;
    public string first_name;
    public string last_name;
    public float height;
    public float arm_length;
    public double learning_rate;
    public double discount_factor;
    public double random_explore;
    public int bubble_time_out;
    public int treatment_time;
    public string reward_table;
    public string last_appearance;
    public string qtable;
    public int iterations_number;
    public Vector3 lastBubblePos;
    public float prevSessionVelocityAverage;

    public float prevSessionJerkAverage;

    public Player(string hand_in_therapy, string id, string first_name, string last_name, float height, float arm_length, double learning_rate, double discount_factor, double random_explore, int bubble_time_out, int treatment_time, string reward_table, string last_appearance, string qtable, int N, int iterations_number,Vector3 lastBubblePos, float prevSessionVelocityAverage,float prevSessionJerkAverage)
    {
        this.hand_in_therapy = hand_in_therapy;
        this.id = id;
        this.first_name = first_name;
        this.last_name = last_name;
        this.height = height;
        this.arm_length = arm_length;
        this.learning_rate = learning_rate;
        this.discount_factor = discount_factor;
        this.random_explore = random_explore;
        this.bubble_time_out = bubble_time_out;
        this.treatment_time = treatment_time;
        this.reward_table = reward_table;
        this.last_appearance = last_appearance;
        this.qtable = qtable;
        this.N = 6;
        this.iterations_number = iterations_number;
        this.lastBubblePos = lastBubblePos;
        this.prevSessionVelocityAverage = prevSessionVelocityAverage;
        this.prevSessionJerkAverage =prevSessionJerkAverage;
    }

    public Player(Player previousPlayer)
    {
        this.hand_in_therapy = previousPlayer.hand_in_therapy;
        this.id = previousPlayer.id;
        this.first_name = previousPlayer.first_name;
        this.last_name = previousPlayer.last_name;
        this.height = previousPlayer.height;
        this.arm_length = previousPlayer.arm_length;
        this.learning_rate = previousPlayer.learning_rate;
        this.discount_factor = previousPlayer.discount_factor;
        this.random_explore = previousPlayer.random_explore;
        this.bubble_time_out = previousPlayer.bubble_time_out;
        this.treatment_time = previousPlayer.treatment_time;
        this.reward_table = previousPlayer.reward_table;
        this.last_appearance = previousPlayer.last_appearance;
        this.qtable = previousPlayer.qtable;
        this.N = previousPlayer.N;
        this.iterations_number = previousPlayer.iterations_number;
        this.lastBubblePos = previousPlayer.lastBubblePos;
        this.prevSessionVelocityAverage = previousPlayer.prevSessionVelocityAverage;
        this.prevSessionJerkAverage =previousPlayer.prevSessionJerkAverage;
    }
}


