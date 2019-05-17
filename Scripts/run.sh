#!/bin/bash

export UI=${1:-HTML}

export STATIC_CONTENT_ROOT=$( cd SharpMemoUI.${UI} ; pwd )
( cd SharpMemoServer && dotnet run )