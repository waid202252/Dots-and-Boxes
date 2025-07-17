# Dots and Boxes Game

**Student:** Yaohui Wang 
**Course:** DI32009  

##  Game Description
A Unity implementation of the classic Dots and Boxes game featuring:
- Two-player local multiplayer
- AI opponent with multiple difficulty levels
- Clean MVC architecture
- Security and ethical considerations

##  Architecture
- **Model:** GameBoard, Player, GameState, Line, Box
- **View:** GameBoardView, UIManager, MainMenuManager
- **Controller:** GameController, InputController, AIController

## AI Implementation
- **Random Strategy:** Basic random move selection
- **Greedy Strategy:** Prioritizes completing boxes and avoiding giving opponents opportunities
- **Minimax Strategy:** Advanced decision making with alpha-beta pruning

## Security Features
- Input validation and sanitization
- Move legality verification
- Game state integrity checks
- Rate limiting for user inputs

## Ethical Considerations
- Accessibility support (color-blind friendly design)
- Fair play enforcement
- Privacy-conscious data handling
- Responsible gaming features

## How to Run
1. Open the project in Unity 2022.3 LTS or later
2. Load the MainMenu scene
3. Build and run, or play in editor

## 📁 Project Structure
Assets/
├── Scenes/          # Game scenes
├── Scripts/
│   ├── Core/        # Game logic
│   ├── UI/          # User interface
│   ├── AI/          # Artificial intelligence
│   └── Utilities/   # Helper classes
└── Prefabs/         # Reusable game objects
