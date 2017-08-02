using BestHTTP;
using Cute;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Galstars.Extensions
{
    public class LanguageHelperChs
    {
        private static string STRING_CHS = "Chs";
        private static bool flag;

        private static void smethod_1(IDictionary<string, string> target_dic, TextAsset orig_textasset, string textasset_name, bool isTrim, Dictionary<string, string> chs_dic)
        {
            IDictionary orig_dic = JsonMapper.ToObject(orig_textasset.ToString())[textasset_name];
            IDictionary jp_dic = (IDictionary)orig_dic["Jpn"];
            IDictionary sub_dic = null;
            if (orig_dic.Contains(STRING_CHS))
            {
                sub_dic = (IDictionary)orig_dic[STRING_CHS];
            }
            else
            {
                sub_dic = (IDictionary)orig_dic["Jpn"];
            }
            Dictionary<string, string> tmp_dic = new Dictionary<string, string>();
            foreach (string key in target_dic.Keys)
            {
                string chs = null;
                if (chs_dic != null && chs_dic.TryGetValue(key, out chs))
                {
                    tmp_dic.Add(key, chs);
                }
                else
                {
                    string str2 = sub_dic.Contains(key) ? sub_dic[key].ToString() : target_dic[key];
                    if (!string.IsNullOrEmpty(key) && ((string.IsNullOrEmpty(str2) || ((STRING_CHS == "Eng") && (key.Trim().ToUpper() == str2.Trim().ToUpper()))) || ((STRING_CHS == "Chs") && (key[0] == str2[0]))))
                    {
                        str2 = jp_dic[key].ToString();
                    }
                    tmp_dic.Add(key, str2);
                }
            }
            foreach (var item in tmp_dic)
            {
                target_dic[item.Key] = tmp_dic[item.Key];
            }
        }

        public static void Wizard_Master_LoadJsonAndParse(IDictionary<string, string> target_dic, string fileName, bool isTrimKey)
        {
            if (!flag)
            {
                flag = true;
                var request = new HTTPRequest(new Uri("https://raw.githubusercontent.com/Leucothea/ShadowverseChineseLocal/master/ver.txt"), onRequestFinished);
                request.Send();
            }
            TextAsset asset = Toolbox.ResourcesManager.LoadObject(fileName) as TextAsset;
            Dictionary<string, string> chs_dic = null;
            var str = Resource1.ResourceManager.GetObject(asset.name);
            if (str != null)
            {
                var s = (string)str;
                if (CustomPreference._localePref == "Eng")
                {
                    var regex = new Regex(@"\[u\]\[ffcd45\](.*?)\[\-\]\[\/u\]");
                    s = regex.Replace(s, "[ffcd45][b]$1[/b][-]");
                }
                chs_dic = JsonMapper.ToObject<Dictionary<string, string>>(s);
            }
            smethod_1(target_dic, asset, asset.name, isTrimKey, chs_dic);
        }

        public static void Wizard_SystemText_LoadAndParse(string string_4, Dictionary<string, string> target_dic)
        {
            TextAsset asset = Resources.Load("Json/Text/" + string_4) as TextAsset;
            Dictionary<string, string> chs_dic = null;
            var str = Resource1.ResourceManager.GetObject(string_4);
            if (str != null)
            {
                var s = (string)str;
                chs_dic = JsonMapper.ToObject<Dictionary<string, string>>(s);
            }
            //smethod_1(dictionary_0, asset, string_4, false, chsdictionary);
            Dictionary<string, string> tmp_dic = new Dictionary<string, string>();
            foreach (string key in target_dic.Keys)
            {
                string chs = null;
                if (chs_dic != null && chs_dic.TryGetValue(key, out chs))
                {
                    tmp_dic.Add(key, chs);
                }
                else
                {
                    tmp_dic.Add(key, target_dic[key]);
                }
            }
            foreach (var item in tmp_dic)
            {
                target_dic[item.Key] = tmp_dic[item.Key];
            }
        }

        public static UnityEngine.Object LoadObject(string objectName, bool isServerResources = true, bool isIfFindLoad = false)
        {
            if (objectName.Contains("scenario_text_"))
            {
                int index = objectName.LastIndexOf("/");
                var str = objectName.Substring(index + 1);
                var t = Resource2.ResourceManager.GetObject(str);
                if (t != null)
                {
                    return new MyTextAss((string)t);
                }
            }
            return Toolbox.ResourcesManager.LoadObject<UnityEngine.Object>(objectName, isServerResources);
        }

        public static void ReadMissionList(JsonData userMissionList)
        {
            for (int i = 0; i < userMissionList.Count; i++)
            {
                JsonData data = userMissionList[i];
                var str = (string)data["mission_name"];
                if (CustomPreference._localePref == "Eng")
                {
                    str = MyEngReplace(str);
                }
                else
                {
                    str = MyJpnReplace(str);
                }
                data["mission_name"] = str;
            }
        }

        public static void ReadAchievementList(JsonData userAchievementList)
        {
            for (int i = 0; i < userAchievementList.Count; i++)
            {
                JsonData data = userAchievementList[i];
                var str = (string)data["achievement_name"];
                if (CustomPreference._localePref == "Eng")
                {
                    str = MyEngReplace(str);
                }
                else
                {
                    str = MyJpnReplace(str);
                }
                data["achievement_name"] = str;
            }
        }

        private static string MyEngReplace(string str)
        {
            //批量正则
            Regex regex1 = new Regex(@"Win (.+?) match[e]?[s]? as (.+?) or (.+?) \(");
            str = regex1.Replace(str, "用$2或$3贏$1場比賽(");
            Regex regex2 = new Regex("Evolve followers (.+?) times(.+)");
            str = regex2.Replace(str, "進化隨從$1次(私人對戰除外)");
            Regex regex6 = new Regex(@"Win (.+?) ranked match[e]?[s]? as (.+?)\b");
            str = regex6.Replace(str, "用$2贏$1場天梯");
            Regex regex7 = new Regex(@"Win (.+?) unranked match[e]?[s]? as (.+?)\b");
            str = regex7.Replace(str, "用$2贏$1場自由對戰");
            Regex regex8 = new Regex(@"Win (.+?) Take Two match[e]?[s]? as (.+?)\b");
            str = regex8.Replace(str, "用$2贏$1場雙選");
            Regex regex9 = new Regex("Win (.+?) ranked matche[e]?[s]?");
            str = regex9.Replace(str, "贏$1場天梯");
            Regex regex10 = new Regex("Win (.+?) unranked matche[e]?[s]?");
            str = regex10.Replace(str, "贏$1場自由對戰");
            Regex regex11 = new Regex("Win all 5 Take Two matches (.+?) times");
            str = regex11.Replace(str, "5戰全勝$1次");
            Regex regex12 = new Regex("Win all (.+?) Private [Mm]atche[e]?[s]?");
            str = regex12.Replace(str, "贏$1場私人對戰");
            Regex regex4 = new Regex(@"Win (.+?) match[e]?[s]? as (.+?)\b");
            str = regex4.Replace(str, "用$2贏$1場比賽");
            Regex regex5 = new Regex("Win (.+?) Take Two matche[e]?[s]?");
            str = regex5.Replace(str, "贏$1場雙選模式比賽");
            Regex regex3 = new Regex(@"Reach level (.+?) in (.+?)\b");
            str = regex3.Replace(str, "$2等級達到$1");
            Regex regex13 = new Regex("Achieve (.*?) rank");
            str = regex13.Replace(str, "達到$1天梯等級");
            Regex regex14 = new Regex("Defeat (.*?) on Elite difficulty.+?Practice.+");
            str = regex14.Replace(str, "練習模式：戰勝$1的超級難度");
            Regex regex15 = new Regex("Defeat (.*?) on Elite 2 difficulty.+?Practice.+");
            str = regex15.Replace(str, "練習模式：戰勝$1的超級難度(2)");
            //替換人名
            str = str.Replace("Forestcraft", "精靈");
            str = str.Replace("Swordcraft", "皇室護衛");
            str = str.Replace("Runecraft", "巫師");
            str = str.Replace("Dragoncraft", "竜");
            str = str.Replace("Shadowcraft", "死靈術士");
            str = str.Replace("Bloodcraft", "吸血鬼");
            str = str.Replace("Havencraft", "主教");
            str = str.Replace("Arisa", "亞里莎");
            str = str.Replace("Erika", "艾莉華");
            str = str.Replace("Isabelle", "伊莎貝爾");
            str = str.Replace("Rowen", "羅文");
            str = str.Replace("Luna", "露娜");
            str = str.Replace("Urias", "尤里亞斯");
            str = str.Replace("Eris", "伊莉絲");
            str = str.Replace("Clear 7 leaders' stories up to Chapter 8", "完成全部7个英雄的故事模式第8章");
            str = str.Replace("Battle 20 players in Private Match", "和20個不同的玩家完成對戰");
            str = str.Replace("without quitting", "不可以棄權結束對戰");
            //最後替換沒什麼用的東西
            str = str.Replace("Ranked", "天梯模式");
            str = str.Replace("Unranked", "自由對戰");
            str = str.Replace("or ", "或者");
            str = str.Replace("Take Two", "雙選模式");
            str = str.Replace("Link Shadowverse with another service", "連攜谷歌或者臉書");
            str = str.Replace("Clear seven leaders' stories", "通關全部7位英雄的主線劇情");
            return str;
        }

        private static string MyJpnReplace(string str)
        {
            //批量正则
            Regex regex2 = new Regex("(.+?)か(.+?)で(.*?)勝する");
            str = regex2.Replace(str, "用$1或$2贏$3場比賽");
            Regex regex3 = new Regex(@"進化を(.+?)回する\(ルームマッチを除く\)");
            str = regex3.Replace(str, "進化隨從$1次(私人對戰除外)");
            Regex regex9 = new Regex("ランクマッチで(.+?)勝する");
            str = regex9.Replace(str, "贏$1場天梯");
            Regex regex10 = new Regex("フリーマッチで(.+?)勝する");
            str = regex10.Replace(str, "贏$1場自由對戰");
            Regex regex11 = new Regex("2Pickで(.+?)勝する");
            str = regex11.Replace(str, "贏$1場雙選");
            Regex regex12 = new Regex("ルームマッチで(.+?)勝する");
            str = regex12.Replace(str, "贏$1場私人對戰");
            Regex regex5 = new Regex("バトルで(.+?)勝する");
            str = regex5.Replace(str, "贏$1場比賽");
            Regex regex4 = new Regex("(.+?)で(.+?)勝する");
            str = regex4.Replace(str, "用$1贏$2場比賽");
            Regex regex6 = new Regex("(.+?)のレベルを");
            str = regex6.Replace(str, "$1的等級達到");
            Regex regex7 = new Regex("プラクティス：(.+?)");
            str = regex7.Replace(str, "練習模式：戰勝$1");
            Regex regex8 = new Regex("5戦全勝を(.+?)回する");
            str = regex8.Replace(str, "5戰全勝$1次");
            //替換人名
            str = str.Replace("エルフ", "精靈");
            str = str.Replace("ロイヤル", "皇室護衛");
            str = str.Replace("ウィッチ", "巫師");
            str = str.Replace("ドラゴン", "竜");
            str = str.Replace("ネクロマンサー", "死靈術士");
            str = str.Replace("ヴァンパイア", "吸血鬼");
            str = str.Replace("ビショップ", "主教");

            str = str.Replace("アリサ", "亞里莎");
            str = str.Replace("エリカ", "艾莉華");
            str = str.Replace("イザベル", "伊莎貝爾");
            str = str.Replace("ローウェン", "羅文");
            str = str.Replace("ルナ", "露娜");
            str = str.Replace("ユリアス", "尤里亞斯");
            str = str.Replace("イリス", "伊莉絲");
            str = str.Replace("プラクティス", "練習模式");
            str = str.Replace("ルームマッチ", "私人對戰");
            str = str.Replace("の超級に勝利する", "的超級難度");
            str = str.Replace("の超級2に勝利する", "的超級難度(2)");
            str = str.Replace("20人と対戦する", "和20個不同的玩家完成對戰");
            str = str.Replace("お互いリタイアせず対戦を終える", "不可以棄權結束對戰");
            str = str.Replace("7リーダーのストーリーを8章までクリアする", "完成全部7个英雄的故事模式第8章");
            //最後替換沒什麼用的東西
            str = str.Replace("ランクマッチ", "天梯模式");
            str = str.Replace("フリーマッチ", "自由模式");
            str = str.Replace("か", "或者");
            str = str.Replace("2Pick", "雙選模式");
            str = str.Replace("アカウント連携をする", "連攜谷歌或者臉書");
            str = str.Replace("にする", "級");
            str = str.Replace("ランクに", "天梯等級");
            str = str.Replace("到達する", "達成");
            str = str.Replace("７リーダーのストーリーを全てクリアする", "通關全部7位英雄的主線劇情");
            str = str.Replace("で", "");
            return str;
        }

        static void onRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response != null && new Version("6.1.0") < new Version(response.DataAsText))
            {
                DialogBase base2 = UIManager.GetInstance().CreateDialogClose();
                base2.SetTitleLabel("漢化有更新了！");
                base2.SetText($"漢化有更新了，最新版本{response.DataAsText}\n點撃確定跳轉網頁下載");
                base2.SetButtonLayout(DialogBase.ButtonLayout.BlueBtn_CancelBtn);
                base2.SetButtonText("確定");
                base2.SetPanelDepth(0x7d0);
                base2.onPushButton1 = () => { Application.OpenURL("http://sennatsu.com/"); };
            }
        }
    }

    public class MyTextAss : TextAsset
    {
        string mytext;

        public MyTextAss(string str)
        {
            mytext = str;
        }

        public override string ToString()
        {
            return mytext;
        }
    }
}
