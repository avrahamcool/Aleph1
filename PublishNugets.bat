for /r %%f in (bin\Release\*.nupkg) do (
	nuget.exe push %%f -Source https://api.nuget.org/v3/index.json
	DEL %%f
)
pause