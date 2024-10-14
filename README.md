# Unity Molecular Builder🧪⚛🦠
<img src="https://github.com/user-attachments/assets/58983dba-46fb-4e03-8c64-77c438e6ca23" alt="Description of image" width="900" height="200">

**📜This project is a molecule builder tool developed in Unity. It allows users to spawn, bond, and manipulate molecules within a 3D space. The tool supports dynamic molecule manipulation, including moving individual molecules and entire molecular structures, with automatic updates to the molecular formula displayed beneath the molecule group.
It also includes multi-player building for groups to build and learn molecules togther.**


### 🚀  Features

- **Spawn Molecules**  
  Users can place atoms (spheres) in 3D space with a simple click, creating the foundation for molecule building.

- **Bond Creation**  
  Automatically detects when two atoms are close enough and forms a bond (visual connector) between them, mimicking molecular bonds.

- **Molecule Manipulation**  
  - **Individual Movement:** Move individual atoms freely in the 3D space.
  - **Group Movement:** Select an entire molecular structure and move it as a whole, keeping all bonds intact.
  - **Bond Adjustment:** As atoms are moved, bonds automatically adjust their length and angle to remain connected.

- **Dynamic Molecular Formula Display**  
  The molecular formula (e.g., H₂O) is automatically updated and displayed beneath the molecule group based on the connected atoms.

- **Multi-User Collaboration**  
  Supports multiplayer mode, allowing multiple users to collaborate in real-time to build and manipulate molecules together in the same virtual space.

- **Intuitive Controls**  
  Simple, user-friendly controls for building and interacting with molecules:
  - **Right-Click + Drag** to move molecules.
  - **Shift + Click** to manipulate entire molecular structures.
  - **Ctrl + Right-Click** for direct manipulation of individual atoms and connected bonds.

- **VR Compatibility**  
  The tool is VR-compatible, allowing users to interact with the molecules in an immersive virtual environment.


## ⚙️ Installation

To get started with Unity Molecular Builder, follow these steps:

### Prerequisites

Make sure you have the following installed on your system:

- **Unity 2021.3 or later**  
  Download Unity Hub and install the appropriate version from [Unity's official site](https://unity.com/).

- **Git**  
  If you want to clone the project repository, make sure Git is installed. You can download it from [Git's official website](https://git-scm.com/).

### 1. Clone the Repository

To clone the repository, open a terminal and run:

```bash
git clone https://github.com/aseel1/molecule-s-_builder_VR.git
```
### 2. Open the Project in Unity

1. Launch **Unity Hub**.
2. Click on **Open Project** and navigate to the folder where you cloned the repository.
3. Select the folder and wait for Unity to load the project.

### 3. Install Required Unity Packages

The following Unity packages are required to run the project. Make sure they are installed in your project via the **Package Manager**:

- **Input System** (for player controls)
- **Netcode for GameObjects** (for multiplayer functionality)
- **XR Interaction Toolkit** (for VR interaction)
- **And there are other Packages which is required from Unity-Asset Store such as:Lowpoly Enviroment & POLYGON(Payment required or free for students)**

#### Steps to install packages and Assets:

1. In Unity, go to **Window > Package Manager**.
2. In the search bar, look for each package mentiond above.
3. Install each of these packages by clicking **Install**.

**Ensure all assets are correctly imported by doing the following:**

1. Go to Assets > Import Package > Custom Package.
2. Select the MoleculeAssets.unitypackage included in this repository (if provided) or manually add models and prefabs related to molecule building.
3. Make sure the molecule models, bonds, and other prefabs are in the appropriate directories:
- Assets/Models
- Assets/Prefabs

### 4. Project Settings Configuration

Some specific settings need to be configured before running the project:

- **Input System**  
  Enable the new Input System for controls:
  - Go to **Edit > Project Settings > Player**.
  - Under **Other Settings**, locate **Active Input Handling**.
  - Set **Active Input Handling** to **Both** (to enable both old and new Input Systems).

- **XR Settings (for VR users)**  
  If you're using VR, configure XR settings:
  - Go to **Edit > Project Settings > XR Plug-in Management**.
  - Enable **OpenXR** for your target platform (e.g., Windows, Android).
  - **VR Controls Configuration Note**:
     Additional changes are required to configure the VR controls (e.g., movements and interaction).
    These configurations must be done manually at the programmer level within Unity:
    Customize the movement controls and actions for VR as per the project needs.
    Ensure the correct input mappings for VR controllers.

### 5. Build and Play

Now that the project is set up, you can either build the project or run it in Unity Editor:

1. Go to **File > Build Settings**.
2. Select your target platform (e.g., PC, Android).
3. Click **Build and Run** to build and play on the selected platform.

Alternatively, you can press **Play** in the Unity editor to test the project in editor mode.

## Gameplay Tutorial
### launching the game 
- For Solo building Press the Host button
- For Multi-player Building the first player should be the Host. and the others players should be logged in as clients.
<img src="https://github.com/user-attachments/assets/22135530-4cb7-495e-89e4-dab6a5892212" alt="Description of image" width="700" height="400">

### Bulding Molecule
1. The player should navigate to the MoleculeTable and choose the atom.
2. by pressing on the atom the player choose it and can start building them by pressing Left-Clicks.

https://github.com/user-attachments/assets/e5851e6b-e126-4fc6-9335-05120803580c

### Adding Molecules create UI formula automatically 

https://github.com/user-attachments/assets/cf2d7e23-b35b-443a-b596-28ebdd555716

### Moving Molecules using Ctrl+RightClick (single atom) or Shift+RightClick (Whole structre) or deletion Shift+leftClick


https://github.com/user-attachments/assets/e00ae18a-7c23-4498-be79-4bc2f672e9af

### FULL tutoriall.

https://github.com/user-attachments/assets/a3c870ef-5785-4c97-b1c7-989bddafbe45

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Credits
**This Project was done As a semester project for a BSc in Computer science at the University of Haifa, under the supervision of Prof. Roi Poranne**
- **Mr. ASEEL SHAHEEN** - Creator of Unity Molecular Builder
- **Ms. AYLEEN Monayer** - Creator of Unity Molecular Builder
- **Prof. Roi Poranne** - Supervisior and Mentor
## ☎Contact
- Github Profile : https://github.com/aseel1
- 📧Email address  : aseelshaheen1@gmail.com
- linkedIn       :https://www.linkedin.com/in/aseel-shaheen-8983b6279/ 









