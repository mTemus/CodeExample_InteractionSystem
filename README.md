# Code Example: Interaction System
======

This repository contains code examples for the Interaction System mechanics for Unity Engine project.

The interaction system is inspired by [git-amend's video about Visitor Pattern](https://www.youtube.com/watch?v=Q2gQs6gIzCM). The system works similar, but it is my own implementation. 
The main idea of this system was to create an interaction mechanic for First Person Perspective that will be **reactive** and will respond to any changes on the interacted object, the interacting entity or even the world. 
For example, when player is holding a box of objects and he is pointing on a shelf, he can store items on the shelf by pressing a button or withdraw items from the shelf. But player can also open and close the box lid by pressing a key. What then?

There is the answer: [LINK](https://youtu.be/LTqIH8Ke6fA)

With the usage of **Observer Pattern** components can listen to other components and react to the changes. When player will store all objects from the box on the shelf, the input of the storing interaction will be unsubscribed, because there is no more objects to store.
It works similar with withdrawal - if all objects will be withdrawn from the shelf, the input of the withdrawal interaction will be unsubscribed.

## System Graph:
![System Graph](https://i.imgur.com/CIPZpMS.png)