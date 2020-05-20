#!/bin/bash
docker image rm triceratops_volumemanager:latest
docker-compose stop
docker-compose rm -f