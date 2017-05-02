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
        private static string string_3 = "Chs";
        private static bool flag;

        private static void smethod_1(IDictionary<string, string> idictionary_0, TextAsset textAsset_0, string string_4, bool bool_0, Dictionary<string, string> chsdictionary)
        {
            IDictionary dictionary = JsonMapper.ToObject(textAsset_0.ToString())[string_4];
            IDictionary dictionary2 = (IDictionary)dictionary["Jpn"];
            IDictionary dictionary3 = null;
            if (dictionary.Contains(string_3))
            {
                dictionary3 = (IDictionary)dictionary[string_3];
            }
            else
            {
                dictionary3 = (IDictionary)dictionary["Jpn"];
            }
            foreach (string str3 in dictionary2.Keys)
            {
                string key = !bool_0 ? str3 : str3.Trim();
                string chs = null;
                if (chsdictionary != null && chsdictionary.TryGetValue(key, out chs))
                {
                    idictionary_0.Add(key, chs);
                }
                else
                {
                    string str2 = dictionary3.Contains(str3) ? dictionary3[str3].ToString() : "";
                    if (!string.IsNullOrEmpty(str3) && ((string.IsNullOrEmpty(str2) || ((string_3 == "Eng") && (key.Trim().ToUpper() == str2.Trim().ToUpper()))) || ((string_3 == "Chs") && (key[0] == str2[0]))))
                    {
                        str2 = dictionary2[str3].ToString();
                    }
                    idictionary_0.Add(key, str2);
                }
            }
        }

        public static void Wizard_Master_LoadJsonAndParse(IDictionary<string, string> idictionary_0, string fileName, bool isTrimKey)
        {
            if (!flag)
            {
                flag = true;
                var request = new HTTPRequest(new Uri("https://raw.githubusercontent.com/Leucothea/ShadowverseChineseLocal/master/ver.txt"), onRequestFinished);
                request.Send();
            }
            TextAsset asset = Toolbox.ResourcesManager.LoadObject(fileName) as TextAsset;
            Dictionary<string, string> chsdictionary = null;
            var str = Resource1.ResourceManager.GetObject(asset.name);
            if (str != null)
            {
                var s = (string)str;
                if (CustomPreference._localePref == "Eng")
                {
                    var regex = new Regex(@"\[u\]\[ffcd45\](.*?)\[\-\]\[\/u\]");
                    s = regex.Replace(s, "[ffcd45][b]$1[/b][-]");
                }
                chsdictionary = JsonMapper.ToObject<Dictionary<string, string>>(s);
            }
            smethod_1(idictionary_0, asset, asset.name, isTrimKey, chsdictionary);
        }

        public static void Wizard_SystemText_LoadAndParse(string string_4, Dictionary<string, string> dictionary_0)
        {
            TextAsset asset = Resources.Load("Json/Text/" + string_4) as TextAsset;
            Dictionary<string, string> chsdictionary = null;
            var str = Resource1.ResourceManager.GetObject(string_4);
            if (str != null)
            {
                var s = (string)str;
                chsdictionary = JsonMapper.ToObject<Dictionary<string, string>>(s);
            }
            smethod_1(dictionary_0, asset, string_4, false, chsdictionary);
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
            str = regex1.Replace(str, "用$2或$3赢$1场比赛(");
            Regex regex2 = new Regex("Evolve followers (.+?) times(.+)");
            str = regex2.Replace(str, "进化随从$1次(私人对战除外)");
            Regex regex6 = new Regex(@"Win (.+?) ranked match[e]?[s]? as (.+?)\b");
            str = regex6.Replace(str, "用$2赢$1场天梯");
            Regex regex7 = new Regex(@"Win (.+?) unranked match[e]?[s]? as (.+?)\b");
            str = regex7.Replace(str, "用$2赢$1场自由对战");
            Regex regex8 = new Regex(@"Win (.+?) Take Two match[e]?[s]? as (.+?)\b");
            str = regex8.Replace(str, "用$2赢$1场双选");
            Regex regex9 = new Regex("Win (.+?) ranked matche[e]?[s]?");
            str = regex9.Replace(str, "赢$1场天梯");
            Regex regex10 = new Regex("Win (.+?) unranked matche[e]?[s]?");
            str = regex10.Replace(str, "赢$1场自由对战");
            Regex regex11 = new Regex("Win (.+?) Take Two matche[e]?[s]?");
            str = regex11.Replace(str, "赢$1场双选");
            Regex regex12 = new Regex("Win (.+?) Private [Mm]atche[e]?[s]?");
            str = regex12.Replace(str, "赢$1场私人对战");
            Regex regex4 = new Regex(@"Win (.+?) match[e]?[s]? as (.+?)\b");
            str = regex4.Replace(str, "用$2赢$1场比赛");
            Regex regex5 = new Regex("Win (.+?) matche[e]?[s]?");
            str = regex5.Replace(str, "赢$1场比赛");
            Regex regex3 = new Regex(@"Reach level (.+?) in (.+?)\b");
            str = regex3.Replace(str, "$2等级达到$1");
            Regex regex13 = new Regex("Achieve (.*?) rank");
            str = regex13.Replace(str, "达到$1段位");
            //替换人名
            str = str.Replace("Forestcraft", "精灵");
            str = str.Replace("Swordcraft", "皇室护卫");
            str = str.Replace("Runecraft", "巫师");
            str = str.Replace("Dragoncraft", "龙");
            str = str.Replace("Shadowcraft", "死灵术士");
            str = str.Replace("Bloodcraft", "血族");
            str = str.Replace("Havencraft", "主教");
            //最后替换没什么用的东西
            str = str.Replace("Ranked", "天梯模式");
            str = str.Replace("Unranked", "自由对战");
            str = str.Replace("or ", "或者");
            str = str.Replace("Take Two", "双选模式");
            str = str.Replace("Link Shadowverse with another service", "绑定谷歌或者Facebook");
            str = str.Replace("Clear seven leaders' stories", "通关全部7位英雄的主线剧情");
            return str;
        }

        private static string MyJpnReplace(string str)
        {
            //批量正则
            Regex regex2 = new Regex("(.+?)か(.+?)で(.*?)勝する");
            str = regex2.Replace(str, "用$1或$2赢$3场比赛");
            Regex regex3 = new Regex(@"進化を(.+?)回する\(ルームマッチを除く\)");
            str = regex3.Replace(str, "进化随从$1次(私人对战除外)");
            Regex regex9 = new Regex("ランクマッチで(.+?)勝する");
            str = regex9.Replace(str, "赢$1场天梯");
            Regex regex10 = new Regex("フリーマッチで(.+?)勝する");
            str = regex10.Replace(str, "赢$1场自由对战");
            Regex regex11 = new Regex("2Pickで(.+?)勝する");
            str = regex11.Replace(str, "赢$1场双选");
            Regex regex12 = new Regex("ルームマッチで(.+?)勝する");
            str = regex12.Replace(str, "赢$1场私人对战");
            Regex regex5 = new Regex("バトルで(.+?)勝する");
            str = regex5.Replace(str, "赢$1场比赛");
            Regex regex4 = new Regex("(.+?)で(.+?)勝する");
            str = regex4.Replace(str, "用$1赢$2场比赛");
            Regex regex6 = new Regex("(.+?)のレベルを");
            str = regex6.Replace(str, "$1的等级达到");
            //替换人名
            str = str.Replace("エルフ", "精灵");
            str = str.Replace("ロイヤル", "皇室护卫");
            str = str.Replace("ウィッチ", "巫师");
            str = str.Replace("ドラゴン", "龙");
            str = str.Replace("ネクロマンサー", "死灵术士");
            str = str.Replace("ヴァンパイア", "血族");
            str = str.Replace("ビショップ", "主教");
            //最后替换没什么用的东西
            str = str.Replace("ランクマッチ", "天梯模式");
            str = str.Replace("フリーマッチ", "自由对战");
            str = str.Replace("か", "或者");
            str = str.Replace("2Pick", "双选模式");
            str = str.Replace("アカウント連携をする", "绑定谷歌或者Facebook");
            str = str.Replace("にする", "级");
            str = str.Replace("ランクに", "天梯等级");
            str = str.Replace("到達する", "达成");
            str = str.Replace("７リーダーのストーリーを全てクリアする", "通关全部7位英雄的主线剧情");
            return str;
        }

        static void onRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response != null && new Version("5.5") < new Version(response.DataAsText))
            {
                DialogBase base2 = UIManager.GetInstance().CreateDialogClose();
                base2.SetTitleLabel("汉化有更新啦！");
                base2.SetText($"汉化有更新啦，最新版本{response.DataAsText}\n点击确定跳转网页下载");
                base2.SetButtonLayout(DialogBase.ButtonLayout.BlueBtn_CancelBtn);
                base2.SetButtonText("确定");
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
