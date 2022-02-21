using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Utils
{

    public static string RewardsTableToJson(int[,,] rewardsTable)
    {
        Dictionary<string, string> rewards = new Dictionary<string, string>();
        for (int i = GameManager.instance.leftBoundry; i <= GameManager.instance.rightBoundry; i++)
        {
            for (int j = GameManager.instance.downBoundry; j <= GameManager.instance.upBoundry; j++)
            {
                for (int k = GameManager.instance.backBoundry; k <= GameManager.instance.forwardBoundry; k++)
                {
                    rewards[i.ToString() +"^"+ j.ToString() +"^"+ k.ToString()] = rewardsTable[i, j, k].ToString();

                }
            }

        }
        var x = DictionaryToJson(rewards);
        return DictionaryToJson(rewards);
    }

    public static int[,,] JsonToRewardsTable(string rewards)
    {
        int[,,] rewards_table = new int[2* GameManager.instance.current_player.N, 2 * GameManager.instance.current_player.N, GameManager.instance.current_player.N];
        Dictionary<string, string> rewardDict = JsonToDictionary(rewards);
        foreach (KeyValuePair<string, string> entry in rewardDict)
        {
            var enteries =  entry.Key.Split('^');
            rewards_table[(int)Int32.Parse(enteries[0]), (int)Int32.Parse(enteries[1]), (int)Int32.Parse(enteries[2])] = Int32.Parse(entry.Value);
        }
        return rewards_table;
    }


    public static string LastAppearanceTableToJson(Queue<int>[,,] lastAppearance)
    {
        Dictionary<string, string> lastAppear = new Dictionary<string, string>();
        for (int i = GameManager.instance.leftBoundry; i <= GameManager.instance.rightBoundry; i++)
        {
            for (int j = GameManager.instance.downBoundry; j <= GameManager.instance.upBoundry; j++)
            {
                for (int k = GameManager.instance.backBoundry; k <= GameManager.instance.forwardBoundry; k++)
                {

                    lastAppear[i.ToString() +"^"+ j.ToString() +"^"+ k.ToString()] = "";

                    foreach (int num in lastAppearance[i, j, k])
                    {
                        lastAppear[i.ToString() + "^" + j.ToString() + "^" + k.ToString()] += num.ToString();
                        lastAppear[i.ToString() + "^" + j.ToString() + "^" + k.ToString()] += "_";
                    }
                }
            }
        }

        var x = DictionaryToJson(lastAppear);
        return DictionaryToJson(lastAppear);
    }

    public static Queue<int>[,,] JsonToLastAppearanceTable(string lastAppear)
    {
        Queue<int>[,,] lastAppearance = new Queue<int>[2* GameManager.instance.current_player.N, 2* GameManager.instance.current_player.N, GameManager.instance.current_player.N];
        Dictionary<string, string> lastAppearDict = JsonToDictionary(lastAppear);
        foreach (KeyValuePair<string, string> entry in lastAppearDict)
        {
            var enteries = entry.Key.Split('^');
            int x = (int)Int32.Parse(enteries[0]);
            int y = (int)Int32.Parse(enteries[1]);
            int z = (int)Int32.Parse(enteries[2]);
            lastAppearance[x, y, z] = new Queue<int>();
            var values = entry.Value.Split('_');
            foreach (string val in values)
            {
                if (val != "") { 
                    lastAppearance[x, y, z].Enqueue(Convert.ToInt32(val));
                }
            }
        }

        return lastAppearance;
    }


    public static string DictionaryToJson(Dictionary<string, string> dict)
    {
        var entries = dict.Select(d =>
            string.Format("{0}:{1}", d.Key, string.Join(",", d.Value)));
        return string.Join(",", entries);
    }

    public static Dictionary<string, string> JsonToDictionary(string json)
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        string[] items = json.Split(',');
        foreach (string item in items)
        {
            string[] keyValue = item.Split(':');
            values.Add(keyValue[0], keyValue[1]);
        }
        return values;
    }

    public static string QtableToJson(Dictionary<string, Dictionary<string, double>> qTable)
    {
        string qtable_string = "";
        foreach (KeyValuePair<string, Dictionary<string, double>> entry in qTable)
        {
            foreach (KeyValuePair<string, double> subEntery in entry.Value)
            {
                qtable_string += entry.Key + ":" + subEntery.Key + ":" + subEntery.Value + ",";
            }
        }

        return qtable_string.TrimEnd(',');

    }

    public static Dictionary<string, Dictionary<string, double>> JsonToQtable(string qTableString)
    {
        Dictionary<string, Dictionary<string, double>> q_Table = new Dictionary<string, Dictionary<string, double>>();

        string[] items = qTableString.Split(',');
        foreach (string item in items)
        {
            string[] keyValue = item.Split(':');

            if (q_Table.ContainsKey(keyValue[0]))
            {
                q_Table[keyValue[0]].Add(keyValue[1], double.Parse(keyValue[2]));
            }
            else
            {
                Dictionary<string, double> value = new Dictionary<string, double>();
                value.Add(keyValue[1], double.Parse(keyValue[2]));
                q_Table.Add(keyValue[0], value);
            }
        }
        return q_Table;
    }





}

