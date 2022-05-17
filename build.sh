#!/bin/bash

FRUITSHOP_DEFAULT_OUTPUT_DIRECTORY="$HOME/.local/bin"
FRUITSHOP_OUTPUT_DIRECTORY="$FRUITSHOP_DEFAULT_OUTPUT_DIRECTORY"
FRUITSHOP_REBUILD=0
FRUITSHOP_CLEAN=0
FRUITSHOP_BUILD_ARGS=""
FRUITSHOP_TARGET_FRAMEWORK="net7.0"

for arg in "$@"
do
case $arg in
  "--clean")
  FRUITSHOP_REBUILD=1
  FRUITSHOP_CLEAN=1
  shift # past argument
  ;;
  -f=*|--framework=*)
  FRUITSHOP_TARGET_FRAMEWORK="${arg#*=}"
  shift # past argument
  ;;
  -o=*|--output=*)
  FRUITSHOP_OUTPUT_DIRECTORY="${arg#*=}"
  shift # past argument
  ;;
  "-h"|"--help")
  echo "Usage: build.sh [Options]"
  echo ""
  echo "Options:"
  echo "  --clean         Clean output directory and exit"
  echo "  -f|--framework  Target framework to build for (default: net7.0)"
  echo "  -h|--help       Print this help screen"
  echo "  -o|--output     Specify output directory (default: $FRUITSHOP_DEFAULT_OUTPUT_DIRECTORY)"
  echo "  --rebuild       Clean output directory before build"
  echo "  -s|--strip      Remove redundant symbols from output binaries"
  echo ""
  exit 0
  shift # past argument
  ;;
  "--rebuild")
  FRUITSHOP_REBUILD=1
  shift # past argument
  ;;
  "-s"|"--strip")
  FRUITSHOP_BUILD_ARGS="-p:PublishTrimmed=true $FRUITSHOP_BUILD_ARGS"
  shift # past argument
  ;;
  *)    # unknown option
  echo "Unknown option $arg"
  exit 1
  shift # past argument
  ;;
esac
done

if [ ! -d "$FRUITSHOP_OUTPUT_DIRECTORY" ]; then
  mkdir -p "$FRUITSHOP_OUTPUT_DIRECTORY"
fi

if [ $FRUITSHOP_REBUILD -eq 1 ]; then
  rm -f $FRUITSHOP_OUTPUT_DIRECTORY/ultralove-aps
  rm -f $FRUITSHOP_OUTPUT_DIRECTORY/ultralove-aps.pdb
  if [ $FASTSPEC_CLEAN -eq 1 ]; then
    exit 0
  fi
fi

FRUITSHOP_RUNTIME_PLATFORM=$(uname)
if [ "$FRUITSHOP_RUNTIME_PLATFORM" == "Darwin" ]; then
  FASTSPEC_RUNTIME_ID="osx-x64"
elif [ "$FRUITSHOP_RUNTIME_PLATFORM" == "Linux" ]; then
  FRUITSHOP_RUNTIME_ID="linux-x64"
else
  FRUITSHOP_RUNTIME_ID="windows-x64"
fi

dotnet publish --output "$FRUITSHOP_OUTPUT_DIRECTORY" --runtime "$FRUITSHOP_RUNTIME_ID" --framework "$FRUITSHOP_TARGET_FRAMEWORK" --configuration Release --self-contained true -p:PublishSingleFile=true $FRUITSHOP_BUILD_ARGS
