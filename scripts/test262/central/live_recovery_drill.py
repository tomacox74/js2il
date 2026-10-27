"""One owner-authorized live recovery campaign; never publishes or changes targets."""
import argparse
from datetime import datetime, timezone
import hashlib
import io
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
from types import SimpleNamespace
import zipfile

from . import commands, lifecycle
from .client import Client, Outbox, bytea, canonical, identity, sha
from .diagnostics import report_failure
from .execution_identity import execution_identity
from .identity_diagnostics import compare
from .importer import provenance
from .inventory import REPO, normalize_fixture
from .monitoring import github
from .worker import stage_runtime, work_staged

REPOSITORY = 'tomacox74/js2il'
AUTHORIZATION = 'live-recovery-2340-two-variants-20261027'
WORKFLOW = '.github/workflows/test262-recovery-drill.yml'


def require(condition, message):
    if not condition:
        raise ValueError(message)


def checkpoint(args, report):
    path = Path(args.output)
    path.parent.mkdir(parents=True, exist_ok=True)
    pending = path.with_name(path.name + '.pending')
    with pending.open('w') as stream:
        json.dump(report, stream, indent=2)
        stream.write('\n')
        stream.flush()
        os.fsync(stream.fileno())
    pending.replace(path)


def image_id(args):
    value = subprocess.check_output(
        ['docker', 'image', 'inspect', args.image, '--format', '{{.Id}}'],
        text=True).strip()
    require(re.fullmatch(r'sha256:[a-f0-9]{64}', value), 'Immutable fixture image identity required')
    return value


def check_invocation(args):
    require(args.authorization == AUTHORIZATION, 'Explicit bounded live authorization required')
    require(os.getenv('GITHUB_REPOSITORY') == REPOSITORY
            and os.getenv('GITHUB_EVENT_NAME') == 'workflow_dispatch'
            and os.getenv('GITHUB_REF') == 'refs/heads/master'
            and os.getenv('GITHUB_RUN_ATTEMPT') == '1',
            'Only the protected manual master workflow first attempt is approved')
    source = os.environ.get('GITHUB_SHA', '')
    run_id = os.environ.get('GITHUB_RUN_ID', '')
    require(re.fullmatch(r'[a-f0-9]{40}', source)
            and re.fullmatch(r'[1-9][0-9]*', run_id), 'Exact Actions source/run required')
    require(commands.git('rev-parse', 'HEAD') == source
            and not commands.git('status', '--porcelain', '--untracked-files=no')
            and commands.current_master(REPOSITORY) == source,
            'The exact clean workflow source must still be current master')
    run = github(f'repos/{REPOSITORY}/actions/runs/{run_id}/attempts/1')
    require(run['id'] == int(run_id) and run['run_attempt'] == 1
            and run['repository']['full_name'] == REPOSITORY
            and run['head_repository']['full_name'] == REPOSITORY
            and run['head_sha'] == source and run['head_branch'] == 'master'
            and run['path'] == WORKFLOW and run['event'] == 'workflow_dispatch'
            and run['status'] == 'in_progress', 'Authenticated workflow identity differs')
    args.revision = source
    return {key: run[key] for key in
            ('id', 'run_attempt', 'head_sha', 'head_branch', 'event', 'path', 'html_url')}


def fetch_report(run_id, source):
    require(type(run_id) is int and run_id > 0, 'Exact preceding Actions run required')
    root = f'repos/{REPOSITORY}/actions'
    run = github(f'{root}/runs/{run_id}/attempts/1')
    require(run['id'] == run_id and run['run_attempt'] == 1
            and run['head_sha'] == source and run['head_branch'] == 'master'
            and run['repository']['full_name'] == REPOSITORY
            and run['head_repository']['full_name'] == REPOSITORY
            and run['path'] == WORKFLOW and run['event'] == 'workflow_dispatch'
            and run['status'] == 'completed' and run['conclusion'] == 'success',
            'Preceding workflow source/scope/outcome differs')
    artifacts = github(f'{root}/runs/{run_id}/artifacts?per_page=100')['artifacts']
    selected = [row for row in artifacts
                if row['name'] == f'test262-recovery-drill-{run_id}-1']
    require(len(selected) == 1, 'Exactly one authenticated preceding artifact required')
    meta = selected[0]
    require(meta['expired'] is False and 0 < meta['size_in_bytes'] <= 2_000_000
            and meta['workflow_run']['id'] == run_id
            and meta['workflow_run']['head_sha'] == source, 'Preceding artifact metadata differs')
    raw = github(f"{root}/artifacts/{meta['id']}/zip", binary=True)
    digest = hashlib.sha256(raw).hexdigest()
    require(len(raw) == meta['size_in_bytes'] and meta['digest'] == 'sha256:' + digest,
            'Preceding artifact digest differs')
    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
        members = [member for member in archive.infolist() if member.filename == 'report.json']
        require(len(members) == 1 and members[0].file_size <= 1_000_000,
                'Exactly one bounded preceding report required')
        encoded = archive.read(members[0])
    report = json.loads(encoded)
    return report, {'run_id': run_id, 'run_attempt': 1, 'artifact_id': meta['id'],
                    'zip_sha256': digest, 'report_sha256': hashlib.sha256(encoded).hexdigest()}


def fetch_previous(run_id, source):
    report, receipt = fetch_report(run_id, source)
    require(report['schema_version'] == 1 and report['repository'] == REPOSITORY
            and report['campaign'] == AUTHORIZATION and report['phase'] == 'exhaust'
            and report['phase_complete'] is True and report['complete'] is False
            and report['stage'] == 'exhaustion-verified'
            and report['source_revision'] == source
            and report['actions']['id'] == run_id and report['actions']['run_attempt'] == 1
            and report['actions']['head_sha'] == source
            and type(report['actual_fixture_executions']) is int
            and report['actual_fixture_executions'] == 1
            and re.fullmatch('[a-f0-9]{64}', report['first_observation_sha256'])
            and len(report['execution_intents']) == len(report['invocations']) == 1
            and report['invocations'][0]['budget']['state'] == 'exhausted'
            and report['invocations'][0]['budget']['charged_attempts'] == 1
            and report['invocations'][0]['worker']['attempts'] == 1
            and report['invocations'][0]['worker']['stop_reason'] == 'budget-exhausted'
            and report['control_plane_unchanged'] is True
            and all(report[key] is False for key in
                    ('publication', 'authority_changed', 'permissions_changed',
                     's3_access', 'imports_or_restores', 'cron_witness')),
            'Preceding report is not a verified single-execution exhaustion')
    return report, receipt


def fetch_baseline(run_id, source):
    report, receipt = fetch_report(run_id, source)
    require(report['schema_version'] == 1 and report['repository'] == REPOSITORY
            and report['campaign'] == AUTHORIZATION and report['phase'] == 'verify'
            and report['phase_complete'] is True and report['complete'] is False
            and report['stage'] == 'baseline-verified' and report['baseline_read_only'] is True
            and report['source_revision'] == source
            and report['actions']['id'] == run_id and report['actions']['run_attempt'] == 1
            and report['actions']['head_sha'] == source
            and type(report['actual_fixture_executions']) is int
            and report['actual_fixture_executions'] == 0
            and report['execution_intents'] == report['invocations'] == []
            and report['control_plane_unchanged'] is True
            and report['preflight_comparison']['matches'] is True
            and report['preflight_comparison']['expected_sha256']
            == report['preflight_comparison']['actual_sha256']
            == report['native_execution_identity_sha256']
            and report['sealed_upstream_matches'] is True
            and report['required_variants'] == ['non-strict', 'strict']
            and re.fullmatch('[a-f0-9]{64}', report['image_archive_sha256'])
            and re.fullmatch('sha256:[a-f0-9]{64}', report['fixture_image_id'])
            and all(report[key] is False for key in
                    ('publication', 'authority_changed', 'permissions_changed',
                     's3_access', 'imports_or_restores', 'cron_witness')),
            'Baseline is not an authenticated zero-execution frozen identity')
    return report, receipt


def image_archive_sha256(args):
    digest = hashlib.sha256()
    with Path(args.image_archive).open('rb') as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            digest.update(chunk)
    return digest.hexdigest()


def restore_image(args):
    baseline = args.baseline_report
    require(image_archive_sha256(args) == baseline['image_archive_sha256'],
            'Frozen image archive differs; do not load or execute it')
    subprocess.run(['docker', 'image', 'load', '--input', args.image_archive],
                   check=True, stdout=subprocess.DEVNULL, stderr=subprocess.PIPE)
    require(image_id(args) == baseline['fixture_image_id'],
            'Loaded image differs from the frozen baseline')
    return {'archive_sha256': baseline['image_archive_sha256'],
            'image_id': baseline['fixture_image_id']}


def control_plane(client, args):
    contract = client.db.execute('SELECT test262.api_contract()').fetchone()[0]
    require(contract['repository_id'] == args.repository
            and contract['producer_id'] == args.producer
            and contract['permission'] == 'coordinator' and contract['trust_class'] == 'trusted'
            and contract['deployment_state'] == 'active'
            and contract['minimum_writer_epoch'] == client.epoch,
            'Approved active repository-bound coordinator required')
    return {
        'authority': {key: contract[key] for key in
                      ('repository_id', 'producer_id', 'permission', 'trust_class',
                       'deployment_state', 'minimum_writer_epoch')},
        'native_target': client.one('reporting_targets', channel='master', evidence_kind='native'),
        'mvp_target': client.one('reporting_targets', channel='master', evidence_kind='mvp-composite'),
        'reconciliation': client.one('reconciliation_state', pipeline='native', channel='master'),
    }


def check_identity(args, corpus, expected, expected_image):
    require(commands.git('rev-parse', 'HEAD') == args.revision
            and not commands.git('status', '--porcelain', '--untracked-files=no')
            and commands.current_master(REPOSITORY) == args.revision,
            'Source changed; no further drill execution is allowed')
    upstream = subprocess.check_output(
        ['git', '-C', args.root, 'rev-parse', 'HEAD'], text=True).strip()
    dirty = subprocess.check_output(
        ['git', '-C', args.root, 'status', '--porcelain', '--untracked-files=no'], text=True).strip()
    require(upstream == expected['upstream']['commit'] and not dirty,
            'Pinned upstream differs or has tracked changes')
    require(execution_identity(args, corpus) == expected and image_id(args) == expected_image,
            'Execution identity/image changed; no further drill execution is allowed')


def select_fixture(client, args, target, base, *, fixture_id=None):
    snapshot = client.one('registration_snapshots', snapshot_id=target['registration_snapshot_id'])
    require(snapshot and snapshot['state'] == 'sealed'
            and snapshot['source_revision'] == args.revision,
            'Current sealed master registrations required')
    registered = {row['upstream_path'] for row in
                  client.read('registrations', {'snapshot_id': snapshot['snapshot_id']})}
    filters = {'provenance_id': target['provenance_id'], 'eligibility': 'runnable'}
    if fixture_id is not None:
        filters['fixture_id'] = fixture_id
    for eligible in client.read('provenance_fixture_eligibility', filters):
        fixture = client.one('fixtures', fixture_id=eligible['fixture_id'])
        if (fixture['upstream_path'] in registered or fixture['is_support_file']
                or fixture['metadata_state'] != 'valid' or fixture['corpus_id'] != base['inventory']):
            continue
        variants = list(client.read('fixture_variants', {'fixture_id': fixture['fixture_id']}))
        if (len(variants) != 2 or not all(row['required'] for row in variants)
                or sorted(row['variant'] for row in variants) != ['non-strict', 'strict']):
            continue
        original, = commands.catalog.bridge(
            {'command': 'metadata', 'root': args.root, 'paths': [fixture['upstream_path']]})
        row = normalize_fixture(original, args.root)
        state, reasons = commands.fixture_eligibility('native', original, row, base['capabilities'])
        require(original['path'] == fixture['upstream_path']
                and bytea(row['sha256']) == fixture['content_sha256']
                and bytea(row['dependency_manifest_digest']) == fixture['dependency_manifest_digest']
                and row['metadata'] == fixture['metadata'],
                'Sealed fixture metadata/content/dependencies differ')
        if state != 'runnable':
            continue
        require(sorted(v['variant'] for v in row['variants']) == ['non-strict', 'strict'],
                'Local required variants differ')
        return fixture, row, reasons
    raise ValueError('No eligible unregistered sealed two-variant fixture; no writes performed')


def observations(client, pid):
    return list(client.read('observations', {'provenance_id': pid}))


def outbox_proof(args):
    outbox = Outbox(args.outbox, args.repository, args.producer, args.epoch)
    try:
        require(outbox.db.execute('PRAGMA integrity_check').fetchall() == [('ok',)],
                'Drill outbox integrity differs')
        pending = {table: outbox.db.execute(
            f'SELECT count(*) FROM {table} WHERE receipt IS NULL').fetchone()[0]
            for table in ('messages', 'completions')}
        require(not any(pending.values()), 'Unacknowledged drill evidence remains')
        return {'integrity': 'ok', 'pending_messages': pending['messages'],
                'pending_completions': pending['completions']}
    finally:
        outbox.db.close()


class GuardedClaims:
    def __init__(self, client, before_claim):
        self.client, self.before_claim = client, before_claim

    def __getattr__(self, name):
        return getattr(self.client, name)

    def call(self, operation, *values):
        if operation == 'claim':
            with self.client.db.transaction():
                self.before_claim()
                return self.client.call(operation, *values)
        return self.client.call(operation, *values)


def run(client, args, report):
    require(args.phase in ('verify', 'exhaust', 'recover'), 'Explicit bounded phase required')
    previous = getattr(args, 'previous_report', None)
    require((args.phase in ('verify', 'exhaust') and previous is None)
            or (args.phase == 'recover' and previous is not None),
            'Recovery requires authenticated preceding evidence')
    index = 1 if args.phase == 'recover' else 0
    args.kind = 'native'
    args.repository_name = REPOSITORY
    args.cap_ms = 120000
    args.runtime_timeout = 30
    args.compile_timeout = 60
    args.compilation_coverage = False
    args.candidate_limit = args.accepted_limit = args.attempt_limit = 1
    args.budget_ms = 1200000
    args.limit, args.seconds, args.area = 2, 600, ''
    args.epoch = client.epoch
    report.update(schema_version=1, repository=REPOSITORY, campaign=AUTHORIZATION, phase=args.phase,
                  complete=False, stage='preflight', fixture_executions_maximum=2,
                  execution_intents=[], invocations=[], publication=False,
                  authority_changed=False, permissions_changed=False, s3_access=False,
                  imports_or_restores=False, cron_witness=False)
    if previous is not None:
        report['execution_intents'] = list(previous['execution_intents'])
        report['invocations'] = list(previous['invocations'])
        report['first_observation'] = previous['first_observation']
        report['first_observation_sha256'] = previous['first_observation_sha256']
    checkpoint(args, report)
    before = control_plane(client, args)
    target = before['native_target']
    require(target, 'Existing native reporting target required')
    current = client.one('provenances', provenance_id=target['provenance_id'])
    corpus = current['corpus_id']
    inventory = client.one('corpora', corpus_id=corpus)
    require(inventory and inventory['inventory_state'] == 'sealed',
            'Existing sealed inventory required; the drill never imports or registers a corpus')
    base = execution_identity(args, corpus)
    report['preflight_comparison'] = compare(current['identity_document'], base)
    report['sealed_upstream_matches'] = (
        inventory['revision'] == base['upstream']['commit']
        and inventory['upstream_url'] == base['upstream']['cloneUrl'])
    checkpoint(args, report)
    require(report['preflight_comparison']['matches'] and report['sealed_upstream_matches'],
            'Current native target and built execution identity differ')
    validation = commands.github_validation(REPOSITORY, args.revision)
    frozen_image = image_id(args)
    args.image = frozen_image
    check_identity(args, corpus, base, frozen_image)
    baseline = getattr(args, 'baseline_report', None)
    selected_id = (previous['fixture']['fixture_id'] if previous else
                   baseline['fixture']['fixture_id'] if baseline else None)
    fixture, normalized, reasons = select_fixture(
        client, args, target, base,
        fixture_id=selected_id)
    doc = dict(base, operator_recovery_drill={'campaign': AUTHORIZATION, 'fixture_image': frozen_image})
    pid = identity(args.repository, 'native', sha(doc))
    marker_keys = [AUTHORIZATION + ':exhaust', AUTHORIZATION + ':recover']
    marker_ids = [identity(args.repository, args.producer, key) for key in marker_keys]
    run_key = f"actions:{report['actions']['id']}:1"
    budget_key = f"actions:{report['actions']['id']}"
    this_run = identity(args.repository, args.producer, run_key)
    this_budget = identity(args.repository, 'budget', budget_key)
    run_ids = ([this_run] if previous is None else
               [previous['invocations'][0]['context']['run'], this_run])
    report.update(source_revision=args.revision, corpus=corpus, provenance_id=pid,
                  native_provenance_id=current['provenance_id'],
                  native_execution_identity_sha256=sha(base),
                  image_archive_sha256=image_archive_sha256(args),
                  execution_identity_sha256=sha(doc), execution_identity_components={
                      key: sha(base[key]) for key in ('binaries', 'harness', 'tooling',
                                                     'capabilities', 'environment_identity', 'timeouts')},
                  fixture_image_id=frozen_image, master_validation=validation,
                  control_plane_sha256=sha(before),
                  fixture={key: fixture[key] for key in
                           ('fixture_id', 'upstream_path', 'content_sha256', 'dependency_manifest_digest')},
                  required_variants=['non-strict', 'strict'])
    if args.phase == 'verify':
        check_identity(args, corpus, base, frozen_image)
        require(control_plane(client, args) == before
                and all(client.one('runs', external_run_key=key) is None for key in marker_keys)
                and not any(client.read('work_items', {'state': 'leased'})),
                'Baseline changed, campaign already started or unrelated work is leased')
        report.update(stage='baseline-verified', phase_complete=True, complete=False,
                      actual_fixture_executions=0, control_plane_unchanged=True,
                      checked_at=datetime.now(timezone.utc).isoformat())
        checkpoint(args, report)
        return report
    frozen_fields = ('source_revision', 'corpus', 'provenance_id', 'native_provenance_id',
                     'native_execution_identity_sha256', 'execution_identity_sha256',
                     'fixture_image_id', 'image_archive_sha256', 'control_plane_sha256',
                     'fixture', 'required_variants')
    require(baseline and baseline['phase'] == 'verify'
            and all(sha(baseline[key]) == sha(report[key]) for key in frozen_fields),
            'Source/full identity/image/fixture/control plane differs from the frozen baseline')
    if previous is not None:
        require(previous['source_revision'] == args.revision
                and previous['execution_identity_sha256'] == sha(doc)
                and previous['fixture_image_id'] == frozen_image
                and previous['provenance_id'] == pid
                and previous['control_plane_sha256'] == sha(before)
                and previous['fixture'] == report['fixture']
                and previous['baseline_source'] == report['baseline_source']
                and previous['actions']['id'] != report['actions']['id'],
                'Previous source/full identity/fixture/control plane differs')
    checkpoint(args, report)
    prepared = {'source': args.revision, 'corpus': corpus,
                'ids': {normalized['path']: fixture['fixture_id']},
                'snapshot': target['registration_snapshot_id'], 'pid': pid, 'doc': doc,
                'normalized': [normalized], 'eligibility': [{
                    'repository_id': args.repository, 'provenance_id': pid,
                    'fixture_id': fixture['fixture_id'], 'eligibility': 'runnable',
                    'reason_codes': reasons}]}
    completed_work = {row['work_item_id'] for row in report['execution_intents']}
    first_budget = previous['invocations'][0]['budget'] if previous else None

    def check_previous_evidence():
        if previous is not None:
            original = [row for row in observations(client, pid)
                        if row['observation_id'] == report['first_observation']]
            require(len(original) == 1 and sha(original[0]) == report['first_observation_sha256']
                    and sha(client.one('budget_scopes', budget_scope_id=first_budget['budget_scope_id']))
                    == sha(first_budget),
                    'Original observation payload or previous charges changed; stop the drill')

    check_previous_evidence()

    def guard(lease):
        check_identity(args, corpus, base, frozen_image)
        check_previous_evidence()
        require(control_plane(client, args) == before, 'Control-plane state changed; stop the drill')
        require(lease['fixture_id'] == fixture['fixture_id'] and lease['provenance_id'] == pid
                and lease['variant'] in ('non-strict', 'strict')
                and lease['work_item_id'] not in completed_work
                and len(report['execution_intents']) == index,
                'Foreign, repeated or excess fixture execution rejected')
        expected = identity(args.repository, pid, fixture['fixture_id'], lease['variant'], 0)
        require(lease['work_item_id'] == expected and lease['lease_generation'] == 1,
                'Unexpected retry/work identity; no additional execution allowed')
        completed_work.add(lease['work_item_id'])
        report['execution_intents'].append({
            key: lease[key] for key in ('work_item_id', 'fixture_id', 'variant', 'lease_generation')})
        checkpoint(args, report)

    def before_claim():
        check_identity(args, corpus, base, frozen_image)
        check_previous_evidence()
        # Match api_lock's repository key and keep this lock through api_claim,
        # so its repository-wide expiry sweep cannot race the scoped preflight.
        client.db.execute('SELECT pg_advisory_xact_lock(hashtextextended(%s,0))',
                          ('test262:' + args.repository,))
        require(control_plane(client, args) == before, 'Control-plane state changed before claiming')
        # The shared claim API also settles expired leases. Do not use a drill
        # invocation to clean up or compete with unrelated in-flight work.
        require(not any(client.read('work_items', {'state': 'leased'})),
                'Existing leased work must finish before this bounded drill')

    with tempfile.TemporaryDirectory(prefix='authorized-recovery-runtime-') as runtime:
        stage_runtime(args, runtime)
        Path(runtime).chmod(0o755)
        args.runtime = runtime
        folder = Path(args.output).parent
        for index in [index]:
            report['stage'] = 'prepare-exhaustion' if index == 0 else 'prepare-recovery'
            checkpoint(args, report)
            check_identity(args, corpus, base, frozen_image)
            require(control_plane(client, args) == before, 'Control-plane state changed before planning')
            args.run_key, args.budget_key = run_key, budget_key
            args.output_context = str(folder / f'context-{index}.json')
            private = {'campaign': AUTHORIZATION, 'fixture_id': fixture['fixture_id'],
                       'cursor_before': index}
            preparation = SimpleNamespace(**vars(args))
            preparation.output = args.output_context
            with client.db.transaction():
                client.db.execute('SELECT pg_advisory_xact_lock(%s)',
                                  (int(sha(AUTHORIZATION)[:15], 16),))
                if index == 0:
                    require(all(client.one('runs', external_run_key=key) is None for key in marker_keys),
                            'Authorized campaign already started; retries cannot gain executions')
                    provenance(client, args.repository, corpus, 'native', doc)
                    client.put('provenance_fixture_eligibility', prepared['eligibility'])
                else:
                    require(client.one('budget_scopes', budget_scope_id=first_budget['budget_scope_id']) == first_budget
                            and client.one('runs', external_run_key=marker_keys[0]) == previous['campaign_marker']
                            and client.one('runs', external_run_key=marker_keys[1]) is None,
                            'Previous charges changed or recovery allocation already exists')
                require(client.one('runs', run_id=this_run) is None
                        and client.one('budget_scopes', budget_scope_id=this_budget) is None,
                        'Actions allocation already exists; retries cannot reset it')
                marker = client.call('start_run', {
                    'run_id': marker_ids[index], 'provenance_id': pid,
                    'external_run_key': marker_keys[index], 'source_revision': args.revision,
                    'run_kind': 'native', 'environment_diagnostics': {
                        'campaign': AUTHORIZATION, 'phase': args.phase,
                        'administrative_latch': True, 'fixture_executions': 0,
                        'actions_run_id': report['actions']['id']}})
                marker = client.call(
                    'finish_run', marker_ids[index], 'cancelled', marker['started_at'],
                    {'source': 'supervisor', 'external_run_key': marker_keys[index],
                     'source_revision': args.revision, 'reason': 'job-cancelled',
                     'flush_verified': True})
                context = commands.prepare_from_inventory(
                    client, preparation, prepared, recovery_drill=private)
            report['campaign_marker'] = marker
            require(context['run'] == this_run and context['budget'] == this_budget
                    and context['candidate_ids'] == [fixture['fixture_id']],
                    'Prepared allocation escaped the fixed fixture')
            expected_recovery = ((0, 2, 0) if index == 0 else (1, 1, 1))
            require(tuple(context['recovery'][key] for key in
                          ('resumed_fixtures', 'queued_variants', 'reused_terminal_variants')) == expected_recovery,
                    'Missing-variant planning differs from the authorized drill')
            args.run, args.budget = context['run'], context['budget']
            args.outbox = str(folder / f'outbox-{index}.sqlite')
            report['stage'] = 'execute-exhaustion' if index == 0 else 'execute-recovery'
            checkpoint(args, report)
            worker = work_staged(GuardedClaims(client, before_claim), args, before_execute=guard)
            require(worker['attempts'] == 1
                    and (index != 0 or worker['stop_reason'] == 'budget-exhausted'),
                    'Exactly one execution per allocation and real initial budget exhaustion required')
            receipt = lifecycle.finalize(client, SimpleNamespace(
                repository=args.repository, producer=args.producer, context=args.output_context,
                outbox=args.outbox, run_key=args.run_key, conclusion='success', flush_verified=True))
            budget = client.one('budget_scopes', budget_scope_id=args.budget)
            rows = observations(client, pid)
            work = list(client.read('work_items', {'provenance_id': pid}))
            require(budget['reserved_attempts'] == budget['reserved_ms'] == 0
                    and budget['charged_attempts'] == 1
                    and receipt['budget']['result'] == budget,
                    'Budget charges/reservations do not match the live receipt')
            require(len(rows) == index + 1 and len({row['work_item_id'] for row in rows}) == index + 1
                    and all(row['outcome'] in ('pass', 'fail', 'unsupported') for row in rows),
                    'Missing, repeated or infrastructure-only evidence cannot complete the drill')
            if index == 0:
                require(budget['state'] == 'exhausted'
                        and sorted(row['state'] for row in work) == ['completed', 'pending'],
                        'Initial budget did not leave exactly one required variant pending')
                first_budget = budget
                report['first_observation'] = rows[0]['observation_id']
                report['first_observation_sha256'] = sha(rows[0])
            else:
                check_previous_evidence()
                require(budget['state'] == 'closed'
                        and client.one('budget_scopes', budget_scope_id=first_budget['budget_scope_id']) == first_budget
                        and {row['variant'] for row in rows} == {'non-strict', 'strict'}
                        and {row['run_id'] for row in rows} == set(run_ids)
                        and any(row['observation_id'] == report['first_observation'] for row in rows)
                        and all(row['state'] == 'completed' for row in work),
                        'Recovery lost/replaced original evidence, charges or the missing variant')
            report['invocations'].append({
                'phase': index, 'context': context, 'worker': worker, 'lifecycle': receipt,
                'budget': budget, 'outbox': outbox_proof(args),
                'observations': [{key: row[key] for key in
                                  ('observation_id', 'run_id', 'provenance_id', 'fixture_id',
                                   'work_item_id', 'variant', 'outcome', 'lease_generation')}
                                 for row in rows],
                'work': [{key: row[key] for key in
                          ('work_item_id', 'fixture_id', 'variant', 'state', 'lease_generation')}
                         for row in work],
            })
            checkpoint(args, report)
    check_identity(args, corpus, base, frozen_image)
    require(control_plane(client, args) == before and len(report['execution_intents']) == index + 1,
            'Final source/control-plane/execution bounds differ')
    report.update(stage='verified' if index == 1 else 'exhaustion-verified',
                  phase_complete=True, complete=index == 1, actual_fixture_executions=index + 1,
                  original_budget_unchanged=index == 1, control_plane_unchanged=True,
                  checked_at=datetime.now(timezone.utc).isoformat())
    checkpoint(args, report)
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--authorization', required=True)
    parser.add_argument('--phase', choices=['verify', 'exhaust', 'recover'], required=True)
    parser.add_argument('--previous-run-id', type=int)
    parser.add_argument('--baseline-run-id', type=int)
    parser.add_argument('--image-archive', required=True)
    parser.add_argument('--restore-image-only', action='store_true')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--repository', default=os.getenv('TEST262_REPOSITORY_ID'))
    parser.add_argument('--producer', default=os.getenv('TEST262_PRODUCER_ID'))
    parser.add_argument('--image', default='test262-fixture:recovery-drill')
    parser.add_argument('--jroc', default='src/Cli/bin/Release/net10.0/Jroc.dll')
    parser.add_argument('--host', default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    args = parser.parse_args(argv)
    report = {'complete': False, 'stage': 'authorization'}
    client = None
    try:
        report['actions'] = check_invocation(args)
        require((args.phase in ('verify', 'exhaust') and args.previous_run_id is None)
                or (args.phase == 'recover' and args.previous_run_id is not None),
                'Only recovery takes the preceding run identity')
        require((args.phase == 'verify' and args.baseline_run_id is None and not args.restore_image_only)
                or (args.phase in ('exhaust', 'recover') and args.baseline_run_id is not None),
                'Only live phases take the authenticated frozen baseline')
        if args.phase != 'verify':
            args.baseline_report, report['baseline_source'] = fetch_baseline(
                args.baseline_run_id, args.revision)
        if args.phase == 'recover':
            args.previous_report, report['previous_source'] = fetch_previous(
                args.previous_run_id, args.revision)
        require(args.repository and args.producer, 'Explicit scoped catalogue identities required')
        if args.restore_image_only:
            report.update(stage='image-verified', image=restore_image(args), fixture_executions=0)
            checkpoint(args, report)
            return 0
        client = Client()
        if args.phase == 'verify':
            with client.view():
                client.db.execute('SET LOCAL default_transaction_read_only=on')
                require(client.db.execute('SHOW default_transaction_read_only').fetchone()[0] == 'on'
                        and client.db.execute('SHOW transaction_read_only').fetchone()[0] == 'on',
                        'Baseline must use verified server read-only settings')
                report['baseline_read_only'] = True
                run(client, args, report)
        else:
            run(client, args, report)
        return 0
    except Exception as error:
        report.update(complete=False, error_class=type(error).__name__)
        error._test262_stage = 'recovery-drill.' + report['stage']
        try:
            checkpoint(args, report)
        except OSError as journal_error:
            journal_error._test262_stage = 'recovery-drill.failure-journal'
            report_failure(journal_error)
        report_failure(error)
        return 1
    finally:
        if client is not None:
            client.close()


if __name__ == '__main__':
    raise SystemExit(main())
