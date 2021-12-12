using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.OdinsHollow;
using System.Reflection;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties

[assembly: AssemblyTitle(Main.Namespace)]
[assembly: AssemblyDescription(Main.Name)]
[assembly: AssemblyConfiguration(AssemblyInfo.Configuration)]
[assembly: AssemblyCompany(AssemblyInfo.Company)]
[assembly: AssemblyProduct(AssemblyInfo.Product)]
[assembly: AssemblyCopyright(AssemblyInfo.Copyright)]
[assembly: AssemblyTrademark(AssemblyInfo.Trademark)]
[assembly: AssemblyCulture(AssemblyInfo.Culture)]
[assembly: ILMerge.ExcludeAssemblies("Digitalroot.Valheim.TrapSpawners")]

// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("b4c0dd05-51f0-4a7a-8c27-51cfb336fb78")]

[assembly: AssemblyVersion(Main.Version)]
[assembly: AssemblyFileVersion(Main.Version)]

