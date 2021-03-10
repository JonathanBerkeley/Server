[![forthebadge](https://forthebadge.com/images/badges/works-on-my-machine.svg)](https://forthebadge.com) &nbsp;&nbsp;&nbsp; [![forthebadge](https://forthebadge.com/images/badges/uses-badges.svg)](https://forthebadge.com)
# Server software for an in-development unity video game
With thanks to Tom Weiland for the foundational code https://github.com/tom-weiland

# Versioning info:
Major stable versions will have a release download     
[Current version](#099---100)

## Unversioned
Early development / unstable

## 0.9.0 - 0.9.1
Unworking versions for testing

## 0.9.2
Working server version

## 0.9.3 
Cleaned up version with decluttered server logging  
https://github.com/JonathanBerkeley/Server/releases/tag/0.9.3

## 0.9.4 
Projectile handling code reintroduced, server supports projectile data
https://github.com/JonathanBerkeley/Server/releases/tag/0.9.4

## 0.9.5 - 0.9.6
Versions that now support multiplayer chat

## 0.9.7
Foundational support for chat commands. New chat command with usage:
/msg [user] \[message]

## 0.9.8
Added support for disconnect alert packets, to alert other clients that a client has disconnected.

## 0.9.9 - 1.0.0
(Supports client version 1.2.0)
Added server-client flags to communicate errors to the client (such as server full or username taken etc).    
Reformatted server console messages.   
Client version now communicated to the server.    
Fixed issue with server being full sending clients to ghost server.    
https://github.com/JonathanBerkeley/Server/releases/tag/1.0.0
