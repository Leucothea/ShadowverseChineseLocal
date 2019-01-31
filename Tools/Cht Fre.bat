/*
@echo off&&cls
for %%i in (*.txt) do (
    echo %%i
    cscript //NOLOGO /E:javascript "%~f0" "%%~fi"||Rem TEST.txt是要处理的文件
)
pause
exit
*/
function ReadFile (Sourcefile, CharSet) {
    var Str
    var stm = new ActiveXObject("Adodb.Stream")
    stm.Type = 2
    stm.mode = 3
    stm.charset = CharSet
    stm.Open()
    stm.loadfromfile(Sourcefile)
    Str = stm.readtext
    stm.Close()
    var stm = null;
    return Str;
}
function WriteToFile (Getfile, Str, CharSet) {
    var stm = new ActiveXObject("Adodb.Stream")
    stm.Type = 2
    stm.mode = 3
    stm.charset = CharSet
    stm.Open()
    stm.WriteText(Str)
    stm.SaveToFile(Getfile,2)
    stm.flush()
    stm.Close()
    var stm = null
}
WriteToFile(WScript.Arguments(0),ReadFile(WScript.Arguments(0),"UTF-8").match(/[\s\S.]+[\r\n](.*\"Cht\"[\s\S.]+)/)[1].match(/([\S\s]+)[\r\n].*\"Fre\"[\S\s]*/)[1],"UTF-8")