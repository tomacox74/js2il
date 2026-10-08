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


LIBPQ_ENVIRONMENT={'host':'PGHOST','hostaddr':'PGHOSTADDR','port':'PGPORT','dbname':'PGDATABASE','user':'PGUSER',
                   'password':'PGPASSWORD','sslmode':'PGSSLMODE','sslrootcert':'PGSSLROOTCERT','sslcert':'PGSSLCERT',
                   'sslkey':'PGSSLKEY','sslcrl':'PGSSLCRL','channel_binding':'PGCHANNELBINDING','options':'PGOPTIONS',
                   'application_name':'PGAPPNAME','connect_timeout':'PGCONNECT_TIMEOUT',
                   'target_session_attrs':'PGTARGETSESSIONATTRS','gssencmode':'PGGSSENCMODE'}


def libpq_environment(dsn):
    """Translate a URI/keyword DSN into libpq variables.

    libpq never expands a URI from PGDATABASE, and argv would expose the password.
    """
    from psycopg.conninfo import conninfo_to_dict
    env={k:v for k,v in os.environ.items() if not k.startswith('PG')}
    for key,value in conninfo_to_dict(dsn).items():
        if key not in LIBPQ_ENVIRONMENT:
            raise ValueError('Unsupported connection parameter for client tools: '+key)
        if value is not None:
            env[LIBPQ_ENVIRONMENT[key]]=str(value)
    return env


def run(command, env=None):
    subprocess.run(command,check=True,env=env)


def create_snapshot(args):
    import psycopg
    dsn=os.environ['TEST262_BACKUP_DATABASE_URL']
    output=Path(args.output);output.mkdir(parents=True,exist_ok=True)
    archive=output/'catalogue.dump'
    env=libpq_environment(dsn)
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
    return archive,output/'manifest.json'


def backup(args):
    from datetime import datetime, timezone
    from .retained_objects import destination, verify_bucket, verify_object
    output=Path(args.output);output.mkdir(parents=True,exist_ok=True)
    report_path=output/'backup-report.json'
    report={'schema_version':1,'backup_verified':False,'stage':'snapshot','objects':{},
            'contains_performance_data':False}
    try:
        archive,manifest=create_snapshot(args)
        if not args.destination:
            return
        if not args.kms_key:
            raise ValueError('An external KMS key is required')
        import boto3
        s3=boto3.client('s3')
        bucket,prefix=destination(args.destination)
        report.update(bucket=bucket,prefix=prefix,stage='retention-metadata')
        verify_bucket(s3,bucket)
        report['bucket_retention_verified']=True
        extra={'ServerSideEncryption':'aws:kms','SSEKMSKeyId':args.kms_key}
        for file in (archive,manifest):
            report['stage']='upload-'+file.name
            key=prefix+'/'+file.name
            s3.upload_file(str(file),bucket,key,ExtraArgs=extra)
            report['stage']='verify-'+file.name
            proof=verify_object(s3,bucket,key,args.kms_key)
            with file.open('rb') as stream:
                local=hashlib.file_digest(stream,'sha256').hexdigest()
            if proof['sha256']!=local or proof['content_length']!=file.stat().st_size:
                raise ValueError('Uploaded backup bytes differ from snapshot')
            report['objects'][file.name]=proof
        report.update(backup_verified=True,stage='complete',checked_at=datetime.now(timezone.utc).isoformat())
        report_path.write_text(json.dumps(report,indent=2)+'\n')
        # Only publish a discoverable receipt after both exact-version readbacks pass.
        key=prefix+'/backup-report.json'
        s3.upload_file(str(report_path),bucket,key,ExtraArgs=extra)
        with report_path.open('rb') as stream:
            pinned=hashlib.file_digest(stream,'sha256').hexdigest()
        receipt=verify_object(s3,bucket,key,args.kms_key)
        if receipt['sha256']!=pinned:
            raise ValueError('Published backup receipt differs')
        report['receipt']=dict(receipt,key=key)
        print(json.dumps({'backup_verified':True,'receipt_version':receipt['version_id']}))
    except Exception as error:
        report.update(backup_verified=False,error_class=type(error).__name__,failed_stage=report['stage'],stage='failed')
        report['aws_error_code']=getattr(error,'response',{}).get('Error',{}).get('Code')
        raise
    finally:
        report.setdefault('checked_at',datetime.now(timezone.utc).isoformat())
        report_path.write_text(json.dumps(report,indent=2)+'\n')


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
        for role in ('test262_ingest','test262_coordinator','test262_reporter','test262_backup','anon','authenticated','service_role'):
            db.execute('DO $$ BEGIN IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname=\''+role+'\') THEN CREATE ROLE '+role+' NOLOGIN; END IF; END $$')
    # Credential fencing is part of the restore transaction: no committed window
    # can expose restored enabled bindings/active authority, even if validation fails.
    with tempfile.TemporaryDirectory(prefix='catalogue-restore-') as directory:
        sql=Path(directory)/'restore.sql'
        run(['pg_restore','--exit-on-error','--no-owner','--file',str(sql),str(archive)])
        with sql.open('a') as stream:
            stream.write("\nUPDATE test262.api_subjects SET enabled=false;\nUPDATE test262.schema_contract SET deployment_state='shadow',minimum_writer_epoch=minimum_writer_epoch+1;\n")
        run(['psql','--no-psqlrc','--set','ON_ERROR_STOP=on','--single-transaction','--file',str(sql)],libpq_environment(dsn))
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
        if db.execute('SELECT count(*) FROM test262.api_subjects WHERE enabled').fetchone()[0]:
            raise ValueError('Restore credential fencing failed')
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
