#!/bin/bash
rm .env
rm docker-compose.override.yml
cp linux.env .env
cp linux_docker-compose.override.yml docker-compose.override.yml
docker build -t triceratops_volumemanager:latest -f VolumeManager.Dockerfile .
docker-compose -f composefiles/docker-compose.production.yml -f composefiles/docker-compose.linux.yml up -d
