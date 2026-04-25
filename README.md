# Total Tales Tussle 🏰

> Solo indie dev | Unity RTS | Work in Progress

Source code for **Total Tales Tussle** — a real-time strategy game inspired by Age of Empires, built solo in Unity. I'm sharing this as I go: documenting the architecture, posting breakdowns on social media, and learning in public.

---

## ⚠️ Disclaimer

This is **indie dev source code, not a reference codebase.**

I'm a solo developer handling everything — programming, art, animation, and design. The code reflects real-world solo dev decisions: pragmatic choices, mid-refactor states, and things I'd do differently next time. If you're here to learn or get inspired, great. If you're here for textbook-perfect code, this isn't that — and that's okay.

---

## 🎮 What is this game?

An RTS with fairy tale aesthetics. Units gather resources, construct buildings, and fight — all driven by a custom hierarchical state machine system built from scratch in Unity.

Core inspirations: **Age of Empires**, classic RTS design.

---

## 🗂️ Project Structure

```
Scripts/
├── CoreStateMachine/       # Hierarchical state machine (Major + Minor states)
├── Entity/
│   ├── Unit/               # Unit logic: controller, movement, health, AI states
│   ├── Building/           # Buildings and construction
│   ├── Resource/           # Resource nodes and gathering
│   └── Interface/          # Shared interfaces (ISelectable, IDamageable, etc.)
├── Player/                 # Player input, selection, construction flow
├── Manager/                # ResourceManager, BuildingManager, Team system
├── UI/                     # HUD, selection box, ability panel, resource display
├── Camera/                 # Camera controller
└── Shared/                 # Utilities: Singleton, Timer
```

---

## 🧠 Architecture Highlights

### Hierarchical State Machine
Every entity (units, player) runs on a custom `StateMachine` with **Major** and **Minor** states. Major states handle broad behavior (Idle, Moving, Collecting, Combat), Minor states handle sub-behavior within those.

### Unit Architecture
Each unit is built from modular components owned and distributed by `UnitController`:
- `UnitOrderHandler` — routes incoming orders to the right state transition
- `UnitActionQueue` — queues and processes unit intentions
- `UnitStateMachine` — executes behavior via named transition methods
- `UnitSensor`, `UnitMovement`, `UnitHealth` — dedicated component responsibilities

### Team System
Ownership and allegiance managed through a `Team` enum — used for selection filtering, targeting, and building ownership.

### ScriptableObject Data
Units, buildings, resources, and abilities are all data-driven via ScriptableObjects (`UnitSetting`, `BuildingSO`, `ResourceNodeSO`, `AbilityBaseSO`).

---

## 🔧 Tech

- **Engine:** Unity (2D/3D hybrid RTS)
- **Language:** C#
- **Inspector:** Odin Inspector
- **Version Control:** Git / GitHub

---
## ⚠️ Dependencies & Missing Assets

This repo contains **Scripts only**. To run this project you will need the following paid/third-party assets — they are **not included**:

- **Odin Inspector** — serialization and custom inspector tools
- **DOTween (HOTween)** — animation and tweening
- **UniTask** — async/await utilities for Unity
- **Hot Reload** — editor-only live reload tool, not related to gameplay code

Without these, the project will throw errors. This is a source code share for learning and reference, not a plug-and-play project.
---
## 📢 Follow the Development

I post architecture breakdowns, dev logs, and progress on social media as I build this. If you want to follow along or ask questions about any system, feel free.

---
## 📄 License

Free to use, copy, or learn from. No credit required, but always appreciated. 🙏
