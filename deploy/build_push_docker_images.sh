#!/bin/bash
set -e

# Error handling function
failure() {
  local lineno=$1
  local msg=$2
  echo "Error occurred at line $lineno: $msg"
}
trap 'failure ${LINENO} "$BASH_COMMAND"' ERR

# Parse command line arguments
USE_CURRENT_BRANCH=false

while [[ $# -gt 0 ]]; do
  case $1 in
    --current-branch)
      USE_CURRENT_BRANCH=true
      shift
      ;;
    *)
      echo "Unknown option: $1"
      echo "Usage: $0 [--current-branch]"
      exit 1
      ;;
  esac
done

# 1. check out main branch and pull rebase (unless --current-branch is used)
if [ "$USE_CURRENT_BRANCH" = true ]; then
  CURRENT_BRANCH=$(git branch --show-current)
  echo "Using current branch: $CURRENT_BRANCH"
else
  echo "Checking out main branch and pulling/rebasing..."
  git checkout main
  git pull --rebase origin main
fi

# 2. ask user current version
echo "Please enter the version to build (e.g., 1.0.0):"
read -r VERSION

if [ -z "$VERSION" ]; then
  echo "Version cannot be empty. Exiting."
  exit 1
fi

# 3. add tag to current branch with current version value (unless --current-branch is used)
if [ "$USE_CURRENT_BRANCH" = false ]; then
  echo "Tagging current branch with version $VERSION..."
  git tag -a "$VERSION" -m "Release version $VERSION"
else
  echo "Skipping tag creation (using current branch mode)..."
fi

# 4. call dotnet gitversion and set current version as variable
echo "Running dotnet-gitversion..."
# Using dotnet-gitversion tool.
# Usually, gitversion provides a json output.
GITVERSION_JSON=$(dotnet gitversion)
GIT_VERSION=$(echo "$GITVERSION_JSON" | jq -r '.FullSemVer')

if [ -z "$GIT_VERSION" ]; then
  echo "Failed to extract Git Version. Using $VERSION as fallback."
  GIT_VERSION=$VERSION
fi

echo "Git Version detected: $GIT_VERSION"

# 5. add a check to ensure that a .env file exists before launching build process
FRONTEND_DIR="sources/gui/Ggio.BikeSherpa.Frontend"
ENV_FILE="$FRONTEND_DIR/.env"

echo "Checking for .env file at $ENV_FILE..."
if [ ! -f "$ENV_FILE" ]; then
  echo "Error: .env file not found in $FRONTEND_DIR."
  echo "Please create it (e.g., from .env.blank) before building."
  exit 1
fi

# 5. build docker image of Backend project tagged with BikeSherpa/Backend:latest and with current git version
# We need to build from the root because the Dockerfile copies from sources/core/...
echo "Building Docker image..."
ACR_NAME="bikeherpa"
IMAGE_NAME="bikesherpa/backend"
ACR_IMAGE_PREFIX="$ACR_NAME.azurecr.io"

docker build -t "$IMAGE_NAME:latest" \
             -t "$IMAGE_NAME:$GIT_VERSION" \
             -t "$ACR_IMAGE_PREFIX/$IMAGE_NAME:latest" \
             -t "$ACR_IMAGE_PREFIX/$IMAGE_NAME:$GIT_VERSION" \
             -f sources/core/Ggio.BikeSherpa.Backend/Dockerfile .
             
echo "Pushing Docker image of backend to ACR..."
docker push "$ACR_IMAGE_PREFIX/$IMAGE_NAME:latest"
docker push "$ACR_IMAGE_PREFIX/$IMAGE_NAME:$GIT_VERSION"
             
IMAGE_NAME="bikesherpa/front-web"

docker build -t "$IMAGE_NAME:latest" \
            -t "$IMAGE_NAME:$GIT_VERSION" \
            -t "$ACR_IMAGE_PREFIX/$IMAGE_NAME:latest" \
            -t "$ACR_IMAGE_PREFIX/$IMAGE_NAME:$GIT_VERSION" \
            -f "$FRONTEND_DIR/Dockerfile" "$FRONTEND_DIR"

# 6. push docker image to ACR
echo "Pushing Docker image of frontend to ACR..."
docker push "$ACR_IMAGE_PREFIX/$IMAGE_NAME:latest"
docker push "$ACR_IMAGE_PREFIX/$IMAGE_NAME:$GIT_VERSION"

# 7. push current version tag to origin (unless --current-branch is used)
if [ "$USE_CURRENT_BRANCH" = false ]; then
  echo "Pushing tag $VERSION to origin..."
  git push origin "$VERSION"
else
  echo "Skipping tag push (using current branch mode)..."
fi

echo "Build and push completed successfully!"
