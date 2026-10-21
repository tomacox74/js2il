"""Immutable full-inventory and checkout-local registration adapters."""
import hashlib
import json
from pathlib import Path
import re
import subprocess
from .client import bytea, canonical, identity, sha

REPO = Path(__file__).resolve().parents[3]


def hash_bytes(value):
    return hashlib.sha256(value).hexdigest()


def normalize_fixture(row, root=None):
    metadata = row.get('metadata', {})
    negative = metadata.get('negative') or {}
    support = row.get('isSupportFile', bool(re.search(r'(^|/)(?:_[^/]*|[^/]*_FIXTURE)\.js$', row['path'])))
    dependencies = []
    unresolved = root is None
    if root:
        root = Path(root).resolve()
        source = (root / row['path']).read_text(encoding='utf8')
        # Conservative closure: includes plus relative static/dynamic literal imports.
        names = [('harness/' + name, 'harness') for name in metadata.get('execution', {}).get('harnessIncludes', metadata.get('includes', []))]
        pending = [(root / row['path'], source)]
        visited = {row['path']}
        while pending:
            parent, text = pending.pop()
            for relative in re.findall(r'''(?:\b(?:import|export)\s+(?:[^'"\n]*?\s+from\s+)?|\bimport\s*\()\s*['"](\.[^'"]+)['"]''', text):
                target = parent.parent / relative
                if not target.suffix:
                    target = target.with_suffix('.js')
                try:
                    name = target.resolve().relative_to(root).as_posix()
                    names.append((name, 'module'))
                    if name not in visited:
                        visited.add(name)
                        if target.is_file():
                            pending.append((target,target.read_text(encoding='utf8')))
                        else:
                            unresolved=True
                except ValueError:
                    unresolved=True
            # Computed imports cannot establish a verified dependency closure.
            if re.search(r"""\bimport\s*\(\s*[^\s'\"]""", text):
                unresolved=True
        for name, kind in sorted(set(names)):
            target = root / name
            if not target.is_file():
                unresolved = True
                continue
            dependencies.append({'dependency_path': name, 'content_sha256': hash_bytes(target.read_bytes()),
                                 'dependency_kind': kind, 'resolved_path': name if name.startswith('test/') else None})
    variants = [{'variant': v, 'expected_phase': negative.get('phase'), 'expected_error_type': negative.get('type'), 'required': True}
                for v in sorted(row['variants'])] if not support else []
    return {'path': row['path'], 'sha256': row['sha256'], 'metadata': metadata,
            'metadata_state': 'unresolved' if root is None else ('error' if row['state'] == 'metadata-error' else 'valid'),
            'is_support_file': support, 'feature_tags': sorted(metadata.get('features', [])),
            'dependency_manifest_digest': None if unresolved else sha(dependencies),
            'variants': variants, 'dependencies': dependencies}


def inventory_digest(rows):
    return hash_bytes(''.join(canonical(row)+'\n' for row in sorted(rows, key=lambda x: x['path'])).encode())


def register(client, repository, upstream, rows, reuse_sealed=False):
    paths={row['path'] for row in rows}
    for row in rows:
        for dependency in row['dependencies']:
            if dependency['resolved_path'] not in paths:
                dependency['resolved_path']=None
        if row['dependency_manifest_digest'] is not None:
            row['dependency_manifest_digest']=sha(row['dependencies'])
    digest = inventory_digest(rows)
    corpus = identity(upstream['cloneUrl'], upstream['commit'], digest)
    fixtures = {row['path']: identity(corpus, row['path']) for row in rows}
    if reuse_sealed:
        existing = client.one('corpora', corpus_id=corpus)
        if existing and existing['inventory_state'] == 'sealed':
            expected = {'upstream_url': upstream['cloneUrl'], 'revision': upstream['commit'],
                        'revision_algorithm': 'sha1', 'inventory_digest': bytea(digest),
                        'expected_fixture_count': len(rows)}
            if any(existing.get(key) != value for key, value in expected.items()):
                raise ValueError('Sealed inventory identity mismatch')
            # Sealed inventory rows are immutable and the API read is repository scoped.
            print('Reusing verified sealed inventory:', len(rows), 'fixtures', flush=True)
            return corpus, fixtures
    client.put('corpora', [{'corpus_id': corpus, 'upstream_url': upstream['cloneUrl'], 'revision': upstream['commit'],
                           'revision_algorithm': 'sha1', 'inventory_digest': bytea(digest), 'expected_fixture_count': len(rows)}])
    client.put('repository_corpora', [{'repository_id': repository, 'corpus_id': corpus}])
    client.put('fixtures', [{'fixture_id': fixtures[r['path']], 'corpus_id': corpus, 'upstream_path': r['path'],
                            'content_sha256': bytea(r['sha256']), 'metadata': r['metadata'], 'metadata_state': r['metadata_state'],
                            'is_support_file': r['is_support_file'], 'feature_tags': r['feature_tags'],
                            'dependency_manifest_digest': bytea(r['dependency_manifest_digest'])} for r in rows])
    client.put('fixture_variants', [dict(v, fixture_id=fixtures[r['path']]) for r in rows for v in r['variants']])
    client.put('fixture_dependencies', [dict(fixture_id=fixtures[r['path']], dependency_path=d['dependency_path'],
                                           content_sha256=bytea(d['content_sha256']), dependency_kind=d['dependency_kind'],
                                           resolved_fixture_id=fixtures.get(d['resolved_path'])) for r in rows for d in r['dependencies']])
    # A repeated registration of an already sealed inventory must not UPDATE it.
    # Transition API returns identical sealed replay after hash verification.
    client.call('transition', 'corpora', {'corpus_id': corpus}, 0, {'inventory_state': 'sealed'})
    return corpus, fixtures


def enrich_specification_metadata(client, args):
    """Append source-verified annotations without changing sealed inventory/evidence."""
    from scripts.test262 import catalog

    corpus = client.one('corpora', corpus_id=args.corpus)
    if not corpus:
        raise ValueError('Unknown or inaccessible corpus')
    if corpus['inventory_state'] != 'sealed':
        raise ValueError('Specification metadata enrichment requires a sealed corpus')
    root = Path(args.root).resolve()
    revision = subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip()
    if revision != corpus['revision']:
        raise ValueError('Enrichment checkout revision differs from the original corpus revision')
    if subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain', '--untracked-files=no'], text=True).strip():
        raise ValueError('Enrichment requires a clean upstream checkout')
    count = 0
    pending = []

    def upload(fixtures):
        rows = catalog.bridge({'command': 'metadata', 'root': str(root),
                               'paths': [f['upstream_path'] for f in fixtures]})
        if len(rows) != len(fixtures):
            raise ValueError('Incomplete enrichment metadata response')
        annotations = []
        for fixture, row in zip(fixtures, rows):
            if row['path'] != fixture['upstream_path'] or bytea(row['sha256']) != fixture['content_sha256']:
                raise ValueError('Enrichment fixture path/content differs from original inventory: ' + fixture['upstream_path'])
            metadata = {key: row['metadata'][key] for key in
                        ('description', 'info', 'author', 'esid', 'es5id', 'es6id', 'specification')}
            annotations.append({'fixture_id': fixture['fixture_id'], 'upstream_revision': revision,
                                'upstream_path': fixture['upstream_path'], 'content_sha256': fixture['content_sha256'],
                                'metadata': metadata, 'metadata_sha256': bytea(sha(metadata))})
        client.put('fixture_specification_metadata', annotations)
        return len(annotations)

    for fixture in client.read('fixtures', {'corpus_id': args.corpus}):
        pending.append(fixture)
        if len(pending) == 250:
            count += upload(pending)
            pending = []
    if pending:
        count += upload(pending)
    return {'corpus': args.corpus, 'revision': revision, 'enriched_fixtures': count}


def registrations(client, repository, source_revision, corpus_rows):
    import sys
    sys.path.insert(0, str(REPO / 'scripts/test262'))
    import catalog
    root = REPO / 'tests/Jroc.Test262.Tests'
    paths, warnings = catalog.registration_inventory(root)
    records = []
    for path, sources in sorted(paths.items()):
        for source in sorted(sources):
            # Literal fixture names are relative to the C# caller's JavaScript
            # directory, including any nested segments in the name. Two callers
            # can register the same upstream path from different local files.
            caller = Path(source).parent
            relative = Path(path.removeprefix('test/')).relative_to(caller)
            native_file = root / caller / 'JavaScript' / relative
            records.append({'upstream_path': path, 'source_file': source, 'native_fixture_sha256': bytea(hash_bytes(native_file.read_bytes())),
                            'registration_kind': 'native', 'fixture_id': corpus_rows.get(path)})
    snapshot = identity(repository, source_revision, sha(records))
    client.put('registration_snapshots', [{'snapshot_id': snapshot, 'repository_id': repository, 'source_revision': source_revision,
                                          'inventory_sha256': bytea(sha(records)), 'warnings': warnings}])
    client.put('registrations', [dict(r, snapshot_id=snapshot) for r in records])
    client.call('transition', 'registration_snapshots', {'snapshot_id': snapshot}, 0, {'state': 'sealed'})
    return snapshot
