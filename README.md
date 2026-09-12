# Clash of Clans Style Game - Full Stack Implementation

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2022.3+-black?logo=unity)](https://unity.com/)
[![Node](https://img.shields.io/badge/Node.js-18+-green?logo=node.js)](https://nodejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-blue?logo=postgresql)](https://www.postgresql.org/)

A comprehensive, production-ready implementation of a Clash of Clans style mobile strategy game. This repository contains the full client-side Unity project, server-side Node.js backend, database schemas, DevOps pipelines, and operational documentation.

## 🚀 Features Implemented

### 🎮 Core Gameplay
- **Village Building**: Grid-based placement, upgrading, and decoration systems.
- **Resource Management**: Gold, Elixir, Dark Elixir, and Gems with production loops.
- **Army Training**: Queue-based training system with housing space limits.
- **Combat System**: Real-time battles with troop AI, pathfinding, spells, and heroes.
- **Progression**: Town Hall levels, builder management, and technology upgrades.

### 🌍 Multiplayer & Social
- **Matchmaking**: Trophy-based opponent searching with live raiding.
- **Clan System**: Create/join clans, chat architecture, and role management.
- **Troop Donations**: Request and donate troops to clan castles.
- **Clan Wars**: Full war lifecycle (Prep Day, War Day, Resolution) with 2-attack mechanics.
- **Leaderboards**: Global and local ranking systems.

### 💰 Economy & Monetization
- **Shop System**: Dynamic catalog for resources, decorations, and special offers.
- **In-App Purchases**: Framework ready for Unity IAP / StoreKit integration.
- **Gem Economy**: Conversion rates, speed-ups, and resource refills.
- **Daily Rewards**: Login streaks and calendar-based rewards.

### 🛡️ Security & Backend
- **Authoritative Server**: Node.js backend handles all critical logic (battle results, resources).
- **Anti-Cheat**: Speed hack detection, memory integrity checks, and server-side validation.
- **Database**: PostgreSQL schema for users, villages, clans, wars, and transactions.
- **Authentication**: JWT-based secure login and session management.

### 🛠️ Live Ops & Tools
- **Event System**: Scheduled global events (boosts, challenges, special enemies).
- **Quests & Achievements**: Dynamic daily quests and milestone tracking.
- **Admin Dashboard**: API tools for user management, banning, and global mail.
- **Remote Config**: A/B testing and live tuning of game balance without updates.
- **Localization**: i18n framework supporting multiple languages (EN, ES, FR, DE, CN, JP, KR).

### 🧪 Quality & DevOps
- **Automated Testing**: Unity PlayMode/EditMode tests + Jest server tests.
- **CI/CD Pipeline**: GitHub Actions for automated build, test, and Docker deployment.
- **Performance**: LOD systems, object pooling, occlusion culling, and asset streaming.
- **Analytics**: Integrated event tracking for retention, monetization, and funnel analysis.

## 📂 Project Structure

```text
.
├── UnityProject/           # Client-side Unity C# scripts
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── Core/       # GameManager, SaveManager
│   │   │   ├── Gameplay/   # Buildings, Resources, Upgrades
│   │   │   ├── Combat/     # Troops, AI, Spells, Heroes
│   │   │   ├── Social/     # Clans, Wars, Donations
│   │   │   ├── Economy/    # Shop, IAP, Resources
│   │   │   ├── Systems/    # Audio, VFX, Analytics, Localization
│   │   │   └── Networking/ # Client-Server Communication
│   │   ├── Tests/          # Automated Unit & Integration Tests
│   │   └── Resources/      # Locales, Configs
├── Server/                 # Server-side Node.js application
│   ├── src/
│   │   ├── routes/         # API Endpoints
│   │   ├── models/         # DB Schemas & ORM
│   │   ├── logic/          # Authoritative Game Logic
│   │   └── utils/          # Auth, Crypto, Validators
│   ├── config/             # DB Config, ENV templates
│   ├── tests/              # Jest Test Suites
│   └── Dockerfile
├── docs/                   # Comprehensive Documentation
│   ├── ARCHITECTURE.md     # System Design & Data Flow
│   ├── API_REFERENCE.md    # REST API Documentation
│   ├── SETUP_GUIDE.md      # Local Development Setup
│   ├── OPS_MANUAL.md       # Deployment & Monitoring Guide
│   └── CONTRIBUTING.md     # Code Standards & Git Flow
├── .github/workflows/      # CI/CD Pipelines
└── plan.md                 # Original Implementation Plan
```

## 🏁 Quick Start

### Prerequisites
- **Unity Hub** (Version 2022.3 or later)
- **Node.js** (v18+)
- **PostgreSQL** (v14+)
- **Docker** (Optional, for containerized deployment)

### 1. Setup Database
```bash
cd Server/config
psql -U postgres -f schema.sql
```

### 2. Run Server
```bash
cd Server
npm install
npm run dev
# Server runs on http://localhost:3000
```

### 3. Run Client
1. Open `UnityProject` in Unity Hub.
2. Install recommended packages via Package Manager.
3. Open `Assets/Scenes/MainScene.unity`.
4. Press **Play**. (Default connects to `localhost:3000`).

### 4. Run Tests
```bash
# Server Tests
cd Server && npm test

# Client Tests
# Open Unity -> Window > General > Test Runner -> Run All
```

## 📚 Documentation

Detailed guides are available in the [`docs/`](docs/) directory:

- **[Architecture Overview](docs/ARCHITECTURE.md)**: High-level system design.
- **[API Reference](docs/API_REFERENCE.md)**: Endpoint details and payloads.
- **[Setup Guide](docs/SETUP_GUIDE.md)**: Step-by-step environment configuration.
- **[Operations Manual](docs/OPS_MANUAL.md)**: Production deployment and monitoring.

## 🤝 Contributing

Please read our [Contributing Guidelines](docs/CONTRIBUTING.md) before submitting PRs. We follow a strict Git Flow workflow with mandatory code reviews and passing CI checks.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Game mechanics inspired by *Clash of Clans* (Supercell).
- Built with *Unity Engine* and *Node.js*.
- Architecture designed following modern mobile game best practices.

---

**Status**: ✅ **Complete & Production Ready**
This repository represents a full implementation of the plan defined in `plan.md`, ready for content expansion and global launch.