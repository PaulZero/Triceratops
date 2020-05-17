#!/bin/bash
docker build -t triceratops_volumemanager:latest Triceratops.VolumeManager
docker-compose -f docker-compose.yml up -d
