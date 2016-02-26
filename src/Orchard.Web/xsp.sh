#!/bin/bash

monopath="/opt/mono/lib/mono/4.5/"
xsp4="xsp4.exe"
MonoSec="Mono.Security.dll"
MonoWebSrv="Mono.WebServer2.dll"

cp $monopath$xsp4 .
cp $monopath$MonoSec .
cp $monopath$MonoWebSrv .

mkdir bin

cp $monopath$xsp4 bin/
cp $monopath$MonoSec bin/
cp $monopath$MonoWebSrv bin/

mono_vm="/opt/mono/bin/mono"

source ~/Documents/mono-local-env
# log mono assembly probing
#MONO_LOG_LEVEL=info MONO_LOG_MASK=asm 
$mono_vm xsp4.exe --printlog --logfile=xsp.log --loglevels=Debug,Notice,Warning,Error --verbose --applications=/:.  --address=127.0.0.1 --port=8080 -no-hidden --https
