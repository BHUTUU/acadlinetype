import os

autoDeskFolder = r"C:\Program Files\Autodesk"
if not os.path.exists(autoDeskFolder):
  raise RuntimeError("AutoCAD is not installed on your C drive. Please change the autoDeskFolder variable in this script accordingly.")

internalAutoDeskFolder = os.listdir(autoDeskFolder)
versions = set()

for fs in internalAutoDeskFolder:
  if fs.startswith("AutoCAD"):
    versionNumber = fs.split()[1]
    versions.add(versionNumber)
if not versions:
  raise RuntimeError("No AutoCAD versions found on your C drive. Please check if Autodesk AutoCAD is installed.")

print("\nThe following versions of AutoCAD are available:\n")
for count, version in enumerate(versions, start=1):
  print(f"{count} - AutoCAD {version}")

selectedIndex = None
while selectedIndex is None:
  try:
    selectedIndex = int(input("\nPlease enter the index of your selected version: "))
    if selectedIndex < 1 or selectedIndex > len(versions):
      print("\nPlease select a valid version index.")
      selectedIndex = None
  except ValueError:
    print("\nInvalid input. Please enter a valid number.")

selectedVersion = list(versions)[selectedIndex - 1]
print(f"You have selected the following version: AutoCAD {selectedVersion}")

configuration = f"""<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup>
  <OutputType>Library</OutputType>
  <TargetFramework>net4.8</TargetFramework>
</PropertyGroup>
<ItemGroup>
  <Reference Include="AcDbMgd">
    <HintPath>{os.path.join(autoDeskFolder, f'AutoCAD {selectedVersion}', 'AcDbMgd.dll')}</HintPath>
  </Reference>
  <Reference Include="AcMgd">
    <HintPath>{os.path.join(autoDeskFolder, f'AutoCAD {selectedVersion}', 'AcMgd.dll')}</HintPath>
  </Reference>
  <Reference Include="AcCoreMgd">
    <HintPath>{os.path.join(autoDeskFolder, f'AutoCAD {selectedVersion}', 'AcCoreMgd.dll')}</HintPath>
  </Reference>
</ItemGroup>
</Project>"""

with open('AcadLineTypeSolution.csproj', "w") as f:
  f.write(configuration)

print("Project configuration file AcadLineTypeSolution.csproj has been created successfully.")

if os.system("dotnet build -c release") == 0:
  print("Build completed successfully.")
  print(f"The built library can be found at {os.getcwd()}\\bin\\Release\\net4.8\\AcadLineTypeSolution.dll")
else:
  print("Build Failed. Please install dotnet and build using \"dotnet build -c release\" command.")