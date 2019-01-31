<# :********ע����**********
@echo off
more +7 "%~f0" >"%~n0.ps1"
powershell -Mta -NoLogo -NoProfile -ExecutionPolicy bypass -File "%~n0.ps1"
del /f /q "%~n0.ps1"

********ע����**********#>

#************�û�������**************
$THREADS=10;#����߳���
$SrcDir='C:\Users\Yi Xiao\Desktop\1';#ԴĿ¼
$ListFile='keylist.txt';#�ؼ����б��ļ�,UTF8����
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
	[io.file]::WriteAllLines($file,$result);#������
}

#�Լ�
if(!(test-path $SrcDir)) {"Ŀ¼������: `"$SrcDir`"";pause;exit}
if(!(test-path $ListFile)) {"�ļ�������: `"$ListFile`"";pause;exit}

$time=Get-Date;
cd $SrcDir;
$Files=dir *.txt -r  -Exclude $ListFile;#ɨ���ļ�
[system.collections.Arraylist] $arrKeyList=@();
$KeyList=[io.file]::ReadAllLines($ListFile);#��ȡ�б��ļ�
foreach($line in $KeyList){[void] $arrKeyList.Add($line -split "`t");}

#�������߳�
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

#�ȴ������߳̽���
$count=$Files.Count;
do{
	$completed=0;
	cls;
	foreach($job in $Jobs)
	{
		if($job.Result.IsCompleted){$job.Pipe.EndInvoke($job.Result); $completed+=1;}
	}
 
	$t="{0:0.00}" -f ((Get-Date)-$time).TotalSeconds
	"����: $completed/$count	����ʱ:$($t) ��"
	start-sleep 1
}while($completed -lt $count)

pause
