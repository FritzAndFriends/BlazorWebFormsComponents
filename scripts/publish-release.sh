#!/bin/bash
# Script to publish a release using Nerdbank.GitVersioning
# Usage: ./scripts/publish-release.sh
# Example: ./scripts/publish-release.sh

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if nbgv is installed
if ! command -v nbgv &> /dev/null; then
    echo -e "${RED}Error: nbgv (Nerdbank.GitVersioning CLI) is not installed${NC}"
    echo "Install it with: dotnet tool install -g nbgv"
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

# Get the version from version.json
if [ ! -f "version.json" ]; then
    echo -e "${RED}Error: version.json not found${NC}"
    exit 1
fi

VERSION=$(jq -r .version version.json)
if [ -z "$VERSION" ] || [ "$VERSION" = "null" ]; then
    echo -e "${RED}Error: Could not read version from version.json${NC}"
    exit 1
fi

TAG_NAME="v${VERSION}"

# Check if tag already exists
if git rev-parse "$TAG_NAME" >/dev/null 2>&1; then
    echo -e "${RED}Error: Tag $TAG_NAME already exists${NC}"
    exit 1
fi

# Generate release notes if RELEASE_NOTES.md doesn't exist
if [ ! -f "RELEASE_NOTES.md" ]; then
    echo -e "${YELLOW}Generating release notes...${NC}"
    ./scripts/generate-release-notes.sh > RELEASE_NOTES.md
fi

echo -e "${GREEN}Publishing release v${VERSION}...${NC}"

# Create and push the tag using nbgv
echo -e "${YELLOW}Creating git tag using Nerdbank.GitVersioning...${NC}"

# Use nbgv tag to create the tag and capture output
if ! nbgv tag > /tmp/nbgv_tag_output.txt 2>&1; then
    echo -e "${RED}Error: Failed to create tag with nbgv${NC}"
    cat /tmp/nbgv_tag_output.txt
    exit 1
fi

# nbgv doesn't output the tag name, so we construct it from the version
CREATED_TAG="v${VERSION}"

# Verify the tag was created
if ! git rev-parse "$CREATED_TAG" >/dev/null 2>&1; then
    echo -e "${RED}Error: Tag $CREATED_TAG was not created successfully${NC}"
    exit 1
fi

echo -e "${YELLOW}Pushing tag to origin...${NC}"
git push origin "$CREATED_TAG"

echo -e "${GREEN}Tag ${CREATED_TAG} created and pushed successfully!${NC}"
echo ""
echo "Next steps:"
echo "1. The GitHub Actions workflow will automatically build and publish the NuGet package"
echo "2. Create a GitHub release at: https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases/new?tag=${CREATED_TAG}"
echo "3. Use the contents of RELEASE_NOTES.md for the release description"
echo ""
echo "Or use the GitHub CLI to create the release automatically:"
echo "gh release create ${CREATED_TAG} --title \"Release v${VERSION}\" --notes-file RELEASE_NOTES.md"
