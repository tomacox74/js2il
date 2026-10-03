"""Catalogue-only, off-project encrypted backups and disposable restore drills.

Never use the restore command against Supabase/performance data. AWS credentials
belong to this dedicated job, not to fixture runners.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import tempfile
import uuid


def run(command, env=None):
    subprocess.run(command,check=True,env=env)


def backup(args):
    import psycopg
    dsn=os.environ['TEST262_BACKUP_DATABASE_URL']
    output=Path(args.output);output.mkdir(parents=True,exist_ok=True)
    archive=output/'catalogue.dump'
    env=dict(os.environ,PGDATABASE=dsn)
    # Pin the dump and its verification counts to the SAME exported MVCC snapshot.
    with psycopg.connect(dsn) as db:
        db.execute('SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY')
        token=db.execute('SELECT pg_export_snapshot()').fetchone()[0]
        run(['pg_dump','--snapshot='+token,'--format=custom','--no-owner','--schema=test262','--schema=test262_reporting','--file',str(archive)],env)
        contract=db.execute('SELECT to_jsonb(c) FROM test262.schema_contract c').fetchone()[0]
        names=[r[0] for r in db.execute("SELECT tablename FROM pg_tables WHERE schemaname='test262' ORDER BY tablename")]
        counts={name:db.execute('SELECT count(*) FROM test262."'+name+'"').fetchone()[0] for name in names}
        views=[r[0] for r in db.execute("SELECT table_name FROM information_schema.views WHERE table_schema='test262_reporting'")]
        view_counts={name:db.execute('SELECT count(*) FROM test262_reporting."'+name+'"').fetchone()[0] for name in views}
    digest=hashlib.sha256(archive.read_bytes()).hexdigest()
    manifest={'sha256':digest,'contract':{k:str(v) if hasattr(v,'isoformat') else v for k,v in contract.items()},'row_counts':counts,'view_counts':view_counts,
              'roles':['test262_ingest','test262_coordinator','test262_reporter'],'contains_performance_data':False}
    (output/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
    if args.destination:
        if not args.kms_key:
            raise ValueError('An external KMS key is required')
        for file in (archive,output/'manifest.json'):
            run(['aws','s3','cp',str(file),args.destination.rstrip('/')+'/'+file.name,'--sse','aws:kms','--sse-kms-key-id',args.kms_key,'--only-show-errors'])
        print('Encrypted catalogue backup uploaded; verify object version IDs and retention at the destination.')


def restore(args):
    import psycopg
    dsn=os.environ['TEST262_RESTORE_DATABASE_URL']
    archive=Path(args.archive);manifest=json.loads(Path(args.manifest).read_text())
    if hashlib.sha256(archive.read_bytes()).hexdigest()!=manifest['sha256']:
        raise ValueError('Backup digest mismatch')
    with psycopg.connect(dsn,autocommit=True) as db:
        exists=db.execute("SELECT count(*) FROM pg_namespace WHERE nspname IN ('test262','test262_reporting')").fetchone()[0]
        perf=db.execute("SELECT to_regclass('public.perf_results')").fetchone()[0]
        if exists or perf:
            raise ValueError('Restore requires a new empty disposable database')
        # Catalogue privileges from the archive reference only these dedicated roles.
        for role in ('test262_ingest','test262_coordinator','test262_reporter','anon','authenticated','service_role'):
            db.execute('DO $$ BEGIN IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname=\''+role+'\') THEN CREATE ROLE '+role+' NOLOGIN; END IF; END $$')
    run(['pg_restore','--exit-on-error','--single-transaction','--no-owner',str(archive)],dict(os.environ,PGDATABASE=dsn))
    with psycopg.connect(dsn,autocommit=True) as db:
        for name,count in manifest['row_counts'].items():
            if not name.replace('_','').isalnum():
                raise ValueError('Invalid manifest table name')
            if db.execute('SELECT count(*) FROM test262."'+name+'"').fetchone()[0]!=count:
                raise ValueError('Restored table count differs from snapshot: '+name)
        for name,count in manifest['view_counts'].items():
            if not name.replace('_','').isalnum():
                raise ValueError('Invalid manifest view name')
            if db.execute('SELECT count(*) FROM test262_reporting."'+name+'"').fetchone()[0]!=count:
                raise ValueError('Restored reporting count differs from snapshot: '+name)
        integrity=db.execute("SELECT count(*) FROM pg_constraint c JOIN pg_namespace n ON n.oid=c.connamespace WHERE n.nspname='test262' AND NOT convalidated").fetchone()[0]
        if integrity:
            raise ValueError('Unvalidated catalogue constraints after restore')
        if db.execute("SELECT has_table_privilege('test262_ingest','test262.observations','INSERT')").fetchone()[0]:
            raise ValueError('Restored writer unexpectedly has raw table rights')
        views=[r[0] for r in db.execute("SELECT table_name FROM information_schema.views WHERE table_schema='test262_reporting'")]
        for view in views:
            db.execute('SELECT * FROM test262_reporting."'+view+'" LIMIT 1').fetchall()
        # Disable every restored credential binding and bump epoch before any access.
        db.execute('UPDATE test262.api_subjects SET enabled=false')
        db.execute("UPDATE test262.schema_contract SET deployment_state='shadow',minimum_writer_epoch=minimum_writer_epoch+1")
    print(json.dumps({'restore_verified':True,'view_count':len(views),'writer_bindings_disabled':True,'authority':'shadow'}))


def main():
    p=argparse.ArgumentParser();sub=p.add_subparsers(dest='command',required=True)
    b=sub.add_parser('backup');b.add_argument('--output',required=True);b.add_argument('--destination');b.add_argument('--kms-key')
    r=sub.add_parser('restore');r.add_argument('--archive',required=True);r.add_argument('--manifest',required=True)
    args=p.parse_args()
    globals()[args.command](args)


if __name__=='__main__':
    try:
        main()
    except Exception as error:
        print('Catalogue backup/restore failed ('+type(error).__name__+').',file=__import__('sys').stderr)
        raise SystemExit(1)
