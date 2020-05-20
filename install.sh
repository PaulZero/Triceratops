#!/bin/bash
rm docker-compose.override.yml
cp linux_docker-compose.override.yml docker-compose.override.yml
docker build -t triceratops_volumemanager:latest -f VolumeManager.Dockerfile .
docker-compose -f docker-compose.yml up -d
