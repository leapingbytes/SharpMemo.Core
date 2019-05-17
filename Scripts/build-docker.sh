#!/usr/bin/env bash

export UI=${1:-HTML}

mkdir -p ./obj/UI
cp -r ./SharpMemoUI.${UI}/* ./SharpMemoServer/obj/UI

cd ./SharpMemoServer
docker build -t sharp-memo-server .