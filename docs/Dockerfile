FROM squidfunk/mkdocs-material:9.7.1

RUN apk add --no-cache \
      git \
      git-fast-import \
      openssh \
  && apk add --no-cache --virtual .build gcc musl-dev \
  && pip install --no-cache-dir \
      'markdown-include==0.5.1' \
      'mike==1.1.2' \
      'mkdocs-exclude==1.0.2' \
      'mkdocs-macros-plugin==0.5.12' \
  && apk del .build gcc musl-dev \
  && rm -rf /tmp/*
