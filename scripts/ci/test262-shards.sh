#!/usr/bin/env bash
set -euo pipefail

project=tests/Jroc.Test262.Tests/Jroc.Test262.Tests.csproj
language=Jroc.Test262.Tests.language.
builtins=Jroc.Test262.Tests.built_ins.
shards=(language-expressions language-statements builtins-array-object builtins-typed-regexp-string other)

filter_for() {
  case "$1" in
    language-expressions)
      echo "Category!=ReleaseOnly&FullyQualifiedName~${language}expressions."
      ;;
    language-statements)
      echo "Category!=ReleaseOnly&FullyQualifiedName~${language}statements."
      ;;
    builtins-array-object)
      echo "Category!=ReleaseOnly&(FullyQualifiedName~${builtins}Array.|FullyQualifiedName~${builtins}Object.)"
      ;;
    builtins-typed-regexp-string)
      echo "Category!=ReleaseOnly&(FullyQualifiedName~${builtins}TypedArray.|FullyQualifiedName~${builtins}TypedArrayConstructors.|FullyQualifiedName~${builtins}RegExp.|FullyQualifiedName~${builtins}String.)"
      ;;
    other)
      echo "Category!=ReleaseOnly&FullyQualifiedName!~${language}expressions.&FullyQualifiedName!~${language}statements.&FullyQualifiedName!~${builtins}Array.&FullyQualifiedName!~${builtins}Object.&FullyQualifiedName!~${builtins}TypedArray.&FullyQualifiedName!~${builtins}TypedArrayConstructors.&FullyQualifiedName!~${builtins}RegExp.&FullyQualifiedName!~${builtins}String."
      ;;
    *)
      echo "Unknown test262 shard: $1" >&2
      exit 1
      ;;
  esac
}

list_tests() {
  dotnet test "$project" --no-build --list-tests --filter "$1" |
    sed -n 's/^    //p'
}

if [[ "${1:-}" == --verify ]]; then
  for shard in "${shards[@]}"; do
    if [[ -z "$(list_tests "$(filter_for "$shard")")" ]]; then
      echo "Empty test262 shard: $shard" >&2
      exit 1
    fi
  done

  diff -u \
    <(list_tests 'Category!=ReleaseOnly' | LC_ALL=C sort) \
    <(for shard in "${shards[@]}"; do list_tests "$(filter_for "$shard")"; done | LC_ALL=C sort)
  echo "All test262 cases occur in exactly one shard."
else
  filter_for "${1:-}"
fi
