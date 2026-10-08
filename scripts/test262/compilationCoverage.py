"""Credential-free aggregation of static compiler reports, not execution coverage."""
import argparse
from collections import Counter
import json
import math
from pathlib import Path
import sqlite3

from scripts.test262.central.client import canonical, sha

MEASUREMENT = 'statement-source-sites-v1'
MODES = ('directIl', 'runtimeIntrinsic', 'runtimeDispatch', 'unsupported')


def validate_report(report):
    if type(report.get('schemaVersion')) is not int or report['schemaVersion'] != 1 or report.get('measurement') != MEASUREMENT:
        raise ValueError('Incompatible compilation coverage schema or measurement')
    if type(report.get('compilationSucceeded')) is not bool or report.get('complete') is not report['compilationSucceeded']:
        raise ValueError('Invalid compilation coverage completeness')
    counts = report['counts']
    if any(type(counts.get(mode)) is not int or counts[mode] < 0 for mode in MODES):
        raise ValueError('Invalid compilation coverage counts')
    total = sum(counts[mode] for mode in MODES)
    sites = report['sites']
    if type(counts.get('total')) is not int or counts['total'] != total or len(sites) != total:
        raise ValueError('Compilation coverage counts disagree with source sites')
    actual = Counter(site['mode'] for site in sites)
    if any(actual[mode] != counts[mode] for mode in MODES) or set(actual) - set(MODES):
        raise ValueError('Compilation coverage mode counts disagree')
    for mode in MODES:
        expected = 100 * counts[mode] / total if total else 0
        percentage = report['percentages'][mode]
        if type(percentage) not in (float, int) or not math.isclose(percentage, expected, abs_tol=1e-9):
            raise ValueError('Invalid compilation coverage percentage')


def aggregate(entries, identity):
    correctness = Counter()
    counts = dict.fromkeys(MODES, 0)
    measured = 0
    missing = 0
    details = []
    seen = set()
    for entry in entries:
        key = (entry['fixture'], entry['variant'])
        if key in seen:
            raise ValueError('Duplicate fixture variant in compilation coverage input')
        seen.add(key)
        correctness[entry['outcome']] += 1
        report = entry.get('report')
        included = False
        if report is None:
            missing += 1
        else:
            validate_report(report)
            if entry['outcome'] == 'pass' and report['complete']:
                included = True
                measured += 1
                for mode in MODES:
                    counts[mode] += report['counts'][mode]
        details.append({**entry, 'includedInPassingCompilationModes': included})
    total = sum(counts.values())
    return {
        'schemaVersion': 1, 'measurement': MEASUREMENT, 'identity': identity,
        'correctness': dict(sorted(correctness.items())),
        'passingCompilationModes': {
            'measuredFixtureVariants': measured, 'counts': {**counts, 'total': total},
            'percentages': {mode: 100 * counts[mode] / total if total else 0 for mode in MODES}},
        'missingReports': missing, 'results': details,
    }


def from_summary(filename):
    summary = json.loads(Path(filename).read_text())
    entries = []
    for result in summary['results']:
        verdict = result['classification']['verdict']
        entries.append({
            'fixture': result['relativePath'], 'variant': result['variant'],
            'outcome': {'matched': 'pass', 'unexpected': 'fail', 'not-run': 'not-run'}[verdict],
            'kind': result['classification']['kind'],
            'report': result.get('compilation_coverage'),
            'unavailableReason': result.get('compilation_coverage_unavailable'),
        })
    return aggregate(entries, {'runner': 'mvp-composite', 'sourceScope': 'prepared-fixture-and-JavaScript-harness',
                               'pin': summary['pin'], 'selection': summary['selection'],
                               'compiler': summary.get('compilationCoverageIdentity')})


def from_outbox(filename, context_filename):
    context = json.loads(Path(context_filename).read_text())
    observations = {}
    latest = {}
    with sqlite3.connect(Path(filename).resolve().as_uri() + '?mode=ro', uri=True) as db:
        scope = json.loads(db.execute('SELECT identity FROM scope WHERE singleton=1').fetchone()[0])
        if scope[:2] != [context['repository'], context['producer']]:
            raise ValueError('Coverage outbox belongs to another repository or producer')
        for payload, digest in db.execute('SELECT payload,digest FROM messages ORDER BY rowid'):
            rows = json.loads(payload)
            if sha(rows) != digest:
                raise ValueError('Corrupt coverage outbox')
            for row in rows:
                if row['run_id'] != context['run'] or row['provenance_id'] != context['provenance']:
                    continue
                observation = row['observation_id']
                if observation in observations and observations[observation] != row:
                    raise ValueError('Conflicting coverage observation identity')
                observations[observation] = row
                key = (row['fixture_id'], row['variant'])
                order = (row['lease_generation'], row['finished_at'], observation)
                if key not in latest or order > latest[key][0]:
                    latest[key] = (order, row)
    entries = []
    for key in sorted(latest):
        row = latest[key][1]
        payload = row['payload']
        entries.append({
            'fixture': row['fixture_id'], 'path': payload.get('path', payload.get('relativePath')),
            'variant': row['variant'], 'outcome': row['outcome'],
            'observationId': row['observation_id'], 'leaseGeneration': row['lease_generation'],
            'failureClass': row.get('failure_class'), 'report': payload.get('compilation_coverage'),
        })
    result = aggregate(entries, {
        'runner': context['kind'], 'run': context['run'], 'provenance': context['provenance'],
        'outboxScope': scope, 'compilerOptions': context['identity'].get('compiler_options', {}),
        'sourceScope': 'prepared-native-fixture' if context['kind'] == 'native'
                       else 'prepared-fixture-and-JavaScript-harness',
    })
    result['observedAttempts'] = len(observations)
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument('--summary')
    source.add_argument('--outbox')
    parser.add_argument('--context')
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    if args.outbox and not args.context:
        parser.error('--outbox requires --context')
    report = from_summary(args.summary) if args.summary else from_outbox(args.outbox, args.context)
    output = Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, indent=2) + '\n')
    print(canonical({'output': str(output), 'correctness': report['correctness'],
                     'passingCompilationModes': report['passingCompilationModes'],
                     'missingReports': report['missingReports']}))


if __name__ == '__main__':
    main()
