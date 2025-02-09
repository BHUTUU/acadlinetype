# AcadLineType

**AcadLineType** is an AutoCAD .NET plugin designed to simplify the creation and manipulation of linetypes. It provides two powerful commands: one for copying existing linetypes (including dash patterns, shapes, and text) and another for reversing the orientation of shapes and text within linetypes. This tool is ideal for CAD professionals who need to manage and customize linetypes efficiently.

---

## Features

### 1. **Copy Linetypes**
   - **Copy Existing Linetypes**: Create a duplicate of any linetype in your drawing, including its dash patterns, shapes, and text.
   - **Custom Naming**: The new linetype is automatically named with a suffix (e.g., `_copyOfOriginalLinetype`) to avoid conflicts.
   - **Preserve Properties**: All properties of the original linetype, such as dash lengths, shapes, and text, are preserved in the new linetype.

### 2. **Reverse Linetype Shapes and Text**
   - **Rotate Shapes and Text**: Rotate shapes and text within a linetype by 180 degrees, effectively reversing their orientation.
   - **Smart Detection**: The plugin detects if a linetype is already reversible and provides appropriate feedback to the user.
   - **Supports Polylines and Lines**: Works with both polylines and lines, ensuring flexibility in your workflow.

---

## Commands

### 1. **CopyLinetypeFromEntity**
   - **Usage**: Copies the linetype of a selected entity and creates a new linetype with a modified name.
   - **Command**: `CopyLinetypeFromEntity`
   - **Steps**:
     1. Run the command.
     2. Select an entity (e.g., a line or polyline) whose linetype you want to copy.
     3. A new linetype is created with the same properties as the original, but with a unique name.

### 2. **Forcereverse**
   - **Usage**: Reverses the orientation of shapes and text within a linetype by rotating them 180 degrees.
   - **Command**: `Forcereverse`
   - **Steps**:
     1. Run the command.
     2. Select a polyline or line whose linetype you want to modify.
     3. The plugin will rotate the shapes and text in the linetype by 180 degrees.

---

## Installation

1. Download the latest release (`AcadLineType.dll`) from the [Releases](https://github.com/BHUTUU/acadlinetype/releases) section.
2. Place the `.dll` file in a directory accessible to AutoCAD.
3. Load the plugin in AutoCAD using the `NETLOAD` command and selecting the `.dll` file.
4. Use the commands `CopyLinetypeFromEntity` and `Forcereverse` to start using the tool.

---

## Usage Examples

### Copying a Linetype
```plaintext
Command: CopyLinetypeFromEntity
Select an entity to copy linetype: (Select an entity)
Linetype 'OriginalLinetype_copyOfOriginalLinetype' successfully created.
```

### Reversing Shapes and Text in a Linetype
```plaintext
Command: Forcereverse
Select a polyline or line: (Select an entity)
Linetype 'OriginalLinetype' shape/text rotated by 180 degrees.
```

---

## Code Overview

The plugin is built using the AutoCAD .NET API and includes the following key components:

1. **CopyLinetypeFromEntity**:
   - Copies an existing linetype, including dash patterns, shapes, and text.
   - Ensures the new linetype has a unique name to avoid conflicts.

2. **Forcereverse**:
   - Reverses the orientation of shapes and text in a linetype by rotating them 180 degrees.
   - Detects if a linetype is already reversible and provides user feedback.

---

## Contributing

Contributions are welcome! If you have suggestions, bug reports, or feature requests, please open an [issue](https://github.com/BHUTUU/acadlinetype/issues) or submit a pull request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/BHUTUU/acadlinetype/blob/main/LICENSE) file for details.

---

## About

**AcadLineType** was developed to streamline the process of managing and customizing linetypes in AutoCAD. Whether you need to create variations of existing linetypes or reverse the orientation of shapes and text, this plugin provides a simple and efficient solution.
