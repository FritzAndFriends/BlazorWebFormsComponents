FROM squidfunk/mkdocs-material:9.7.1

RUN \
  apk add --no-cache \
    git \
    git-fast-import \
    openssh \
  && pip install --no-cache-dir \
    'markdown-include' \
    'mike' \
    'mkdocs-exclude' \
    'mkdocs-macros-plugin' \
    'mkdocs-git-revision-date-localized-plugin' \
  && rm -rf /tmp/*
