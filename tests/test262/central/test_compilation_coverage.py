import copy
import argparse
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central.client import Outbox
from scripts.test262.central.inventory import REPO
from scripts.test262.central.worker import run_container
from scripts.test262.compilationCoverage import aggregate, from_outbox, from_summary, validate_report


def report(mode='directIl', succeeded=True):
    counts = dict.fromkeys(('directIl', 'runtimeIntrinsic', 'runtimeDispatch', 'unsupported'), 0)
    counts[mode] = 1
    return {'schemaVersion': 1, 'measurement': 'statement-source-sites-v1',
            'compilationSucceeded': succeeded, 'complete': succeeded,
            'counts': {**counts, 'total': 1}, 'percentages': {key: value * 100 for key, value in counts.items()},
            'sites': [{'file': 'input.js', 'line': 1, 'column': 1, 'mode': mode, 'reasons': ['test']}],
            'diagnostics': []}


class CompilationCoverageTests(unittest.TestCase):
    def test_correctness_and_implementation_modes_are_separate(self):
        entries = [
            {'fixture': 'passing', 'variant': 'strict', 'outcome': 'pass', 'report': report('runtimeDispatch')},
            {'fixture': 'assertion-failed', 'variant': 'strict', 'outcome': 'fail', 'report': report()},
            {'fixture': 'eval', 'variant': 'strict', 'outcome': 'fail', 'report': report('unsupported', False)},
            {'fixture': 'parse-negative', 'variant': 'strict', 'outcome': 'pass', 'report': report('unsupported', False)},
            {'fixture': 'harness-gap', 'variant': 'strict', 'outcome': 'unsupported'},
        ]
        result = aggregate(entries, {'runner': 'native'})
        self.assertEqual(result['correctness'], {'fail': 2, 'pass': 2, 'unsupported': 1})
        self.assertEqual(result['passingCompilationModes']['counts']['total'], 1)
        self.assertEqual(result['passingCompilationModes']['percentages']['runtimeDispatch'], 100)
        self.assertEqual(result['passingCompilationModes']['counts']['unsupported'], 0)
        self.assertEqual(result['missingReports'], 1)
        self.assertFalse(result['results'][2]['includedInPassingCompilationModes'])
        self.assertEqual(result['results'][2]['report']['counts']['unsupported'], 1)

    def test_empty_totals_are_zero_and_duplicates_are_rejected(self):
        result = aggregate([], {})
        self.assertTrue(all(value == 0 for value in result['passingCompilationModes']['percentages'].values()))
        entry = {'fixture': 'one', 'variant': 'strict', 'outcome': 'pass'}
        with self.assertRaisesRegex(ValueError, 'Duplicate'):
            aggregate([entry, entry], {})

    def test_incompatible_and_corrupt_reports_fail_explicitly(self):
        mutations = [
            ('schemaVersion', 2), ('measurement', 'instruction-counts'),
            ('counts', {'directIl': -1}), ('percentages', {'directIl': 1}),
            ('sites', []), ('complete', False),
        ]
        for field, value in mutations:
            invalid = copy.deepcopy(report())
            invalid[field] = value
            with self.subTest(field=field), self.assertRaises((ValueError, KeyError)):
                validate_report(invalid)

    def test_outbox_replay_and_retries_count_latest_variant_once(self):
        with tempfile.TemporaryDirectory() as directory:
            context = {'repository': 'repo', 'producer': 'producer', 'run': 'run', 'provenance': 'pin',
                       'kind': 'native', 'identity': {'compiler_options': {'compilation_coverage': True}}}
            context_path = Path(directory) / 'context.json'
            context_path.write_text(json.dumps(context))
            path = Path(directory) / 'outbox.sqlite'
            outbox = Outbox(path, 'repo', 'producer', 1)
            first = {'observation_id': 'first', 'run_id': 'run', 'provenance_id': 'pin', 'fixture_id': 'fixture',
                     'variant': 'strict', 'lease_generation': 1, 'finished_at': '2026-01-01T00:00:00Z',
                     'outcome': 'pass', 'payload': {'compilation_coverage': report()}}
            retry = {**first, 'observation_id': 'retry', 'lease_generation': 2, 'outcome': 'fail',
                     'payload': {'compilation_coverage': report('runtimeDispatch')}}
            unrelated = {**first, 'observation_id': 'other', 'run_id': 'other-run'}
            outbox.enqueue([first], 'request-one')
            outbox.enqueue([first, retry, unrelated], 'request-two')
            outbox.db.close()
            result = from_outbox(path, context_path)
            self.assertEqual(result['observedAttempts'], 2)
            self.assertEqual(result['correctness'], {'fail': 1})
            self.assertEqual(result['passingCompilationModes']['counts']['total'], 0)
            self.assertEqual(result['results'][0]['observationId'], 'retry')
            outbox = Outbox(path, 'repo', 'producer', 1)
            outbox.db.execute("UPDATE messages SET digest='corrupt'")
            outbox.db.commit()
            outbox.db.close()
            with self.assertRaisesRegex(ValueError, 'Corrupt'):
                from_outbox(path, context_path)

    def test_summary_preserves_pin_variant_and_harness_scope(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'summary.json'
            path.write_text(json.dumps({
                'pin': {'commit': 'pinned'}, 'selection': {'selected': 1},
                'compilationCoverageIdentity': {'files': {'Jroc.dll': 'hash'}},
                'results': [{'relativePath': 'test/language/sample.js', 'variant': 'strict',
                             'classification': {'verdict': 'matched', 'kind': 'pass'},
                             'compilation_coverage': report()}]}))
            result = from_summary(path)
            self.assertEqual(result['passingCompilationModes']['measuredFixtureVariants'], 1)
            self.assertEqual(result['identity']['sourceScope'], 'prepared-fixture-and-JavaScript-harness')
            self.assertEqual(result['identity']['compiler']['files']['Jroc.dll'], 'hash')

    def test_worker_propagates_opt_in_inside_existing_isolation_boundary(self):
        for kind in ('mvp-composite', 'native'):
            for enabled in (False, True):
                with self.subTest(kind=kind, enabled=enabled), tempfile.TemporaryDirectory() as directory:
                    args = argparse.Namespace(
                        root=directory, runtime=directory, image='fixture:local', kind=kind,
                        jroc=str(REPO / 'src/Cli/bin/Release/net10.0/Jroc.dll'),
                        host=str(REPO / 'scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll'),
                        runtime_timeout=5, compile_timeout=5, compilation_coverage=enabled)
                    fixture = {'upstream_path': 'test/sample.js', 'content_sha256': '\\x' + 'a' * 64}
                    with patch('scripts.test262.central.worker.subprocess.run',
                               return_value=Mock(returncode=0, stdout='{}')) as execute:
                        run_container(args, fixture, 'strict', 30000, {'dependencies': []}, Path(directory))
                    command = execute.call_args_list[0].args[0]
                    self.assertIn('--network', command)
                    self.assertEqual(command[command.index('--network') + 1], 'none')
                    self.assertIn('--read-only', command)
                    if kind == 'mvp-composite':
                        request = json.loads(execute.call_args_list[0].kwargs['input'])
                        self.assertEqual(request.get('compilationCoverage', False), enabled)
                    else:
                        request = json.loads((Path(directory) / 'plan.json').read_text())
                        self.assertEqual(request.get('compilation_coverage', False), enabled)
