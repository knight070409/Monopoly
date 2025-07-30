# Monopoly-Unity-Game-Development-Assignment
A 2D Monopoly-style board game inspired by 'Bankroll' from the Plato platform. Designed for a 2-player local multiplayer experience, this project features dice rolls, property acquisition, rent mechanics, chance events, jail system, and a full user interface with live transaction logs.

---

##  Project Summary

This project was developed for the **Unity Game Developer Assignment**. It replicates core Monopoly-style gameplay mechanics with a clean UI and object-oriented structure, entirely in Unity using C#.

---

## 🧱 Game Architecture Overview

This Monopoly-style board game is built in **Unity 6** using **C#**, following clean architecture principles with design patterns like **Singleton** and **Observer** for maintainability and scalability.

---

### 📂 Main Systems Overview

#### 1. Game Manager (Singleton)
- Controls overall game flow: turn management, dice rolling, player switching.
- Emits game events using the **Observer pattern** (`OnTurnChanged`, `OnDiceRolled`).
- Handles rules such as:
  - Jail turns
  - End of turn logic

#### 2. Player
- Handles:
  - Movement logic using `IEnumerator` to animate player steps
  - Money adjustments (rent, rewards, buying)
  - Jail state and turns
  - Overlapping logic when two players are on the same tile
  - Owns a list of purchased properties

#### 3. Tile System
- `Tile.cs` holds:
  - Tile type: Property, Start, Jail, GoToJail, Chance, etc.
  - Owner ID, cost, rent
- `TileType` enum defines the board logic

#### 4. UI Manager (Singleton)
- Reacts to game events (observer) and updates:
  - Current turn indicators
  - Player info (money, property count)
  - Transaction log
  - Buy/Pass popup panel
  - Property detail panel
  - Handles user input for buying/passing properties

#### 5. Dice System
- Animates and rolls dice via coroutine
- Communicates result back to GameManager

#### 6. Event System (Observer Pattern)
- Allows decoupled communication between systems
- UI listens to `GameManager` events to update turn UI, logs, and panels

---

### 🧩 Design Patterns Used

| Pattern    | Used For                         |
|------------|----------------------------------|
| Singleton  | `GameManager`, `UIManager`       |
| Observer   | `OnTurnChanged`, `OnDiceRolled`  |
| Coroutine  | Player movement & animations     |

---

### ⚙️ Key Features Implemented

- Turn-based gameplay with 2 players
- Buying and owning properties
- Paying rent (with jail exceptions)
- Jail system with 3 missed turns
- Chance tile with probabilistic reward/penalty
- Passing "Start" gives $200
- UI panels for property and player info
- Game log of actions

---

## 🧠 Future Improvements

-  Property trading
-  Online multiplayer or AI
-  Smarter turn options
-  Multilingual support

---

## 🏁 Credits

**Developer:** Yash Pal

**Assignment:** Unity Game Developer Assignment  

**Tools:** Unity 6, C#, Canva, Krita

---
