#!/bin/bash
cd "$(dirname "$0")" || exit 1

STRG_PROVIDER=$1

if [ -z $STRG_PROVIDER ]; then
    echo 'Must provide storage provider as an argument'
    exit 1
fi

STRG_PROVIDER_DIR=../$STRG_PROVIDER

ls $STRG_PROVIDER_DIR >/dev/null 2>&1
if [ $? -ne 0 ]; then
    echo 'Provider not found'
    exit 1
fi

dotnet pack $STRG_PROVIDER_DIR
cp $STRG_PROVIDER_DIR/bin/Release/*.nupkg $KANAFKA_PACKAGES_DIR
