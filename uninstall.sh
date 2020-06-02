#!/bin/bash
docker-compose  -f composefiles/docker-compose.production.yml -f composefiles/docker-compose.linux.yml down -v --rmi all
