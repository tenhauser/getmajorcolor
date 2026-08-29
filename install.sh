#!/usr/bin/env bash
set -euo pipefail

# Install script for getmajorcolors.
# Downloads the latest release binary from GitHub and places it on PATH.

REPO="<owner>/getmajorcolors"
INSTALL_DIR="${INSTALL_DIR:-$HOME/.local/bin}"

OS=$(uname -s | tr '[:upper:]' '[:lower:]')
ARCH=$(uname -m)

 case "$ARCH" in
    x86_64)
        RID="x64"
        ;;
    aarch64|arm64)
        RID="arm64"
        ;;
    *)
        echo "Unsupported architecture: $ARCH" >&2
        exit 1
        ;;
esac

case "$OS" in
    linux)
        PLATFORM="linux"
        EXT="tar.gz"
        ;;
    darwin)
        PLATFORM="osx"
        EXT="tar.gz"
        ;;
    mingw*|cygwin*|msys*)
        PLATFORM="win"
        EXT="zip"
        ;;
    *)
        echo "Unsupported operating system: $OS" >&2
        exit 1
        ;;
esac

ASSET="getmajorcolors-${PLATFORM}-${RID}.${EXT}"
URL="https://github.com/${REPO}/releases/latest/download/${ASSET}"

echo "Downloading ${ASSET}..."
TMP_DIR=$(mktemp -d)
trap 'rm -rf "$TMP_DIR"' EXIT

if command -v curl >/dev/null 2>&1; then
    curl -fsSL "$URL" -o "$TMP_DIR/$ASSET"
elif command -v wget >/dev/null 2>&1; then
    wget -q "$URL" -O "$TMP_DIR/$ASSET"
else
    echo "curl or wget is required" >&2
    exit 1
fi

echo "Extracting..."
cd "$TMP_DIR"
if [ "$EXT" = "zip" ]; then
    unzip -q "$ASSET"
else
    tar -xzf "$ASSET"
fi

mkdir -p "$INSTALL_DIR"

if [ "$PLATFORM" = "win" ]; then
    cp getmajorcolors.exe "$INSTALL_DIR/" 2>/dev/null || cp getmajorcolors "$INSTALL_DIR/"
    chmod +x "$INSTALL_DIR/getmajorcolors.exe" 2>/dev/null || true
else
    cp getmajorcolors "$INSTALL_DIR/"
    chmod +x "$INSTALL_DIR/getmajorcolors"
fi

echo "Installed to $INSTALL_DIR"
echo "Ensure $INSTALL_DIR is on your PATH."
