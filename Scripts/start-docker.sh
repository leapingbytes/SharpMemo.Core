#!/usr/bin/env bash

docker run -d -p 8000:80 sharp-memo-server:latest | tee /tmp/sharp-memo-server.container-id

open http://localhost:8000/index.html