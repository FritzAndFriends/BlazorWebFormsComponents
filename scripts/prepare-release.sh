#!/bin/bash
# Script to prepare a release by updating version.json and generating release notes
# Usage: ./scripts/prepare-release.sh <new-version>
# Example: ./scripts/prepare-release.sh 0.14

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if version argument is provided
if [ -z "$1" ]; then
    echo -e "${RED}Error: Version number is required${NC}"
    echo "Usage: $0 <new-version>"
    echo "Example: $0 0.14"
    exit 1
fi

NEW_VERSION=$1

# Validate version format
if ! [[ $NEW_VERSION =~ ^[0-9]+\.[0-9]+$ ]]; then
    echo -e "${RED}Error: Invalid version format. Use format: X.Y (e.g., 0.14)${NC}"
    exit 1
fi

# Check if we're on the dev branch
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "dev" ]; then
    echo -e "${YELLOW}Warning: You are not on the 'dev' branch (current: $CURRENT_BRANCH)${NC}"
    read -p "Do you want to continue? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Check for uncommitted changes
if ! git diff-index --quiet HEAD --; then
    echo -e "${RED}Error: You have uncommitted changes. Please commit or stash them first.${NC}"
    exit 1
fi

echo -e "${GREEN}Preparing release v${NEW_VERSION}...${NC}"

# Update version.json
echo -e "${YELLOW}Updating version.json...${NC}"
cat version.json | jq --arg version "$NEW_VERSION" '.version = $version' > version.json.tmp
mv version.json.tmp version.json

# Generate release notes
echo -e "${YELLOW}Generating release notes from dev branch...${NC}"
./scripts/generate-release-notes.sh > RELEASE_NOTES.md

echo -e "${GREEN}Release preparation complete!${NC}"
echo ""
echo "Next steps:"
echo "1. Review the changes in version.json and RELEASE_NOTES.md"
echo "2. Commit the changes: git add version.json RELEASE_NOTES.md && git commit -m 'Prepare release v${NEW_VERSION}'"
echo "3. Merge dev into main"
echo "4. Run ./scripts/publish-release.sh ${NEW_VERSION} to create the tag and GitHub release"
