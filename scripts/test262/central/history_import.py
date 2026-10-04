"""Import bounded selections from an attributed, previously archived history run."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
from types import SimpleNamespace

from .connection_check import check
from .importer import import_snapshot
from .client import Client

WORKFLOW = '.github/workflows/test262-history-import.yml'
SOURCE_WORKFLOWS = ('test262-catalog.yml', 'test262-native-port.yml')


def gh_json(endpoint):
    return json.loads(subprocess.check_output(['gh', 'api', endpoint], text=True))


def verify_run(run, repository):
    if (run['head_repository']['full_name'] != repository or run['head_branch'] != 'master'
            or run['path'] != WORKFLOW or run['event'] != 'workflow_dispatch'
            or run['status'] != 'completed' or run['conclusion'] != 'success'):
        raise ValueError('Archive must come from a successful master discovery workflow')


def download(args):
    if not re.fullmatch(r'[1-9][0-9]*', args.run_id):
        raise ValueError('A discovery run ID is required')
    run = gh_json(f'repos/{args.repository_name}/actions/runs/{args.run_id}')
    verify_run(run, args.repository_name)
    artifacts = gh_json(f'repos/{args.repository_name}/actions/runs/{args.run_id}/artifacts?per_page=100')['artifacts']
    name = 'test262-history-archive-' + args.run_id
    matches = [a for a in artifacts if a['name'] == name and not a['expired']]
    if len(matches) != 1:
        raise ValueError('Discovery archive is missing, expired or ambiguous')
    output = Path(args.output)
    if output.exists():
        raise ValueError('Archive download requires a new directory')
    subprocess.run(['gh', 'run', 'download', args.run_id, '--repo', args.repository_name,
                    '--name', name, '--dir', str(output)], check=True)
    if not (output / 'history-manifest.json').is_file():
        raise ValueError('Missing history manifest')
    print('Downloaded attributed discovery archive from run ' + args.run_id)


def digest(path):
    h = hashlib.sha256()
    with path.open('rb') as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()


def select(directory, repository, kind, artifact_ids, limit):
    if not 1 <= limit <= 100:
        raise ValueError('max_snapshots must be between 1 and 100')
    requested = set()
    if artifact_ids:
        for value in artifact_ids.split(','):
            value = value.strip()
            if not re.fullmatch(r'[1-9][0-9]*', value):
                raise ValueError('Artifact IDs must be comma-separated positive integers')
            requested.add(int(value))
    manifest = json.loads((directory / 'history-manifest.json').read_text())
    if manifest['repository'] != repository:
        raise ValueError('History manifest repository mismatch')
    rows = []
    found = set()
    for artifact in manifest['artifacts']:
        aid = artifact['artifact_id']
        if not isinstance(aid, int) or aid <= 0 or artifact['workflow'] not in SOURCE_WORKFLOWS:
            raise ValueError('Invalid original artifact attribution')
        if requested and aid not in requested:
            continue
        for database in artifact['databases']:
            filename = Path(database['path']).name
            expected_name = 'catalog.sqlite' if artifact['workflow'] == 'test262-catalog.yml' else 'native.sqlite'
            if filename != expected_name:
                raise ValueError('Catalogue does not match attributed workflow')
            if kind != 'all' and filename != ('catalog.sqlite' if kind == 'mvp' else 'native.sqlite'):
                continue
            source_uri = f'github-actions:{repository}/{artifact["workflow"]}/{artifact["run_id"]}/{aid}'
            if database['source_uri'] != source_uri:
                raise ValueError('Source URI attribution mismatch')
            parent = directory / str(aid)
            archive, source = parent / 'source.zip', parent / filename
            # Reject redirected files; paths are reconstructed, never supplied by the manifest.
            for path in (archive, source):
                if path.is_symlink() or path.resolve().parent != parent.resolve() or parent.is_symlink():
                    raise ValueError('Unsafe archive path')
            rows.append({'artifact_id': aid, 'source': str(source), 'source_uri': source_uri,
                         'sha256': database['sha256'], 'archive': str(archive),
                         'archive_sha256': artifact['archive_sha256']})
            found.add(aid)
    if requested - found:
        raise ValueError('Selected artifact IDs have no matching recoverable database')
    if not rows:
        raise ValueError('No matching snapshots; inspect the discovery manifest')
    if requested and len(rows) > limit:
        raise ValueError('Explicit selection exceeds max_snapshots; split it into smaller runs')
    selected = rows[:limit]
    # Verify every selected source before any import write begins.
    for row in selected:
        if digest(Path(row['archive'])) != row['archive_sha256'] or digest(Path(row['source'])) != row['sha256']:
            raise ValueError('Archived source checksum mismatch')
    return manifest, selected, len(rows) - len(selected)


def import_history(args):
    directory, output = Path(args.directory), Path(args.output)
    output.mkdir(parents=True, exist_ok=True)
    report = {'snapshots': [], 'complete': False, 'history_complete': False}
    report_path = output / 'import-report.json'

    def save():
        report_path.write_text(json.dumps(report, indent=2) + '\n')

    save()
    try:
        manifest, selected, remaining = select(directory, args.repository_name, args.kind,
                                                args.artifact_ids, args.max_snapshots)
        report.update(selected_artifact_ids=[r['artifact_id'] for r in selected],
                      remaining_snapshots=remaining, gaps=manifest['gaps'],
                      manifest_sha256=digest(directory / 'history-manifest.json'))
        save()
        check('test262_importer', 'legacy')
        # Pass an explicit TLS DSN to the ordinary importer client as well.
        from psycopg.conninfo import conninfo_to_dict, make_conninfo
        dsn = os.environ['TEST262_DATABASE_URL']
        dsn = make_conninfo(dsn, sslmode=conninfo_to_dict(dsn).get('sslmode', 'require'), connect_timeout=20)
        client = Client(dsn=dsn)
        try:
            if client.contract['deployment_state'] not in ('schema-only', 'shadow'):
                raise ValueError('Historical import requires inactive authority')
            if (client.contract['repository_id'] != os.environ['TEST262_REPOSITORY_ID']
                    or client.contract['producer_id'] != os.environ['TEST262_PRODUCER_ID']
                    or client.contract['trust_class'] != 'legacy'):
                raise ValueError('Importer scope changed after connection check')
            for row in selected:
                snapshot = dict(row, state='uploading')
                report['snapshots'].append(snapshot)
                save()
                item = SimpleNamespace(source=row['source'], source_uri=row['source_uri'],
                                       repository=os.environ['TEST262_REPOSITORY_ID'],
                                       producer=os.environ['TEST262_PRODUCER_ID'], root=os.getenv('TEST262_ROOT'),
                                       missing_artifacts=['See archived discovery manifest sha256:' + report['manifest_sha256'],
                                                         json.dumps(manifest['gaps'], separators=(',', ':'))],
                                       outbox=str(output / (str(row['artifact_id']) + '-outbox.sqlite')))
                result = import_snapshot(client, item)
                # Independent central mapping/observation coverage check; reads are paginated.
                mappings = {r['entity_id'] for r in client.read('import_records', {'import_id': result['import_id']})
                            if r['entity_kind'] == 'observation'}
                missing = set(mappings)
                for observation in client.read('observations'):
                    missing.discard(observation['observation_id'])
                if len(mappings) != result['observations'] or missing:
                    raise ValueError('Central observation mapping parity failed')
                if digest(Path(row['source'])) != row['sha256']:
                    raise ValueError('Original snapshot changed during import')
                snapshot.update(state='verified', result=result, verified_observation_mappings=len(mappings))
                save()
            report['complete'] = True
            save()
        finally:
            client.close()
    except Exception as error:
        report['error_type'] = type(error).__name__
        save()
        raise
    print(json.dumps({'verified_snapshots': len(selected), 'remaining_snapshots': remaining,
                      'history_complete': False, 'report': str(report_path)}))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest='command', required=True)
    d = sub.add_parser('download')
    d.add_argument('--run-id', required=True)
    d.add_argument('--repository-name', required=True)
    d.add_argument('--output', required=True)
    i = sub.add_parser('import')
    i.add_argument('--directory', required=True)
    i.add_argument('--repository-name', required=True)
    i.add_argument('--kind', choices=('mvp', 'native', 'all'), default='mvp')
    i.add_argument('--artifact-ids', default='')
    i.add_argument('--max-snapshots', type=int, default=5)
    i.add_argument('--output', required=True)
    args = parser.parse_args()
    try:
        download(args) if args.command == 'download' else import_history(args)
    except Exception as error:
        print('Historical import operation failed (' + type(error).__name__ + '). Inspect discovery manifest and recovery report.', file=sys.stderr)
        raise SystemExit(1)
