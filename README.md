# Project 3 – Multiplayer Air Hockey

This project is a networked air hockey game using Unity Netcode for GameObjects. It supports variable human player counts and multiplayer play on a single machine (using Unity's Multiplayer Play Mode) or across multiple clients.

## Multiplayer Setup

- **Host**: Starts a new game session, selects match type (1v1, 2v1, 2v2), and claims a paddle slot.
- **Client**: Joins the Host session and claims an available paddle slot.
- **Slot Selection**: Each player must claim one slot; unavailable slots are disabled.
- **Match Configurations**:
  - 1v1 → Left Slot 1 + Right Slot 1
  - 2v1 → Left Slot 1, Left Slot 2 + Right Slot 1
  - 2v2 → All four slots
- **Controls**: 
  - WASD or Arrow keys to move your paddle once a slot is claimed.
  - Number keys (1–4) can also claim slots for testing.

## Game Rules

- The puck bounces off walls and can score in either goal.
- First team to 7 points wins.
- After a goal, the puck resets to the center; the scored-upon team serves next.
- Paddles are confined to their team’s half of the rink.

## Optional AI

- Any unclaimed paddle slots can optionally be filled by AI agents (bonus).
- AI can use Project 2 models to automatically occupy remaining paddles.

## Demo Video

- Show hosting and joining a game.
- Demonstrate match configuration selection.
- Claim paddle slots.
- Show gameplay with paddle movement and scoring.
- Include 1v1 and 2v2 configurations.
