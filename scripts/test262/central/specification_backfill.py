"""Scoped specification canary. Only sanitized receipts leave the supervisor."""
import argparse
from collections import Counter
from contextlib import closing
from datetime import datetime, timezone, timedelta
import hashlib
import json
import os
from pathlib import Path
import subprocess
from types import SimpleNamespace

from .client import Client, bytea, canonical, sha
from .inventory import enrich_specification_metadata
from .verify_specification_deployment import project_reference

REPOSITORY = 'faf01df8-aa65-4375-a708-6c1e555f957b'
REVISION = '2b2ecead6e828dd9af13a9ec72065e645724a50f'
SOURCE_MANIFEST = '70a161c0a68617c93eeee60f7acd73ad6bb60ac230e2a063711f108dda41a4e6'
CORPORA = {
    'cf3b4d8e-e508-5b39-94a6-dd455585be9e': '6cad81701df0babb51b54ab10ad618a2d56c4e5a30102c4d799f66ed66739bf7',
    '122d7a12-5452-52fc-87ac-3582f7615755': '99e5b5a997ba9ef3fb6acd9636989ab0acd9405877ccf2d87b05fecae33ff3a0',
    '69508b60-11b8-5541-bcf6-0c2671d59f4b': '2ed44d30a7e23c59bc98d976ae43d7cf1bffd1d9129e56925a23d1c8a22d8d04',
    'fe510e3b-b528-5d23-9aaa-9cbd2df7d440': 'caa5fe3bdf06b3ee088e6322e8cc16a4b4cbc1a2143b4bb70b64b5bf22d6bcd2',
}
FIELDS = ('description', 'info', 'author', 'esid', 'es5id', 'es6id', 'specification')
KEYS = {'corpora': 'corpus_id', 'fixtures': 'fixture_id',
        'observations': 'observation_id', 'observation_payloads': 'observation_id',
        'fixture_specification_metadata': 'fixture_id'}


def recovery_receipt(path, run_id):
    report = json.loads(Path(path).read_text())
    age = datetime.now(timezone.utc) - datetime.fromisoformat(report['checked_at'])
    if (report.get('backup_verified') is not True or
            report.get('bucket_retention_verified') is not True or
            report.get('contains_performance_data') is not False or
            report.get('stage') != 'complete' or not timedelta(0) <= age <= timedelta(hours=6) or
            not report.get('prefix', '').endswith('/' + str(int(run_id)) + '-1') or
            not report.get('receipt', {}).get('version_id')):
        raise ValueError('Fresh independently retained catalogue backup required')
    if not {'catalogue.dump', 'manifest.json'} <= set(report['objects']):
        raise ValueError('Incomplete recovery objects')
    return {'run_id': int(run_id), 'prefix': report['prefix'],
            'checked_at': report['checked_at'], 'receipt_version': report['receipt']['version_id'],
            'receipt_sha256': report['receipt']['sha256']}


def snapshot(client, corpus):
    """Fingerprint fixed historical identities; new concurrent observations are allowed."""
    result = {}
    with client.view():
        token, as_of = client.db.execute('SELECT txid_current_snapshot()::text, transaction_timestamp()').fetchone()
        for table, key in KEYS.items():
            filters = {'corpus_id': corpus} if table in ('corpora', 'fixtures') else {}
            print('Fingerprinting', table, flush=True)
            scan = client.read(table, filters, page=1000) if filters else client.export_rows(table)
            with closing(scan) as rows:
                result[table] = {row[key]: sha(row) for row in rows}
            print('Fingerprinted', table, len(result[table]), 'records', flush=True)
    return {'snapshot_token': token, 'as_of': as_of.isoformat(), 'rows': result}


def retained_fingerprints(before, after):
    proofs = {}
    for table, rows in before['rows'].items():
        if table == 'fixture_specification_metadata':
            continue
        current = after['rows'][table]
        if any(current.get(key) != digest for key, digest in rows.items()):
            raise ValueError('Historical evidence changed: ' + table)
        if table in ('fixtures', 'corpora') and set(current) != set(rows):
            raise ValueError('Sealed inventory membership changed')
        proofs[table] = {'historical_count': len(rows), 'historical_sha256': sha(rows)}
    return proofs


def prepare(client, args):
    from scripts.test262 import catalog
    root = Path(args.root).resolve()
    if subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip() != REVISION:
        raise ValueError('Original upstream revision required')
    if subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain', '--untracked-files=no'], text=True).strip():
        raise ValueError('Clean original source required')
    corpus = client.one('corpora', corpus_id=args.corpus)
    if (not corpus or corpus['inventory_state'] != 'sealed' or corpus['revision'] != REVISION or
            corpus['upstream_url'] != 'https://github.com/tc39/test262.git' or
            corpus['expected_fixture_count'] != 53432 or
            corpus['inventory_digest'] != bytea(CORPORA[args.corpus])):
        raise ValueError('Corpus does not match reviewed scope')
    fixtures = sorted(client.read('fixtures', {'corpus_id': args.corpus}, page=1000),
                      key=lambda row: row['upstream_path'])
    if len(fixtures) != corpus['expected_fixture_count']:
        raise ValueError('Incomplete original inventory')
    source_digest = hashlib.sha256()
    metadata_digest = hashlib.sha256()
    expected = {}
    states = Counter()
    diagnostics = 0
    samples = {}
    for start in range(0, len(fixtures), 250):
        batch = fixtures[start:start+250]
        rows = catalog.bridge({'command': 'metadata', 'root': str(root),
                               'paths': [row['upstream_path'] for row in batch]}, timeout=60)
        if len(rows) != len(batch):
            raise ValueError('Incomplete source extraction')
        for fixture, row in zip(batch, rows):
            path = fixture['upstream_path']
            if row['path'] != path or bytea(row['sha256']) != fixture['content_sha256']:
                raise ValueError('Original source identity mismatch')
            metadata = {key: row['metadata'][key] for key in FIELDS}
            digest = sha(metadata, database_numbers=True)
            annotation = {'fixture_id': fixture['fixture_id'], 'upstream_revision': REVISION,
                          'upstream_path': path, 'content_sha256': fixture['content_sha256'],
                          'metadata': metadata, 'metadata_sha256': bytea(digest)}
            expected[fixture['fixture_id']] = sha(annotation, database_numbers=True)
            source_digest.update((path + ':' + row['sha256'] + '\n').encode())
            metadata_digest.update((path + ':' + digest + '\n').encode())
            spec = metadata['specification']
            states[spec['reference_state']] += 1
            diagnostics += int(bool(spec.get('diagnostic')))
            if path in ('test/built-ins/AggregateError/cause-property.js',
                        'test/language/arguments-object/10.5-1-s.js',
                        'test/built-ins/BigInt/parseInt/nonexistent.js',
                        'test/built-ins/Function/prototype/toString/line-terminator-normalisation-CR.js'):
                samples[path] = {'source_sha256': row['sha256'], 'metadata_sha256': digest,
                                 'raw_identifiers': spec['raw_identifiers'], 'state': spec['reference_state'],
                                 'has_description': bool(metadata['description'])}
        if start % 5000 == 0:
            print('Source preflight:', start + len(batch), 'fixtures', flush=True)
    if source_digest.hexdigest() != SOURCE_MANIFEST or metadata_digest.hexdigest() != args.metadata_digest:
        raise ValueError('Reviewed source/metadata manifest mismatch')
    modern = samples['test/built-ins/AggregateError/cause-property.js']
    legacy = samples['test/language/arguments-object/10.5-1-s.js']
    absent = samples['test/built-ins/BigInt/parseInt/nonexistent.js']
    cr = samples['test/built-ins/Function/prototype/toString/line-terminator-normalisation-CR.js']
    if (modern['raw_identifiers'] != {'esid': 'sec-aggregate-error'} or not modern['has_description'] or
            legacy['raw_identifiers'] != {'es5id': '10.5-1-s'} or legacy['state'] != 'legacy' or
            not legacy['has_description'] or absent['state'] != 'absent' or absent['raw_identifiers'] or
            cr['raw_identifiers'] != {'esid': 'sec-function-definitions-runtime-semantics-instantiatefunctionobject'}):
        raise ValueError('Required source controls failed')
    return expected, {'source_manifest_sha256': source_digest.hexdigest(),
                      'metadata_manifest_sha256': metadata_digest.hexdigest(),
                      'fixtures': len(fixtures), 'reference_states': dict(states),
                      'diagnostics': diagnostics, 'samples': samples}


class CanaryPause(Exception):
    pass


class AnnotationWriter:
    """Restrict the existing enrichment command to preflighted annotation bytes."""
    def __init__(self, client, expected, pause=False):
        self.client, self.expected, self.pause = client, expected, pause

    def one(self, table, **filters):
        return self.client.one(table, **filters)

    def read(self, table, filters):
        return self.client.read(table, filters)

    def put(self, table, rows):
        if (table != 'fixture_specification_metadata' or not 0 < len(rows) <= 250 or
                any(self.expected.get(row['fixture_id']) != sha(row, database_numbers=True) for row in rows)):
            raise ValueError('Write differs from reviewed annotation plan')
        self.client.put(table, rows)
        if self.pause:
            raise CanaryPause('First acknowledged batch retained for resume verification')


def annotations(client, expected):
    result = {}
    print('Verifying annotation fingerprints', flush=True)
    with client.view(), closing(client.export_rows('fixture_specification_metadata')) as rows:
        for row in rows:
            if row['fixture_id'] not in expected:
                continue
            payload = {key: value for key, value in row.items() if key not in ('actor', 'recorded_at')}
            if sha(payload, database_numbers=True) != expected[row['fixture_id']] or not row['actor'] or not row['recorded_at']:
                raise ValueError('Conflicting immutable annotation')
            result[row['fixture_id']] = sha(row)
    print('Verified annotation fingerprints', len(result), 'records', flush=True)
    return result


def run(client, args, report):
    with client.view():
        expected, plan = prepare(client, args)
    existing = annotations(client, expected)
    report.update(plan=plan, existing_annotations=len(existing))
    before = snapshot(client, args.corpus)
    report['before'] = {key: before[key] for key in ('snapshot_token', 'as_of')}
    report['historical_fingerprints'] = retained_fingerprints(before, before)
    if args.mode == 'preflight':
        report.update(verified=True, annotations_written=False)
        return
    # Persist the pre-write receipt even if a later batch fails.
    Path(args.output).write_text(json.dumps(report, sort_keys=True, indent=2) + '\n')
    enrichment_args = SimpleNamespace(corpus=args.corpus, root=args.root)
    if not existing:
        try:
            enrich_specification_metadata(AnnotationWriter(client, expected, pause=True), enrichment_args)
        except CanaryPause:
            pass
        partial = annotations(client, expected)
        if len(partial) != min(250, len(expected)):
            raise ValueError('First acknowledged batch incomplete')
        report['interrupted_batch_annotations'] = len(partial)
    result = enrich_specification_metadata(AnnotationWriter(client, expected), enrichment_args)
    first = annotations(client, expected)
    if result['enriched_fixtures'] != len(expected) or len(first) != len(expected):
        raise ValueError('Incomplete canary enrichment')
    enrich_specification_metadata(AnnotationWriter(client, expected), enrichment_args)
    replay = annotations(client, expected)
    if first != replay:
        raise ValueError('Replay changed annotation identities, actors or timestamps')
    after = snapshot(client, args.corpus)
    proofs = retained_fingerprints(before, after)
    if any(after['rows']['fixture_specification_metadata'].get(key) != digest
           for key, digest in before['rows']['fixture_specification_metadata'].items()):
        raise ValueError('Existing annotations changed')
    if set(after['rows']['fixture_specification_metadata']) - set(before['rows']['fixture_specification_metadata']) != set(expected) - set(existing):
        raise ValueError('Unexpected annotation membership change')
    report.update(verified=True, annotations_written=True, annotated_fixtures=len(replay),
                  newly_annotated_fixtures=len(replay)-len(existing), identical_replay=True,
                  annotation_rows_sha256=sha(replay), historical_fingerprints=proofs,
                  after={key: after[key] for key in ('snapshot_token', 'as_of')})


def main():
    from psycopg.conninfo import conninfo_to_dict
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--corpus', choices=CORPORA, required=True)
    parser.add_argument('--root', required=True)
    parser.add_argument('--mode', choices=('preflight', 'apply'), default='preflight')
    parser.add_argument('--metadata-digest', required=True)
    parser.add_argument('--backup-report', required=True)
    parser.add_argument('--backup-run', required=True, type=int)
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    report = {'verified': False, 'mode': args.mode, 'corpus': args.corpus,
              'source_revision': os.environ.get('GITHUB_SHA'), 'payload_retained': False}
    client = None
    try:
        report['recovery'] = recovery_receipt(args.backup_report, args.backup_run)
        settings = conninfo_to_dict(os.environ['TEST262_DATABASE_URL'])
        sslmode = settings.get('sslmode', 'require')
        if sslmode not in ('require', 'verify-ca', 'verify-full'):
            raise ValueError('TLS required')
        os.environ['PGSSLMODE'] = sslmode
        report['project_reference'] = project_reference(settings)
        if report['project_reference'] != 'nluxctxxzchguuzaalxn':
            raise ValueError('Wrong production project')
        client = Client()
        contract = client.contract
        if (contract['repository_id'] != REPOSITORY or contract['repository_id'] != os.environ['TEST262_REPOSITORY_ID'] or
                contract['producer_id'] != os.environ['TEST262_PRODUCER_ID'] or
                contract['permission'] != 'coordinator' or contract['trust_class'] != 'trusted' or
                contract['deployment_state'] != 'active' or
                client.db.execute('SELECT session_user').fetchone()[0] != 'test262_supervisor'):
            raise ValueError('Wrong trusted coordinator binding')
        report.update(repository_id=REPOSITORY, epoch=client.epoch, upstream_revision=REVISION)
        run(client, args, report)
    except Exception as error:
        report.update(verified=False, error_class=type(error).__name__)
    finally:
        if client:
            client.close()
        Path(args.output).write_text(json.dumps(report, sort_keys=True, indent=2) + '\n')
    print('Specification backfill:', 'PASS' if report['verified'] else 'FAIL', args.mode)
    return 0 if report['verified'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
