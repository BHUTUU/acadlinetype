import os
import re
autoDeskFolder=r"C:\Program Files\Autodesk"
if not os.path.exists(autoDeskFolder):
    raise RuntimeError("Autocad is not installed on your C drive please change the autoDeskFolder variable in this script accordingly.")
    exit(0)
internalAutoDeskFolder = os.listdir(autoDeskFolder)
versions=set()
C3DVersions = set()
AcadVersions = set()
for fs in internalAutoDeskFolder:
    if fs.startswith("AutoCAD"):
        versionNumber = fs.split()[1]
        versions.add(versionNumber)
        # intenalAutoCADFolder = os.listdir(os.path.join(autoDeskFolder, fs))
        # if "C3D" in list(intenalAutoCADFolder):
        #     C3DVersions.add(versionNumber)
        # if "acad.exe" in list(intenalAutoCADFolder):
        #     AcadVersions.add(versionNumber)
if not versions:
    raise RuntimeError("No Autocad versions found on your C drive. Please check if Autodesk AutoCAD is installed.")
    exit(0)
print("The following versions of autocad are available:")
count=1
for i in versions:
    print(f"{count}. AutoCAD {i}")
    count+=1
selectedIndex=None
while(not selectedIndex):
      try:
            selectedIndex = int(input("Please enter index of your selected version: "))
            if selectedIndex<1 or selectedIndex>count-1:
                print("Please select a valid version index.")
                selectedIndex=None
                continue
      except ValueError:
            print("Invalid input. Please enter a valid number.")
selectedVersion=list(versions)[selectedIndex - 1]
print(f"You have selected the follownig version Autocad {selectedVersion}")
print("Trying to build the project...")
configuration=f"""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net4.8</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="AcDbMgd">
      <HintPath>C:\Program Files\Autodesk\AutoCAD {selectedVersion}\AcDbMgd.dll</HintPath>
    </Reference>
    <Reference Include="AcMgd">
      <HintPath>C:\Program Files\Autodesk\AutoCAD {selectedVersion}\AcMgd.dll</HintPath>
    </Reference>
    <Reference Include="AcCoreMgd">
      <HintPath>C:\Program Files\Autodesk\AutoCAD {selectedVersion}\AcCoreMgd.dll</HintPath>
    </Reference>
  </ItemGroup>

</Project>
"""
with open('AcadLineTypeSolution.csproj', "w") as f:
      f.write(configuration)
print("Project configuration file AcadLineTypeSolution.csproj has been created successfully.")
if os.system("dotnet build -c release") == 0:
      print("Build completed successfully.")
      print(f"The built library can be found at {os.getcwd()}\\bin\\Release\\net4.8\\AcadLineTypeSolution.dll")
else:
      print("Build failed.")
      exit(0)