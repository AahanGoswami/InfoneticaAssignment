# 🧠 Configurable Workflow Engine – .NET 8 Minimal API

# Created by Author:Aahan Goswami

A clean, extensible backend service that allows defining, executing, and tracking state-machine-based workflows. Built with **.NET 8** and **Minimal APIs**, this service provides full lifecycle support for workflows including definition, runtime instance management, state transitions, and validation — all in-memory, without a database.

---

## 📌 Project Overview

This project was completed as part of the **Infonetica Software Engineer Take-Home Assignment**.

### 🎯 Objective

Design and implement a backend API that allows:

- Defining a workflow as a **state machine** (states + transitions)
- Starting multiple **workflow instances**
- Executing **actions** to move instances from one state to another
- Viewing **current state and history** of instances

---

## ⚙️ Tech Stack

- **.NET 8**
- **C# 12**
- **Minimal APIs (ASP.NET Core)**
- In-memory data storage (no database)
- Swagger (for API docs)

---

## 🚀 Features

### 🏗️ Workflow Definition

- Define custom workflows with:
  - Named states (initial, final, enabled)
  - Actions (transitions) between states
- Validations:
  - Must have **exactly one initial** state
  - No duplicate IDs
  - Transitions must point to valid and enabled states

---

### 🎬 Workflow Runtime

- Start new instances of any defined workflow
- Each instance:
  - Begins in the initial state
  - Can independently transition between states

---

### 🔁 Action Execution

- Execute transitions on workflow instances via action IDs
- Validations:
  - Action must be defined and enabled
  - Current state must match action’s `fromStates`
  - Target state must exist and be enabled
  - Final states block further transitions

---

### 📜 History Tracking

- Each workflow instance tracks:
  - Action executed
  - Timestamp
- Returned in the `history` array of the instance

---

## ▶️ How to Run

### 🧱 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Postman or `curl` (to test endpoints)

---

### 🏃 Run the App

```command prompt
dotnet run
```
