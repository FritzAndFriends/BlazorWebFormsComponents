#!/bin/bash
# Script to generate release notes from commits in the dev branch since the last merge to main
# This can be run standalone or as part of prepare-release.sh

set -e

# Get the latest commit on main branch from remote
git fetch origin main --quiet 2>/dev/null || true

# Find the merge base between dev and main
MERGE_BASE=$(git merge-base HEAD origin/main 2>/dev/null || git merge-base HEAD main 2>/dev/null || echo "")

if [ -z "$MERGE_BASE" ]; then
    echo "Warning: Could not find merge base with main branch. Using last 20 commits."
    MERGE_BASE="HEAD~20"
fi

# Get the commits since the merge base
echo "# Release Notes"
echo ""
echo "## Changes in this release"
echo ""

# Get commits, filtering out merge commits and grouping by type
git log $MERGE_BASE..HEAD --no-merges --pretty=format:"%s" | while read -r line; do
    # Try to categorize commits
    if [[ $line =~ ^(feat|feature|add|new): ]]; then
        echo "### ✨ New Features"
        echo "- $line"
    elif [[ $line =~ ^(fix|bug): ]]; then
        echo "### 🐛 Bug Fixes"
        echo "- $line"
    elif [[ $line =~ ^(docs|doc|documentation): ]]; then
        echo "### 📚 Documentation"
        echo "- $line"
    elif [[ $line =~ ^(test|tests): ]]; then
        echo "### 🧪 Tests"
        echo "- $line"
    elif [[ $line =~ ^(refactor|refactoring): ]]; then
        echo "### ♻️ Refactoring"
        echo "- $line"
    elif [[ $line =~ ^(chore|build|ci): ]]; then
        echo "### 🔧 Maintenance"
        echo "- $line"
    else
        echo "### 📝 Other Changes"
        echo "- $line"
    fi
done | awk '
    BEGIN { current_section = "" }
    /^###/ {
        if (current_section != $0) {
            if (current_section != "") print ""
            print $0
            print ""
            current_section = $0
        }
        next
    }
    { print }
'

echo ""
echo "## Contributors"
echo ""
git log $MERGE_BASE..HEAD --no-merges --pretty=format:"- %aN" | sort -u

echo ""
echo ""
echo "_Full Changelog_: https://github.com/FritzAndFriends/BlazorWebFormsComponents/compare/v$(jq -r .version version.json)...\$\{TAG\}"
