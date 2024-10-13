# Unity Molecular Builder
This project is a molecule builder tool developed in Unity. It allows users to spawn, bond, and manipulate molecules within a 3D space. The tool supports dynamic molecule manipulation, including moving individual molecules and entire molecular structures, with automatic updates to the molecular formula displayed beneath the molecule group.

Features
Spawn Elements: Click to spawn hydrogen or oxygen molecules in 3D space.
Bond Molecules: When two molecules are close enough, they automatically bond together with a visual connector.
Move Molecules:
Move Entire Group: Hold Shift + Right Click on a molecule to drag the entire molecular structure.
Move Single Molecule: Hold Ctrl + Right Click to move a single molecule along with its connected bond(s), keeping the bond attached to the other molecule.
Automatic Molecular Formula Generation: The molecular formula (e.g., H₂O) is automatically generated and can be displayed beneath the molecule group based on the bonded structure.
Getting Started
Prerequisites
Unity 2020.3 or later
Basic understanding of Unity's interface and C# scripting
Installation
Clone the Repository:

bash
Copy code
git clone https://github.com/your-username/molecule-builder-unity.git
Open in Unity:

Open Unity Hub.
Click Add and select the cloned project folder.
Setup:

Ensure you have the Unity Input System package installed.
The project should be ready to run as-is.
Usage
Select Element:

Use UI buttons to select either hydrogen (H) or oxygen (O).
Spawn Molecules:

Left-click anywhere in the 3D space to spawn the selected element.
Bond Molecules:

Place molecules close enough to each other, and they will automatically bond.
Move Molecules:

Hold Shift + Right Click on a molecule to move the entire group.
Hold Ctrl + Right Click on a molecule to move only that molecule and its connected bonds.
View Molecular Formula:

The molecular formula is generated automatically based on the bonds formed. You can view it in the console or display it in the UI.
Project Structure
Scripts:
ElementSpawner.cs: Manages element spawning, bonding, and molecule movement.
MoleculeGroup.cs: Handles the grouping and management of bonded molecules.
MoleculeInfo.cs: Stores information about each molecule's type (e.g., "H", "O").
Prefabs:
HydrogenPrefab: The prefab for hydrogen molecules.
OxygenPrefab: The prefab for oxygen molecules.
BondPrefab: The visual connector representing bonds between molecules.
Contributions
Contributions are welcome! Feel free to open issues or submit pull requests to enhance the functionality of this molecule builder.

License
This project is licensed under the MIT License - see the LICENSE file for details
