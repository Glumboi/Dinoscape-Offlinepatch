using MelonLoader;
using System.Reflection;

[assembly: AssemblyTitle(DinoScapeOffline.BuildInfo.Description)]
[assembly: AssemblyDescription(DinoScapeOffline.BuildInfo.Description)]
[assembly: AssemblyCompany(DinoScapeOffline.BuildInfo.Company)]
[assembly: AssemblyProduct(DinoScapeOffline.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + DinoScapeOffline.BuildInfo.Author)]
[assembly: AssemblyTrademark(DinoScapeOffline.BuildInfo.Company)]
[assembly: AssemblyVersion(DinoScapeOffline.BuildInfo.Version)]
[assembly: AssemblyFileVersion(DinoScapeOffline.BuildInfo.Version)]
[assembly: MelonInfo(typeof(DinoScapeOffline.AddressInjector), DinoScapeOffline.BuildInfo.Name, DinoScapeOffline.BuildInfo.Version, DinoScapeOffline.BuildInfo.Author, DinoScapeOffline.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]