
Properties {
	$build_dir = Split-Path $psake.build_script_file	
    $nugetPackagesDirectory = Join-Path $build_dir "GeneratedPackages"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Build


Task Build -Depends Clean {	
	
    gci $build_dir  *.sln | 
        ForEach-Object {
            Write-Host "Building Solution $_.FullName" -ForegroundColor Green
            Exec { msbuild $_.FullName /t:Build /p:Configuration=Release /v:quiet } 
    }
}

Task Clean {
     gci $build_dir  *.sln | 
        ForEach-Object {
            Write-Host "Cleaning Solution $_.FullName" -ForegroundColor Green
            Exec { msbuild $_.FullName /t:Clean /p:Configuration=Release /v:quiet } 
    }
}

Task NugetPack -Depends Build {
    if((Test-Path $nugetPackagesDirectory) -eq $false){
        New-Item $nugetPackagesDirectory -type directory
    }
    gci  *.nuspec | 
        ForEach-Object {
            $expression = ".\.nuget\nuget.exe pack {0} -Build -OutputDirectory {1}" -f $_.Name, $nugetPackagesDirectory
            Invoke-Expression $expression
        }
}
Task NugetDeploy -Depends NugetPack {
   
    gci $nugetPackagesDirectory  *.nupkg | 
        ForEach-Object {
            $expression = ".\.nuget\nuget.exe push {0} -ApiKey {1}" -f $_.Name, $nugetApiKey
            Invoke-Expression write-host $expression
        }
}