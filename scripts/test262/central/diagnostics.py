"""Safe operation diagnostics: no exception messages, locals, DSNs or payloads."""
from contextlib import contextmanager
import json
import os
from pathlib import Path
import re
import sys
import time

REPO = Path(__file__).resolve().parents[3]
TOOLS = {'git', 'node', 'dotnet', 'gh', 'docker', 'pg_dump', 'pg_restore'}


def progress(stage, event, **fields):
    if os.getenv('TEST262_PROGRESS') == '1':
        print(json.dumps(dict(stage=stage, event=event, **fields), sort_keys=True), flush=True)


@contextmanager
def stage(name):
    started = time.monotonic()
    progress(name, 'started')
    try:
        yield
    except Exception as error:
        if not getattr(error, '_test262_stage', None):
            error._test262_stage = name
        raise
    else:
        progress(name, 'completed', seconds=round(time.monotonic() - started, 1))


def repository_path(value):
    """Only emit ordinary repository-relative paths or allowlisted tool names."""
    if not isinstance(value, str) or not re.fullmatch(r'[A-Za-z0-9_. /()+-]+', value):
        return None
    if value in TOOLS:
        return value
    try:
        return Path(value).resolve().relative_to(REPO.resolve()).as_posix()
    except (ValueError, OSError):
        return None


def failure_record(error):
    record = {'error_type': type(error).__name__}
    operation = getattr(error, '_test262_stage', None)
    if isinstance(operation, str) and re.fullmatch(r'[a-z][a-z0-9_.-]{0,80}', operation):
        record['stage'] = operation
    sqlstate = getattr(error, 'sqlstate', None)
    if isinstance(sqlstate, str) and re.fullmatch(r'[A-Z0-9]{5}', sqlstate):
        record['sqlstate'] = sqlstate
    if isinstance(error, FileNotFoundError):
        record['missing_path'] = repository_path(error.filename) or '<outside repository or redacted>'
    locations = []
    trace = error.__traceback__
    while trace:
        path = repository_path(trace.tb_frame.f_code.co_filename)
        if path:
            locations.append({'file': path, 'line': trace.tb_lineno})
        trace = trace.tb_next
    if locations:
        record['locations'] = locations
    return record


def report_failure(error):
    record = failure_record(error)
    print('Catalogue operation failed: ' + json.dumps(record, sort_keys=True), file=sys.stderr)
    destination = os.getenv('TEST262_DIAGNOSTIC_FILE')
    if destination:
        try:
            path = Path(destination)
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(json.dumps(record, sort_keys=True) + '\n', encoding='utf-8')
        except OSError:
            print('Could not preserve catalogue diagnostic file.', file=sys.stderr)
