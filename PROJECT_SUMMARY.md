# Dice Party - Project Summary & Developer/Agent Guide

This document provides a comprehensive overview of the **Dice Party** Unity project codebase. It is designed to help developer agents quickly understand the architecture, file layout, and execution flows to begin modifying, debugging, or expanding the game.

---

## 🎮 Project Overview
**Dice Party** is a local multiplayer party board game (similar to *Mario Party*) where players customize their characters in a lobby, roll dice to move along spline paths on a board map, interact with different tile nodes, and compete in various minigames at the end of each round to earn keys/cups and win the game.

---

## 📁 Key Directories & File Layout
All custom gameplay scripts reside in: `Assets/Project/Runtime/Scripts/`

* **`Controller/`**: Handles player inputs and control schemas.
  * [PlayerController.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/Controller/PlayerController.cs): Base controller class.
  * [NewBoardGameController.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/Controller/NewBoardGameController.cs): The state machine controller for moving players on the board map.
  * [MNGPlayerController.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/Controller/MNGPlayerController.cs): Player controller used inside minigames.
* **`BoardNode/`**: Definitions and behaviors for board map tiles.
  * [BoardNode.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/BoardNode/BoardNode.cs): Base node class.
  * Subclasses: `PlusNode`, `TrapNode`, `KeyNode`, `ChestGoldNode`, `ChestNode`, `HealNode`.
* **`Minigame/`**: Core scripts for managing minigames.
  * [WizardMiniGameManager.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/Minigame/WizardMiniGameManager.cs): Core lifecycle manager for all minigames (tutorial, score tracking, timer, victory/ranking).
* **Root Script Folder**:
  * [WizardPartyData.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/WizardPartyData.cs): Persistent data store (`DontDestroyOnLoad`) holding game progress, players' stats (health, keys, cups), and positions.
  * [TurnManager.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/TurnManager.cs): Coordinates players' turns on the board.
  * [Lobby.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/Lobby.cs): Coordinates local player joining and customization.
  * [PlayerCustom.cs](file:///d:/Dice_Party/Assets/Project/Runtime/Scripts/PlayerCustom.cs): Manages customization options (hair, color, body parts).

---

## 🔄 Core Gameplay Loop & Execution Flow

### 1. Lobby Phase (`GameLobby_GetReady.unity`)
1. Players join the game using `Lobby.cs`.
2. Each player can customize their character model (tóc/hairs, màu/colors, trang phục/bodyparts) using `PlayerCustom.cs` which communicates with the persistent `CustomData.cs`.
3. When all players confirm they are ready, `Lobby.cs` loads the board scene (`BoardMap`).

### 2. Board Game Phase (`BoardMap.unity`)
1. **Turn Start**: `TurnManager.cs` instantiates players' custom avatars using `PlayerSetupPosition.cs` and begins the turn loop.
2. **Dice Roll**: The current player rolls a dice. `NewBoardGameController` transitions from `IdleState` to `MovingState`.
3. **Movement**: The character moves along the node splines using `SplineAnimate`. If there is a junction/intersection with multiple pathways:
   * The controller transitions to `ChooseDirectionState`.
   * Directional arrows (`ArrowPointer.cs`) spawn, allowing the player to select their path using input.
4. **Tile Landing (Node Processing)**: Once the player runs out of steps, they land on a `BoardNode`. The node's `ProcessNode()` executes its gameplay effect:
   * Modifies player statistics (e.g., adding/subtracting keys or health in `WizardPartyData`).
   * Triggers the player's `EndTurn()` which advances `TurnManager` to the next player.
5. **Round End & Minigame Transition**: Once every player has finished their turn for the round (`currentPlayerIndex` wraps back to `0`), `TurnManager` starts a 5-second countdown and loads the next minigame scene retrieved from `WizardPartyData.instance.GetMinigame()`.

### 3. Minigame Phase (Various Scenes, e.g., `MNG1` - `MNG10`)
1. **Scene Setup**: `WizardMiniGameManager.cs` initializes HUDs, reads player models from `PlayerManager`, spawns players at minigame spawnpoints using `PlayerSetupPosition.cs`, and switches controllers to `MNGPlayerController`.
2. **Tutorial & Ready Up**: A tutorial panel displays the objective. Every player must press **Confirm** (`Confirm` input action) to ready up.
3. **Gameplay**: Once all players are ready, the game starts. Players compete to earn scores or survive/reach the finish line.
4. **Ranking & Reward**: When the minigame ends (timer runs out or goal is reached), `WizardMiniGameManager.cs` ranks the players, updates their key counts in `WizardPartyData.instance`, plays animations (win/lose), and loads the `BoardMap` scene again.

### 4. Game End
* Landing on `ChestGoldNode` allows players to buy Cups/Stars.
* `WizardPartyData.instance.CheckWin()` checks if any player reached the target cup count (`chestToWin`). If so, they are redirected to the `Win` scene.

---

## 🛠️ Board Movement State Machine
`NewBoardGameController.cs` runs a state machine for board navigation:
1. **`IdleState`**: Player is waiting for their turn to start or wait for input to roll the dice.
2. **`MovingState`**: Player is actively moving along the path. Steps are decremented as nodes are traversed.
3. **`ChooseDirectionState`**: Player reached a node with multiple outgoing connections and must input a direction to choose where to go.
4. **`ItemState`**: Player is using an item from their inventory.
5. **`NodeState`**: Player landed on a node, and the node's special event is playing out.

---

## ➕ How to Add a New Minigame
To implement and queue a new minigame:
1. **Create the Scene**: Create a new scene (e.g., `MNG_NewGame.unity`) under `Assets/Tam/Scenes/MNG/`.
2. **Setup Managers**:
   * Add a `WizardMiniGameManager` component (or a custom sub-class) to the scene.
   * Add a `PlayerSetupPosition` component and link player spawn transforms.
3. **Register Scene**: Add the scene name to the **Build Settings** scene list.
4. **Queue Minigame**: Add the scene name to the `minigames` string list in the `WizardPartyData` component (located in the persistent bootstrapper prefab/scene).
5. **Game Over hook**: Ensure that when the minigame conditions are met, you call `UpdatePlayerCompletedGame(player)` or `UpdatePlayerScore(player, score)` on `WizardMiniGameManager.instance` to trigger the game-over ranking sequence.
