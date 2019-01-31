<# :********注释区**********
@echo off
more +7 "%~f0" >"%~n0.ps1"
powershell -Mta -NoLogo -NoProfile -ExecutionPolicy bypass -File "%~n0.ps1"
del /f /q "%~n0.ps1"

********注释区**********#>

#************用户设置区**************
$THREADS=10;#最大线程数
$SrcDir='C:\Users\Yi Xiao\Desktop\1';#源目录
$ListFile='keylist.txt';#关键字列表文件,UTF8编码
#************************************

$ScriptBlock={
    Param($file,$arrKeyList)
	
    [system.collections.Arraylist] $result=@();
	$content=[io.file]::ReadAllLines($file);
	foreach($line in $content)
	{
		foreach($elem in $arrKeyList)
		{
			$line=$line -replace $elem[0],$elem[1];
		}
		[void] $result.Add($line);
	}
	[io.file]::WriteAllLines($file,$result);#输出结果
}

#自检
if(!(test-path $SrcDir)) {"目录不存在: `"$SrcDir`"";pause;exit}
if(!(test-path $ListFile)) {"文件不存在: `"$ListFile`"";pause;exit}

$time=Get-Date;
cd $SrcDir;
$Files=dir *.txt -r  -Exclude $ListFile;#扫描文件
[system.collections.Arraylist] $arrKeyList=@();
$KeyList=[io.file]::ReadAllLines($ListFile);#读取列表文件
foreach($line in $KeyList){[void] $arrKeyList.Add($line -split "`t");}

#创建多线程
$RunspacePool = [RunspaceFactory]::CreateRunspacePool(1, $THREADS);
$RunspacePool.Open();
$Jobs = @();

foreach($file in $Files){
    $Job = [powershell]::Create().AddScript($ScriptBlock).AddArgument($file).AddArgument($arrKeyList);
    $Job.RunspacePool = $RunspacePool;
    $Jobs+=New-Object PSObject -Property @{
        Pipe=$Job;
        Result=$Job.BeginInvoke();
    }
}

#等待所在线程结束
$count=$Files.Count;
do{
	$completed=0;
	cls;
	foreach($job in $Jobs)
	{
		if($job.Result.IsCompleted){$job.Pipe.EndInvoke($job.Result); $completed+=1;}
	}
 
	$t="{0:0.00}" -f ((Get-Date)-$time).TotalSeconds
	"进度: $completed/$count	已用时:$($t) 秒"
	start-sleep 1
}while($completed -lt $count)

pause
