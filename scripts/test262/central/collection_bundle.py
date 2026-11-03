"""Verify and install the prepared binaries before the worker receives credentials."""
import argparse
import hashlib
import json
from pathlib import Path
import tarfile

RUNTIME_PATHS = ('src/Cli/bin/Release/net10.0',
                 'scripts/test262/NativeScreeningHost/bin/Release/net10.0')


def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def install(directory, expected_context_digest, root):
    directory = Path(directory)
    context_file = directory / 'central-context.json'
    if digest(context_file) != expected_context_digest:
        raise ValueError('Context file digest mismatch')
    context = json.loads(context_file.read_text(encoding='utf-8'))
    runtime = directory / 'runtime.tar.gz'
    image = directory / 'fixture-image.tar.gz'
    if digest(runtime) != context['runtime_digest'] or digest(image) != context['image_digest']:
        raise ValueError('Prepared runtime or fixture image digest mismatch')
    with tarfile.open(runtime, 'r:gz') as archive:
        members = archive.getmembers()
        for member in members:
            path = Path(member.name)
            allowed = any(member.name == prefix or member.name.startswith(prefix + '/')
                          for prefix in RUNTIME_PATHS)
            if path.is_absolute() or '..' in path.parts or not allowed or not (member.isfile() or member.isdir()):
                raise ValueError('Unexpected runtime archive member')
        archive.extractall(root, members=members, filter='data')


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('directory')
    parser.add_argument('context_digest')
    args = parser.parse_args()
    install(args.directory, args.context_digest, Path.cwd())


if __name__ == '__main__':
    main()
