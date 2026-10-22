"""Read-only deployment checks. Retain summaries, never DSNs or export payloads."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import tempfile

from .client import Client, EXPORT_TABLES

# Offline verification of the complete catalogue; page and statement bounds stay in Client.
COORDINATOR_EXPORT_SECONDS = 30 * 60


def project_reference(settings):
    candidates = set()
    for host in settings.get('host', '').split(','):
        match = re.fullmatch(r'db\.([a-z0-9]{20})\.supabase\.co', host)
        if match:
            candidates.add(match[1])
    match = re.fullmatch(r'test262_(?:supervisor|agent_ro)\.([a-z0-9]{20})', settings.get('user', ''))
    if match:
        candidates.add(match[1])
    if len(candidates) != 1:
        raise ValueError('Supabase project reference is missing or inconsistent')
    return candidates.pop()


def verify_export(path, expected):
    """Independently stream-check bytes, counts and the single-snapshot receipt."""
    digest = hashlib.sha256()
    counts = dict.fromkeys(EXPORT_TABLES, 0)
    header = footer = None
    with Path(path).open('rb') as stream:
        for number, line in enumerate(stream):
            record = json.loads(line)
            if number == 0:
                if set(record) != {'header'}:
                    raise ValueError('Missing export header')
                header = record['header']
            elif set(record) == {'footer'}:
                footer = record['footer']
                if stream.read(1):
                    raise ValueError('Data after export footer')
                break
            else:
                if set(record) != {'table', 'row'} or record['table'] not in counts:
                    raise ValueError('Unexpected export record')
                counts[record['table']] += 1
            digest.update(line)
    if not header or not footer:
        raise ValueError('Incomplete export')
    if any(header.get(key) != expected[key] for key in ('repository_id', 'epoch', 'schema', 'api')):
        raise ValueError('Wrong export identity')
    if not header.get('snapshot_token') or not header.get('as_of'):
        raise ValueError('Missing snapshot identity')
    if footer != {'counts': counts, 'sha256': digest.hexdigest()}:
        raise ValueError('Export count or digest mismatch')
    return dict(header, **footer)


def coordinator(dsn, settings):
    client = Client(dsn, epoch=int(os.environ['AUTHORITY_EPOCH']))
    try:
        contract = client.contract
        expected = {'repository_id': os.environ['REPOSITORY_ID'], 'epoch': client.epoch, 'schema': 1, 'api': 1}
        if (contract['repository_id'] != expected['repository_id'] or
                contract['producer_id'] != os.environ['PRODUCER_ID'] or
                contract['permission'] != 'coordinator' or contract['trust_class'] != 'trusted' or
                contract['deployment_state'] != 'active'):
            raise ValueError('Unexpected coordinator contract')
        with client.view():
            login = client.db.execute('SELECT session_user').fetchone()[0]
            if login != 'test262_supervisor':
                raise ValueError('Wrong coordinator login')
            result = client.db.execute('SELECT test262.api_read(%s,%s,%s,NULL,3)',
                                      (client.epoch, 'fixture_specification_metadata', client.Jsonb({}))).fetchone()[0]
            if len(result['rows']) > 3:
                raise ValueError('Read exceeded page bound')
            raw_write = client.db.execute("SELECT has_table_privilege(current_user, c.oid, 'INSERT') FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='test262' AND c.relname='fixture_specification_metadata'").fetchone()[0]
            if raw_write:
                raise ValueError('Coordinator has raw INSERT permission')
            foreign = client.db.execute('SELECT test262.api_read(%s,%s,%s,NULL,3)',
                                       (client.epoch, 'repository_corpora', client.Jsonb({'repository_id': '00000000-0000-0000-0000-000000000000'}))).fetchone()[0]
            if foreign['rows']:
                raise ValueError('Cross-repository rows exposed')
        with tempfile.TemporaryDirectory(prefix='test262-verification-') as temporary:
            path = Path(temporary) / 'snapshot.ndjson'
            exported = client.export(path, seconds=COORDINATOR_EXPORT_SECONDS)
            receipt = verify_export(path, expected)
            if receipt != exported:
                raise ValueError('Exporter receipt mismatch')
        return {'verified': True, 'project_reference': project_reference(settings),
                'bounded_annotation_rows': len(result['rows']), 'raw_insert_denied': True,
                'foreign_repository_read_empty': True, 'export': receipt,
                'payload_retained': False}
    finally:
        client.close()


def reporter(dsn, settings):
    import psycopg
    with psycopg.connect(dsn, sslmode=settings.get('sslmode', 'require'), connect_timeout=20, autocommit=True) as db:
        db.execute('BEGIN READ ONLY')
        db.execute("SET LOCAL statement_timeout='30s'")
        if db.execute('SELECT session_user').fetchone()[0] != 'test262_agent_ro':
            raise ValueError('Wrong reporter login')
        rows = db.execute('SELECT repository_id, upstream_revision, upstream_path, content_sha256, metadata_source, reference_state, mapping_status, specification_edition, interpreted_clause FROM test262_reporting.v_fixture_specification_references WHERE repository_id=%s LIMIT 3', (os.environ['REPOSITORY_ID'],)).fetchall()
        if not rows:
            raise ValueError('No reporting rows')
        raw_write = db.execute("SELECT has_table_privilege(current_user, c.oid, 'INSERT') FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='test262' AND c.relname='fixture_specification_metadata'").fetchone()[0]
        if raw_write:
            raise ValueError('Reporter has raw INSERT permission')
        db.execute('ROLLBACK')
    return {'verified': True, 'project_reference': project_reference(settings),
            'bounded_view_rows': len(rows), 'raw_insert_denied': True}


def main():
    from psycopg.conninfo import conninfo_to_dict
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('identity', choices=('coordinator', 'reporter'))
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    output = Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    report = {'identity': args.identity, 'verified': False, 'source_revision': os.getenv('GITHUB_SHA')}
    if args.identity == 'coordinator':
        report['export_budget_seconds'] = COORDINATOR_EXPORT_SECONDS
    try:
        dsn = os.environ.get('DATABASE_URL')
        if not dsn:
            raise ValueError('Missing dedicated database credential')
        settings = conninfo_to_dict(dsn)
        if settings.get('sslmode', 'require') not in ('require', 'verify-ca', 'verify-full'):
            raise ValueError('TLS required')
        # libpq uses require if the secret omits sslmode; do not log connection settings.
        os.environ['PGSSLMODE'] = settings.get('sslmode', 'require')
        project_reference(settings)
        report.update(globals()[args.identity](dsn, settings))
    except Exception as error:
        # Database exceptions can echo connection strings or source rows.
        report['error_class'] = type(error).__name__
    output.write_text(json.dumps(report, sort_keys=True, indent=2) + '\n')
    print('Specification deployment verification:', 'PASS' if report['verified'] else 'FAIL', args.identity)
    return 0 if report['verified'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
