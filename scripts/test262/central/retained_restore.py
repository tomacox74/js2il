"""Verify retained S3 backup bytes and restore exclusively into disposable loopback PG17."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import time


def recovery_age(manifest, started_at):
    value = manifest.get('snapshot_started_at')
    if value is None:
        return None
    point = datetime.fromisoformat(value)
    if point.tzinfo is None or point > started_at:
        raise ValueError('Invalid backup recovery point')
    return (started_at - point).total_seconds()


def verify(args):
    import boto3
    import psycopg
    from psycopg.conninfo import conninfo_to_dict
    from .retained_objects import (BACKUP_SUFFIX, destination, verify_bucket, verify_object,
                                   latest_receipt, preserve_report, public_report)

    started_at = datetime.now(timezone.utc)
    started = time.monotonic()
    s3 = None
    output = Path(args.output)
    output.mkdir(parents=True, exist_ok=True)
    report = {"restore_verified": False, "independent_retention_verified": False,
              "production_cutover": False, "stage": "validate-input",
              "started_at": started_at.isoformat(), "backup_age_seconds_at_start": None}
    report_path = output / "restore-report.json"
    try:
        # A production DSN cannot be substituted even by environment configuration.
        dsn = os.environ["TEST262_RESTORE_DATABASE_URL"]
        connection = conninfo_to_dict(dsn)
        if (connection.get("host") not in ("127.0.0.1", "localhost")
            or connection.get("hostaddr", os.getenv("PGHOSTADDR", "127.0.0.1")) not in ("127.0.0.1", "::1")):
            raise ValueError("Restore must use loopback")
        if connection.get("dbname") != "catalogue_retained_restore":
            raise ValueError("Restore must use the dedicated disposable database")
        with psycopg.connect(dsn) as db:
            version = db.execute("SHOW server_version_num").fetchone()[0]
            if not 170000 <= int(version) < 180000:
                raise ValueError("PostgreSQL 17 required")
            if db.execute("SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('test262','test262_reporting')) OR to_regclass('public.perf_results') IS NOT NULL").fetchone()[0]:
                raise ValueError("Disposable target is not empty")
        bucket,base=destination(os.environ['TEST262_BACKUP_S3_URI'])
        s3=boto3.client('s3');kms=os.environ['TEST262_BACKUP_KMS_KEY']
        report['stage']='retention-metadata'
        verify_bucket(s3,bucket)
        report['bucket_retention_verified']=True
        receipt=None
        if args.backup_suffix:
            if not BACKUP_SUFFIX.fullmatch(args.backup_suffix):
                raise ValueError('Invalid backup suffix')
            prefix='/'.join(filter(None,(base,args.backup_suffix)))
            report['selection']='explicit-prefix'
        else:
            report['stage']='select-latest-verified-backup'
            prefix,receipt,receipt_proof=latest_receipt(s3,bucket,base,kms,output)
            report.update(selection='latest-verified-receipt',receipt=receipt_proof)
        report.update(bucket=bucket,prefix=prefix,objects={})
        for name in ('catalogue.dump','manifest.json'):
            report['stage']='download-exact-version'
            report['objects'][name]=verify_object(s3,bucket,prefix+'/'+name,kms,
                pinned=receipt['objects'][name] if receipt else None,output=output/name)
        retention_ok=True
        report["stage"] = "digest"
        manifest = json.loads((output / "manifest.json").read_text())
        if report["objects"]["catalogue.dump"]["sha256"] != manifest["sha256"]:
            raise ValueError("Downloaded dump digest differs from manifest")
        report["digest_verified"] = True
        report["independent_retention_verified"] = retention_ok
        report["snapshot_started_at"] = manifest.get("snapshot_started_at")
        report["backup_age_seconds_at_start"] = recovery_age(manifest, started_at)
        report["stage"] = "disposable-restore"
        restore_started = time.monotonic()
        result = subprocess.run(
            [sys.executable, "-m", "scripts.test262.central.backup", "restore",
             "--archive", str(output / "catalogue.dump"),
             "--manifest", str(output / "manifest.json")],
            check=True, capture_output=True, text=True)
        # Keep dump/SQL and raw server diagnostics out of logs and report artifacts.
        restored = json.loads(result.stdout.strip().splitlines()[-1])
        if not restored.get("restore_verified"):
            raise ValueError("Restore tool did not verify")
        with psycopg.connect(dsn) as db:
            state, epoch = db.execute("SELECT deployment_state,minimum_writer_epoch FROM test262.schema_contract").fetchone()
            if state != "shadow" or epoch != int(manifest["contract"]["minimum_writer_epoch"]) + 1:
                raise ValueError("Restored authority fencing mismatch")
            if db.execute("SELECT to_regclass('public.perf_results')").fetchone()[0] is not None:
                raise ValueError("Performance table unexpectedly restored")
        report.update(restore_verified=True, restored_authority=state, restored_epoch=epoch,
                      writer_bindings_disabled=True, row_counts=manifest["row_counts"],
                      view_counts=manifest["view_counts"], stage="complete",
                      isolated_restore_seconds=round(time.monotonic()-restore_started, 6))
        if not retention_ok:
            raise ValueError("Restore passed but retention/encryption acceptance failed")
        status = 0
    except Exception as error:
        report["error_class"] = type(error).__name__
        report.update(failed_stage=report['stage'], stage='failed')
        response = getattr(error, "response", {})
        if isinstance(response, dict):
            report["aws_error_code"] = response.get("Error", {}).get("Code")
        status = 1
    finally:
        report["checked_at"] = datetime.now(timezone.utc).isoformat()
        for name in ("catalogue.dump", "manifest.json", "backup-report.json"):
            (output / name).unlink(missing_ok=True)
        report["elapsed_seconds"] = round(time.monotonic()-started, 6)
        report_path.write_text(json.dumps(public_report(report), indent=2) + "\n")
    report = public_report(report)
    if getattr(args, 'retain_report', False):
        try:
            import re
            run_id = os.environ['GITHUB_RUN_ID']
            attempt = os.environ['GITHUB_RUN_ATTEMPT']
            revision = os.environ['GITHUB_SHA']
            if (not re.fullmatch(r'[1-9]\d*', run_id) or not re.fullmatch(r'[1-9]\d*', attempt)
                    or not re.fullmatch(r'[0-9a-f]{40}', revision)):
                raise ValueError('Invalid report provenance')
            if s3 is None or not report.get('bucket_retention_verified'):
                raise ValueError('No verified retained-report destination')
            report.update(run_id=int(run_id), run_attempt=int(attempt), source_revision=revision)
            report_path.write_text(json.dumps(report, indent=2) + "\n")
            key = '/'.join(filter(None, (base, 'restore-verification', run_id+'-'+attempt,
                                         'restore-report.json')))
            report['retained_report'] = preserve_report(s3, bucket, key, kms, report_path)
            report['report_retention_verified'] = True
        except Exception as error:
            report.update(report_retention_verified=False, retention_error_class=type(error).__name__)
            if status == 0:
                report.update(failed_stage='retain-report', stage='failed')
            status = 1
    report_path.write_text(json.dumps(public_report(report), indent=2) + "\n")
    print(json.dumps({k: report[k] for k in
                     ("stage", "restore_verified", "independent_retention_verified")}))
    return status


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--backup-suffix", default="", help="Exact prefix; omit to restore latest verified receipt, at most 48h old")
    parser.add_argument("--output", required=True)
    parser.add_argument("--retain-report", action="store_true",
                        help="Retain sanitized success/failure report using the existing finite Object Lock policy")
    raise SystemExit(verify(parser.parse_args()))
