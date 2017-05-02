using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace ShadowverseLangPatch
{
    class Program
    {
        static AssemblyDefinition assembly;
        static TypeDefinition mycalss;

        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("暗影之诗 Shadowverse PC+Mac简繁汉化补丁 v5.5");
                Console.WriteLine("汉化：淺夏 <http://steam.pm/id/fylab>");
                Console.WriteLine("      岚兮雨汐 <http://steam.pm/id/momohu>");
                Console.WriteLine("      蔽月八云 <http://steam.pm/id/yakumo17s>");
                Console.WriteLine("      町城安里 <http://steam.pm/id/anrianri>");
                Console.WriteLine("程序：永久告别 <https://www.perfare.net/>");
                Console.WriteLine("--------------------------------------------------------------------");
                if (!File.Exists("Shadowverse.exe"))
                {
                    Console.WriteLine("请将本补丁放在游戏根目录后运行");
                }
                else
                {
                    Console.WriteLine("请选择操作：1. 安装简体(系统默认字体) 2. 安装繁体(日语/英语原版字体) 3.卸载");
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '3')
                    {
                        Directory.SetCurrentDirectory("./Shadowverse_Data/Managed");
                        if (File.Exists("Assembly-CSharp.dll.sv"))
                        {
                            File.Delete("Assembly-CSharp.dll");
                            File.Move("Assembly-CSharp.dll.sv", "Assembly-CSharp.dll");
                            File.Delete("Galstars.Extensions.dll");
                            Console.WriteLine("卸载完成！");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("错误：未找到备份文件，无法卸载，请直接验证完整性！");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                    }
                    else if (key.KeyChar == '1' || key.KeyChar == '2')
                    {
                        Console.WriteLine("应用补丁中，请稍后...");
                        byte[] dll;
                        if (key.KeyChar == '1')//简体
                        {
                            dll = Resource1.Chs;
                        }
                        else if (key.KeyChar == '2')//繁体
                        {
                            dll = Resource1.Cht;
                        }
                        else
                        {
                            return;
                        }
                        assembly = AssemblyDefinition.ReadAssembly("./Shadowverse_Data/Managed/Assembly-CSharp.dll");
                        var hackassembly = ModuleDefinition.ReadModule(new MemoryStream(dll));
                        mycalss = hackassembly.Types.First(x => x.Name == "LanguageHelperChs");
                        if (!Check() &&
                         HookJson("Wizard.Master", "LoadJsonAndParse", "Wizard_Master_LoadJsonAndParse", true) &&
                         HookJson("Wizard.SystemText", "LoadAndParse", "Wizard_SystemText_LoadAndParse", false) &&
                         ChangeFont(key.KeyChar == '1') &&
                         HookLoadObject() &&
                         HookMissionInfoDetail("MissionInfoDetail", "ReadMissionList", "ReadMissionList") &&
                         HookMissionInfoDetail("MissionInfoDetail", "ReadAchievementList", "ReadAchievementList"))
                        {
                            Console.WriteLine("应用补丁成功，备份原文件...");
                            var fileinfo = new FileInfo("./Shadowverse_Data/Managed/Assembly-CSharp.dll");
                            var len = fileinfo.Length;
                            File.Delete("./Shadowverse_Data/Managed/Assembly-CSharp.dll.sv");
                            File.Move("./Shadowverse_Data/Managed/Assembly-CSharp.dll", "./Shadowverse_Data/Managed/Assembly-CSharp.dll.sv");
                            Directory.SetCurrentDirectory("./Shadowverse_Data/Managed");
                            byte[] buff; ;
                            var stream = new MemoryStream();
                            assembly.Write(stream);
                            var buff2 = stream.ToArray();
                            if (buff2.Length < len)
                            {
                                buff = new byte[len];
                                buff2.CopyTo(buff, 0);
                            }
                            else
                            {
                                buff = buff2;
                            }
                            File.WriteAllBytes("Assembly-CSharp.dll", buff);
                            var fileinfo2 = new FileInfo("Assembly-CSharp.dll");
                            fileinfo2.CreationTimeUtc = fileinfo.CreationTimeUtc;
                            fileinfo2.LastAccessTimeUtc = fileinfo.LastAccessTimeUtc;
                            fileinfo2.LastWriteTimeUtc = fileinfo.LastWriteTimeUtc;
                            Console.WriteLine("备份成功");
                            Console.WriteLine("释放汉化文件");
                            File.WriteAllBytes("Galstars.Extensions.dll", dll);
                            Console.WriteLine("所有操作已完成，请进游戏体验");
                            Console.WriteLine("如果发现游戏无法正常运行，请尝试验证完整性后重新运行补丁");
                        }
                        else
                        {
                            Console.WriteLine("哎呀，应用补丁失败...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("哎呀，你没有选对哦，请重启软件再来吧");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("哎呀，程序出错了...");
                Console.WriteLine($"{e.Message}\r\n{e.StackTrace}");
            }
            Console.WriteLine("按任意键退出...");
            Console.ReadKey(true);
        }

        private static bool Check()
        {
            var type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == "Wizard.Master");
            if (type != null)
            {
                var method = type.Methods.FirstOrDefault(o => o.Name == "LoadJsonAndParse");
                if (method != null)
                {
                    if (method.Body.Instructions.Count < 20)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("检测到你已经安装有汉化功能的补丁，请验证完整性后重新安装");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        return true;
                    }
                }
            }
            return false;
        }

        static bool HookJson(string typename, string methodname, string hook, bool b)
        {
            var type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == typename);
            if (type != null)
            {
                var method = type.Methods.FirstOrDefault(o => o.Name == methodname);
                if (method != null)
                {
                    var ilprocessor = method.Body.GetILProcessor();
                    ilprocessor.Body.Instructions.Clear();
                    ilprocessor.Body.ExceptionHandlers.Clear();
                    ilprocessor.Body.Variables.Clear();
                    var mymethod = assembly.MainModule.Import(mycalss.Methods.First(x => x.Name == hook));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ldarg_1));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ldarg_2));
                    if (b)
                        ilprocessor.Append(ilprocessor.Create(OpCodes.Ldarg_3));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Call, mymethod));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ret));
                    return true;
                }
            }
            return false;
        }

        static bool ChangeFont(bool flag)
        {
            if (!flag)
                return true;
            //1
            var type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == "Global");
            if (type != null)
            {
                var method = type.Methods.FirstOrDefault(o => o.Name == ".cctor");
                if (method != null)
                {
                    if (method.Body.Instructions[217].OpCode == OpCodes.Ldstr && (string)method.Body.Instructions[217].Operand == "A-OTF-KaiminTuStd-Bold"
                        && method.Body.Instructions[219].OpCode == OpCodes.Ldstr && (string)method.Body.Instructions[219].Operand == "A-OTF-KaiminTuStd-Bold"
                        )
                    {
                        method.Body.Instructions[217] = Instruction.Create(OpCodes.Ldstr, "TT0818M");
                        method.Body.Instructions[219] = Instruction.Create(OpCodes.Ldstr, "TT0818M");
                        //2
                        type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == "UILabel");
                        if (type != null)
                        {
                            method = type.Methods.FirstOrDefault(o => o.Name == "OnInit");
                            if (method != null)
                            {
                                method.Body.Instructions[9] = Instruction.Create(OpCodes.Ldstr, "Fonts/Jpn/");
                                method.Body.Instructions[10] = Instruction.Create(OpCodes.Ldstr, "TT0818M");
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("警告：字体补丁应用失败");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    return true;
                }
            }
            return false;
        }

        static bool HookLoadObject()
        {
            var type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == "Cute.ResourcesManager");
            if (type != null)
            {
                var method = type.Methods.FirstOrDefault(o => o.FullName == "UnityEngine.Object Cute.ResourcesManager::LoadObject(System.String,System.Boolean,System.Boolean)");
                if (method != null)
                {
                    var ilprocessor = method.Body.GetILProcessor();
                    ilprocessor.Body.Instructions.Clear();
                    ilprocessor.Body.ExceptionHandlers.Clear();
                    ilprocessor.Body.Variables.Clear();
                    var mymethod = assembly.MainModule.Import(mycalss.Methods.First(x => x.Name == "LoadObject"));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ldarg_1));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ldarg_2));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ldc_I4_0));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Call, mymethod));
                    ilprocessor.Append(ilprocessor.Create(OpCodes.Ret));
                    return true;
                }
            }
            return false;
        }

        static bool HookMissionInfoDetail(string typename, string methodname, string hook)
        {
            var type = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == typename);
            if (type != null)
            {
                var method = type.Methods.FirstOrDefault(o => o.Name == methodname);
                if (method != null)
                {
                    var ilprocessor = method.Body.GetILProcessor();
                    var mymethod = assembly.MainModule.Import(mycalss.Methods.First(x => x.Name == hook));
                    var index = ilprocessor.Body.Instructions[0];
                    ilprocessor.InsertBefore(index, ilprocessor.Create(OpCodes.Ldarg_1));
                    ilprocessor.InsertBefore(index, ilprocessor.Create(OpCodes.Call, mymethod));
                    return true;
                }
            }
            return false;
        }
    }
}
