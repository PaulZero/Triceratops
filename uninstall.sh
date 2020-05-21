#!/bin/bash
docker container rm -f Triceratops.VolumeManager
docker image rm -f triceratops_volumemanager:latest
docker-compose down -v --rmi all
