"""Actual catalogue backup/restore on the disposable pilot service only."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
import tempfile
from types import SimpleNamespace
from unittest.mock import patch

from . import backup
from .pilot import disposable_settings
from .pilot_drills import require


def row_digest(db, table):
    from psycopg import sql
    # Only the two deliberately fenced control fields differ after restoration.
    expression = sql.SQL('to_jsonb(t)')
    if table == 'api_subjects':
        expression += sql.SQL(" - 'enabled'")
    elif table == 'schema_contract':
        expression += sql.SQL(" - 'deployment_state' - 'minimum_writer_epoch'")
    digest = hashlib.sha256()
    with db.cursor() as cursor:
        cursor.execute(sql.SQL('SELECT ({})::text FROM test262.{} t ORDER BY 1').format(
            expression, sql.Identifier(table)))
        for row in cursor:
            digest.update((row[0]+'\n').encode())
    return digest.hexdigest()


def metadata(db):
    return db.execute("""
      SELECT 'relation',n.nspname,c.relname,c.relkind::text,
             c.relrowsecurity::text,c.relforcerowsecurity::text,coalesce(c.relacl::text,'')
      FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
      WHERE n.nspname IN ('test262','test262_reporting') AND c.relkind IN ('r','v','S')
      UNION ALL
      SELECT 'function',n.nspname,p.proname,pg_get_function_identity_arguments(p.oid),
             p.prosecdef::text,coalesce(p.proconfig::text,''),coalesce(p.proacl::text,'')
      FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace
      WHERE n.nspname IN ('test262','test262_reporting')
      ORDER BY 1,2,3,4
    """).fetchall()


def docker_tools(output):
    """Use version-matched client tools without installing them on the runner."""
    def run(command, env=None):
        require(command[0] in ('pg_dump','pg_restore','psql'), 'Unexpected restore-drill command')
        environment = env if env is not None else os.environ
        options = ['docker','run','--rm','--network','host',
                   '--mount',f'type=bind,source={output},target={output}',
                   '--mount',f'type=bind,source={tempfile.gettempdir()},target={tempfile.gettempdir()}']
        # Values remain in process environment. Docker receives only variable names.
        for key in backup.LIBPQ_ENVIRONMENT.values():
            if key in environment:
                options += ['--env',key]
        subprocess.run(options+['postgres:17']+command, env=environment, check=True,
                       capture_output=True, timeout=180)
    return run


def run_restore_drill(admin, output, report):
    import psycopg
    from psycopg.conninfo import make_conninfo
    disposable_settings(admin)  # Before any writes or client-tool execution.
    output = (Path(output)/'restore-drill').resolve()
    output.mkdir(parents=True, exist_ok=True)
    target = make_conninfo(admin, dbname='catalogue_pilot_restore')
    report.update(complete=False, scope='disposable PostgreSQL only',
                  stage='source-checkpoint', independent_retention_verified=False)
    with psycopg.connect(admin, autocommit=True) as source:
        require(not source.execute("SELECT 1 FROM pg_database WHERE datname='catalogue_pilot_restore'").fetchone(),
                'Restore drill requires an unused target database name')
        require(source.execute('SELECT count(*) FROM test262.observations').fetchone()[0] >= 11,
                'Restore drill requires the populated real worker/process pilot')
        # A disposable performance sentinel proves the dump excludes public data.
        require(source.execute("SELECT to_regclass('public.perf_results')").fetchone()[0] is None,
                'Unexpected performance data in pilot source')
        source.execute('CREATE TABLE public.perf_results (id integer PRIMARY KEY, sentinel text NOT NULL)')
        source.execute("INSERT INTO public.perf_results VALUES(1,'disposable-preservation-sentinel')")
        tables = [r[0] for r in source.execute("SELECT tablename FROM pg_tables WHERE schemaname='test262' ORDER BY tablename")]
        digests = {table:row_digest(source,table) for table in tables}
        privileges = metadata(source)
        contract = source.execute('SELECT deployment_state,minimum_writer_epoch FROM test262.schema_contract').fetchone()
        enabled = source.execute('SELECT count(*) FROM test262.api_subjects WHERE enabled').fetchone()[0]
        require(contract == ('active',1) and enabled > 0, 'Need enabled source bindings to demonstrate restore fencing')
        source.execute('CREATE DATABASE catalogue_pilot_restore')
        report['stage'] = 'backup'
        with patch.dict(os.environ, {'TEST262_BACKUP_DATABASE_URL':admin,
                                     'TEST262_RESTORE_DATABASE_URL':target}), patch.object(backup,'run',docker_tools(output)):
            backup.backup(SimpleNamespace(output=str(output),destination=None,kms_key=None))
            report['stage'] = 'restore'
            backup.restore(SimpleNamespace(archive=str(output/'catalogue.dump'),manifest=str(output/'manifest.json')))
        report['stage'] = 'verify-content-and-privileges'
        with psycopg.connect(target, autocommit=True) as restored:
            require({table:row_digest(restored,table) for table in tables} == digests,
                    'Restored catalogue content differs beyond intentional credential fencing')
            require(metadata(restored) == privileges, 'Restored RLS/ACL/function security metadata differs')
            require(restored.execute("SELECT to_regclass('public.perf_results')").fetchone()[0] is None,
                    'Performance sentinel entered the catalogue archive')
            require(restored.execute('SELECT deployment_state,minimum_writer_epoch FROM test262.schema_contract').fetchone() == ('shadow',2),
                    'Restored authority/epoch not fenced')
            require(restored.execute('SELECT count(*) FROM test262.api_subjects WHERE enabled').fetchone()[0] == 0,
                    'Restored writer binding enabled')
            with restored.transaction(force_rollback=True):
                restored.execute('SET LOCAL SESSION AUTHORIZATION catalogue_pilot_worker')
                try:
                    restored.execute("SELECT test262.api_read(2,'fixtures','{}'::jsonb,NULL,1)")
                except psycopg.errors.InsufficientPrivilege:
                    pass
                else:
                    raise ValueError('Restored old login still accepted by API')
        require({table:row_digest(source,table) for table in tables} == digests and
                source.execute('SELECT deployment_state,minimum_writer_epoch FROM test262.schema_contract').fetchone() == contract and
                source.execute('SELECT count(*) FROM test262.api_subjects WHERE enabled').fetchone()[0] == enabled,
                'Backup/restore changed source catalogue or authority')
        require(source.execute('SELECT * FROM public.perf_results').fetchall() == [(1,'disposable-preservation-sentinel')],
                'Source performance sentinel changed')
    manifest = json.loads((output/'manifest.json').read_text())
    report.update(complete=True, stage='verified', table_count=len(tables),
                  observations=manifest['row_counts']['observations'], view_count=len(manifest['view_counts']),
                  catalogue_content_parity=True, rls_acl_function_security_parity=True,
                  source_unchanged=True, performance_data_excluded=True,
                  source_enabled_bindings=enabled, restored_enabled_bindings=0,
                  restored_authority='shadow', restored_epoch=2, old_login_rejected=True,
                  archive_sha256=manifest['sha256'], manifest='restore-drill/manifest.json')
