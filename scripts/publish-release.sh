#!/bin/bash
# Script to publish a release by creating a git tag and GitHub release
# Usage: ./scripts/publish-release.sh <version>
# Example: ./scripts/publish-release.sh 0.14

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if version argument is provided
if [ -z "$1" ]; then
    echo -e "${RED}Error: Version number is required${NC}"
    echo "Usage: $0 <version>"
    echo "Example: $0 0.14"
    exit 1
fi

VERSION=$1
TAG_NAME="v${VERSION}"

# Validate version format
if ! [[ $VERSION =~ ^[0-9]+\.[0-9]+$ ]]; then
    echo -e "${RED}Error: Invalid version format. Use format: X.Y (e.g., 0.14)${NC}"
    exit 1
fi

# Check if we're on the main branch
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo -e "${RED}Error: You must be on the 'main' branch to publish a release (current: $CURRENT_BRANCH)${NC}"
    echo "Please merge dev into main first."
    exit 1
fi

# Check for uncommitted changes
if ! git diff-index --quiet HEAD --; then
    echo -e "${RED}Error: You have uncommitted changes. Please commit them first.${NC}"
    exit 1
fi

# Check if tag already exists
if git rev-parse "$TAG_NAME" >/dev/null 2>&1; then
    echo -e "${RED}Error: Tag $TAG_NAME already exists${NC}"
    exit 1
fi

# Check if version.json matches the requested version
CURRENT_VERSION=$(jq -r .version version.json)
if [ "$CURRENT_VERSION" != "$VERSION" ]; then
    echo -e "${RED}Error: version.json shows version $CURRENT_VERSION but you're trying to release $VERSION${NC}"
    echo "Please update version.json first or use the correct version number."
    exit 1
fi

# Generate release notes if RELEASE_NOTES.md doesn't exist
if [ ! -f "RELEASE_NOTES.md" ]; then
    echo -e "${YELLOW}Generating release notes...${NC}"
    ./scripts/generate-release-notes.sh > RELEASE_NOTES.md
fi

echo -e "${GREEN}Publishing release v${VERSION}...${NC}"

# Create and push the tag
echo -e "${YELLOW}Creating git tag ${TAG_NAME}...${NC}"
git tag -a "$TAG_NAME" -m "Release version ${VERSION}"

echo -e "${YELLOW}Pushing tag to origin...${NC}"
git push origin "$TAG_NAME"

echo -e "${GREEN}Tag ${TAG_NAME} created and pushed successfully!${NC}"
echo ""
echo "Next steps:"
echo "1. The GitHub Actions workflow will automatically build and publish the NuGet package"
echo "2. Create a GitHub release at: https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases/new?tag=${TAG_NAME}"
echo "3. Use the contents of RELEASE_NOTES.md for the release description"
echo ""
echo "Or use the GitHub CLI to create the release automatically:"
echo "gh release create ${TAG_NAME} --title \"Release v${VERSION}\" --notes-file RELEASE_NOTES.md"
