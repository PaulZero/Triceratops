# Triceratops

## Introduction
So this is like those control panel things you get access to when you pay
some company to host a game server for you. However it's not in any way 
clustered as so many of them seems to be, and is focused purely on running
your game servers on a single box (you know, if you've decided to get an
old box running a couple of Minecraft servers for you and your mates).

It depends on Docker and will presumably run on all manner of platforms
as long as they can run Docker.

## Concepts

### Servers (previously known as "stacks")

A server is, well, it's one game server. Most of these will just be a single
Docker container and that's lovely. However they could potentially be linked
to multiple containers.

### Server configs

The configuration for a specific server, these are game dependent and will
handle things like the server password, player limit, and other things
specific to the game being run.

### Containers

Erm... It's a Docker container.

## Notes

Paul - stop going nuts with interfaces, get it working you shithouse.