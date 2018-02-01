param($installPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("ModuleInit.cs").Delete()
$project.ProjectItems.Item("App_Start").ProjectItems.Item("UnityWebApiActivator.cs").Delete()

Uninstall-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -ProjectName $project.ProjectName
Uninstall-Package Microsoft.Net.Compilers -ProjectName $project.ProjectName
