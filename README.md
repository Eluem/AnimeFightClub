# AnimeFightClub

- AnimeFightClub is an online multiplayer 2d side scrolling action combat game
- The project was built by two programmers and two artists as a senior college project in 2012
- I was the lead programmer and created the networking architecture, a simple lightweight access database to store user info on the master server, and wrote a simple 2d physics engine
- We used the XNA framework to handle drawing to the screen and getting controller inputs. The project also takes advantage of Lidgren to handle sending packets between the Client, Server, and Master Server.

### Requirements
- XNA 4.0 Redistributable (can be found at: https://www.microsoft.com/en-us/Download/confirmation.aspx?id=20914)
- Xbox Controllers for each player

## Requirements (Source code)

- Visual Studio 2010 with XNA 4.0
- Windows 7

## Setup

- Make sure you have XNA 4.0 installed (you may need to restart if you just installed it)
- Download the release zip and unzip it
- Run the Master Server
- Run the Server
- Run the Client
- Register an account on the Master Server
- Login
- Choose a loadout
- Join your Server
- If you want to play over the network:
  - Set up the correct port forwarding for the hosting computer (Default Port 7777 for Server and ???? for Master Server)
  - Update Client and Server config.txt to point correct public IP that your Master Server is hosted on  
    (look for "Master Server IP = localhost" and replace "localhost" with the correct IP)

## Controls

- LStick: Movement
- RB: Sprint
- RS: Aim (Only for Pistol)
- LB: Nothing
- RT: Main Weapon
- LT: Offhand
- A: Jump>Double Jump
- X: Special #1
- Y: Special #2
- B: Special #3

## Tips

- Specials cost mana, and give no feedback when you don't have enough mana (The Spike Trap costs all your mana, the Poison Spike Trap costs more than half)
- Players drop health orbs on death
- Player health is indicated by the color of the name tag over their head
- The swords have fully charged attacks (Magic Sword increases range, normal Sword increases damage)
- The Pistol can be aimed with the Right Stick if you stand still
