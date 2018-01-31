for %%f in (Nugets/*.nupkg) do nuget.exe push Nugets/%%f -Source https://api.nuget.org/v3/index.json
rmdir Nugets /s /q
pause