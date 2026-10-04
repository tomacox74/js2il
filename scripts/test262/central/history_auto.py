"""Bounded scheduled history selection; immutable API receipts, no dispatch token."""
import argparse
import json
import os
import re
from pathlib import Path
import subprocess
import sys
from types import SimpleNamespace

from .client import Client, sha
from .connection_check import check
from .history_import import download, gh_json, select, digest


def failed_run(repository, current):
    # Paginate: successful blocked/no-op runs must not hide an older failure.
    page = 1
    while True:
        runs = gh_json(f'repos/{repository}/actions/workflows/test262-history-import.yml/runs?event=schedule&per_page=100&page={page}')['workflow_runs']
        for run in runs:
            if str(run['id']) == current:
                continue
            if run['status'] != 'completed' or run['conclusion'] not in ('success', 'skipped'):
                return str(run['id'])
        if len(runs) < 100:
            return None
        page += 1


def pending_rows(rows, receipts):
    verified = {}
    for record in receipts:
        doc = record['document']
        key = doc['source_uri']
        if key in verified and verified[key] != doc:
            raise ValueError('Conflicting verification receipts')
        verified[key] = doc
    pending = []
    for row in rows:
        doc = verified.get(row['source_uri'])
        if doc is None:
            pending.append(row)
        elif (doc['sha256'] != row['sha256'] or doc['archive_sha256'] != row['archive_sha256']):
            raise ValueError('Archived bytes changed since verification')
    return pending


def save_plan(path, plan):
    Path(path).parent.mkdir(parents=True, exist_ok=True)
    Path(path).write_text(json.dumps(plan, indent=2) + '\n')
    with open(os.environ['GITHUB_OUTPUT'], 'a') as stream:
        stream.write('has_pending=' + str(bool(plan.get('selected_artifact_ids'))).lower() + '\n')
        stream.write('artifact_ids=' + ','.join(map(str, plan.get('selected_artifact_ids', []))) + '\n')


def plan(args):
    if not 1 <= args.max_snapshots <= 5:
        raise ValueError('Automatic batch size must be 1–5')
    if not args.archive_run_id.isdigit() or int(args.archive_run_id) < 1:
        raise ValueError('Configure an attributed discovery run')
    blocked = failed_run(args.repository_name, os.environ['GITHUB_RUN_ID'])
    if blocked and os.getenv('TEST262_HISTORY_AUTO_RESUME_RUN_ID') != blocked:
        save_plan(args.output, {'state': 'blocked', 'failed_run_id': blocked,
                  'next_action': 'Investigate recovery evidence, then acknowledge this run with TEST262_HISTORY_AUTO_RESUME_RUN_ID.'})
        return
    check('test262_importer', 'legacy')
    from psycopg.conninfo import conninfo_to_dict, make_conninfo
    dsn = os.environ['TEST262_DATABASE_URL']
    client = Client(dsn=make_conninfo(dsn, sslmode=conninfo_to_dict(dsn).get('sslmode', 'require'), connect_timeout=20))
    try:
        if (client.contract['repository_id'] != os.environ['TEST262_REPOSITORY_ID'] or
            client.contract['producer_id'] != os.environ['TEST262_PRODUCER_ID'] or
            client.contract['trust_class'] != 'legacy' or
            client.contract['deployment_state'] not in ('schema-only', 'shadow')):
            raise ValueError('Automatic importer scope/state mismatch')
        manifest_hash = os.environ.get('TEST262_HISTORY_MANIFEST_SHA256', '')
        if not re.fullmatch('[0-9a-f]{64}', manifest_hash):
            raise ValueError('Configure the preserved manifest SHA-256')
        scope = {'archive_run_id': args.archive_run_id, 'kind': args.kind, 'manifest_sha256': manifest_hash}
        complete = list(client.read('legacy_control_records', {'source_table': 'history_auto_complete', 'source_key': sha(scope)}))
        if complete:
            save_plan(args.output, dict(scope, state='verified-selection-complete', selected_artifact_ids=[], history_complete=False))
            return
        directory = Path(args.directory)
        download(SimpleNamespace(run_id=args.archive_run_id, repository_name=args.repository_name, output=str(directory)))
        if digest(directory / 'history-manifest.json') != manifest_hash:
            raise ValueError('Discovery manifest changed')
        _, rows, overflow = select(directory, args.repository_name, args.kind, '', 100, verify_sources=False)
        if overflow:
            raise ValueError('Archive exceeds automatic selection capacity; split the archive before proceeding')
        receipts = list(client.read('legacy_control_records', {'source_table': 'history_verified_snapshot'}))
        pending = pending_rows(rows, receipts)
        if not pending:
            if not receipts:
                raise ValueError('Completion requires verification receipts')
            client.put('legacy_control_records', [{'import_id': receipts[0]['import_id'],
                'source_table': 'history_auto_complete', 'source_key': sha(scope),
                'document': dict(scope, verified_snapshots=len(rows), history_complete=False)}])
        save_plan(args.output, dict(scope, state='ready' if pending else 'verified-selection-complete',
                  selected_artifact_ids=[r['artifact_id'] for r in pending[:args.max_snapshots]],
                  pending_snapshots=len(pending), verified_snapshots=len(rows)-len(pending), history_complete=False))
    finally:
        client.close()


def report(args):
    directory = Path(args.directory)
    plan_path, report_path = directory/'auto-plan.json', directory/'import-report.json'
    plan_data = json.loads(plan_path.read_text()) if plan_path.exists() else {}
    data = json.loads(report_path.read_text()) if report_path.exists() else {}
    state = 'failure' if args.outcome != 'success' else ('batch-verified' if data.get('complete') else plan_data.get('state', 'manual'))
    run = f'https://github.com/{args.repository_name}/actions/runs/{os.environ["GITHUB_RUN_ID"]}'
    lines = [f'Historical import controller: **{state}**', '', f'Run: {run} (attempt {os.environ["GITHUB_RUN_ATTEMPT"]}).']
    if data:
        verified = [r for r in data['snapshots'] if r['state'] == 'verified']
        lines.append(f'Report: {len(verified)} verified snapshots; selected IDs {data.get("selected_artifact_ids", [])}; invocation complete={data["complete"]}; history_complete=false.')
        for row in verified:
            lines.append(f'- Artifact {row["artifact_id"]}: {row["verified_observation_mappings"]} verified observation mappings.')
    if state == 'failure':
        lines.append('Automatic imports stop behind this failed run. Inspect recovery evidence before setting TEST262_HISTORY_AUTO_RESUME_RUN_ID to this run ID.')
    elif state == 'blocked':
        lines.append('Blocked by run ' + plan_data['failed_run_id'] + '; investigate before acknowledging it.')
    elif state == 'verified-selection-complete':
        lines.append('All snapshots of the configured kind in this archive have verification receipts. Missing history and other kinds remain separate; pause automatic imports or select the next kind/archive.')
    else:
        lines.append('Next scheduled run selects the next unverified bounded batch. Inspect the recovery artifact for full hash/parity evidence.')
    lines.append('Recovery artifact: test262-history-import-recovery-' + os.environ['GITHUB_RUN_ID'] + '-' + os.environ['GITHUB_RUN_ATTEMPT'] + '. Keep independent retention.')
    text = '\n'.join(lines) + '\n'
    with open(os.environ['GITHUB_STEP_SUMMARY'], 'a') as stream:
        stream.write(text)
    # Avoid unchanged blocked/completion comments; first transition still reports.
    marker = '<!-- test262-auto-' + sha([state, plan_data.get('failed_run_id'), plan_data.get('archive_run_id'), plan_data.get('kind')]) + ' -->'
    comments = json.loads(subprocess.check_output(['gh', 'api', '--paginate', '--slurp', f'repos/{args.repository_name}/issues/{args.issue}/comments?per_page=100'], text=True))
    if state in ('blocked', 'verified-selection-complete') and any(marker in c['body'] for page in comments for c in page):
        return
    body = directory/'issue-update.md'
    directory.mkdir(parents=True, exist_ok=True)
    if state == 'blocked':
        return  # The original failed run already records the actionable milestone.
    body.write_text(marker + '\n' + text)
    subprocess.run(['gh', 'issue', 'comment', str(args.issue), '--repo', args.repository_name, '--body-file', str(body)], check=True)


if __name__ == '__main__':
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest='command', required=True)
    q = sub.add_parser('plan')
    q.add_argument('--archive-run-id', required=True)
    q.add_argument('--kind', choices=('mvp','native','all'), default='mvp')
    q.add_argument('--max-snapshots', type=int, default=2)
    q.add_argument('--output', required=True)
    q.add_argument('--directory', required=True)
    q.add_argument('--repository-name', required=True)
    r = sub.add_parser('report')
    r.add_argument('--directory', required=True)
    r.add_argument('--repository-name', required=True)
    r.add_argument('--issue', type=int, default=2242)
    r.add_argument('--outcome', required=True)
    args = p.parse_args()
    try:
        plan(args) if args.command == 'plan' else report(args)
    except Exception as error:
        print('Automatic history operation failed (' + type(error).__name__ + '). Inspect recovery evidence.', file=sys.stderr)
        raise SystemExit(1)
